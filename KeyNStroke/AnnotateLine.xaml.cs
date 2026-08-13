using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KeyNStroke
{
    public partial class AnnotateLine : Window
    {
        private enum ToolType
        {
            Pencil,
            Arrow,
            Rectangle,
            Ellipse,
            Highlighter,
            Badge,
            Text
        }

        private IMouseRawEventProvider m;
        private IKeystrokeEventProvider k;
        private SettingsStore s;
        private IntPtr windowHandle;

        private bool isDrawingActive = false;
        private bool isDragging = false;
        private Point startPoint;
        private UIElement currentPreviewElement = null;

        private ToolType currentTool = ToolType.Pencil;
        private Color currentColor = (Color)ColorConverter.ConvertFromString("#7EEF84");
        private double currentThickness = 4.0;
        private int badgeCounter = 1;

        private readonly List<UIElement> drawnElements = new List<UIElement>();

        [DllImport("user32.dll")]
        private static extern short GetKeyState(int vKey);
        private const int VK_SHIFT = 0x10;

        public AnnotateLine(IMouseRawEventProvider m, IKeystrokeEventProvider k, SettingsStore s)
        {
            InitializeComponent();

            this.m = m;
            this.s = s;
            this.k = k;

            s.PropertyChanged += SettingChanged;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetAnnotateLineShortcut(s.AnnotateLineShortcut);
            windowHandle = new WindowInteropHelper(this).Handle;
            SetFormStyles();
        }

        #region Shortcut & Toggle

        public string AnnotateLineShortcut;

        public void SetAnnotateLineShortcut(string shortcut)
        {
            if (KeystrokeDisplay.ValidateShortcutSetting(shortcut))
            {
                AnnotateLineShortcut = shortcut;
            }
            else
            {
                AnnotateLineShortcut = s.AnnotateLineShortcutDefault;
            }
        }

        public void ToggleDrawingMode()
        {
            isDrawingActive = !isDrawingActive;
            if (isDrawingActive)
            {
                this.Left = SystemParameters.VirtualScreenLeft;
                this.Top = SystemParameters.VirtualScreenTop;
                this.Width = SystemParameters.VirtualScreenWidth;
                this.Height = SystemParameters.VirtualScreenHeight;

                this.Show();
                this.Activate();
            }
            else
            {
                Clear_Click(null, null);
                this.Hide();
            }
        }

        #endregion

        #region Settings & Window Setup

        private void SettingChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                if (s == null) return;
                switch (e.PropertyName)
                {
                    case "AnnotateLineShortcutTrigger":
                        ToggleDrawingMode();
                        break;
                    case "AnnotateLineColor":
                        currentColor = UIHelper.ToMediaColor(s.AnnotateLineColor);
                        break;
                    case "AnnotateLineShortcut":
                        SetAnnotateLineShortcut(s.AnnotateLineShortcut);
                        break;
                }
            }));
        }

        private void SetFormStyles()
        {
            NativeMethodsGWL.HideFromAltTab(windowHandle);
        }

        #endregion

        #region Toolbar Events

        private void Tool_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.Tag != null && Enum.TryParse(rb.Tag.ToString(), out ToolType tool))
            {
                currentTool = tool;
            }
        }

        private void Color_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                if (ColorConverter.ConvertFromString(btn.Tag.ToString()) is Color c)
                {
                    UpdateColorSelectionUI(c);
                }
            }
        }

        private void CustomColorPicker_Click(object sender, RoutedEventArgs e)
        {
            using (var dlg = new System.Windows.Forms.ColorDialog())
            {
                dlg.Color = System.Drawing.Color.FromArgb(currentColor.A, currentColor.R, currentColor.G, currentColor.B);
                dlg.FullOpen = true;
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Color c = Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B);
                    UpdateColorSelectionUI(c);
                }
            }
        }

        private void UpdateColorSelectionUI(Color c)
        {
            currentColor = c;
            if (btnCustomColor != null)
            {
                btnCustomColor.Background = new SolidColorBrush(c);
                double luminance = (0.299 * c.R + 0.587 * c.G + 0.114 * c.B) / 255;
                btnCustomColor.Foreground = luminance < 0.5 ? Brushes.White : Brushes.Black;
            }
        }

        private void Thickness_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboThickness?.SelectedItem is ComboBoxItem item && item.Tag != null)
            {
                if (double.TryParse(item.Tag.ToString(), out double th))
                {
                    currentThickness = th;
                }
            }
        }

        private void Draw_Click(object sender, RoutedEventArgs e)
        {
            var list = new List<UIElement>(drawnElements);
            // Ẩn thanh công cụ trước khi chụp
            annotationToolbar.Visibility = Visibility.Collapsed;
            this.UpdateLayout();

            ((App)Application.Current).showDrawWindow(list, drawingCanvas);

            annotationToolbar.Visibility = Visibility.Visible;
            this.Hide();
            isDrawingActive = false;
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (drawnElements.Count > 0)
            {
                var last = drawnElements[drawnElements.Count - 1];
                drawingCanvas.Children.Remove(last);
                drawnElements.RemoveAt(drawnElements.Count - 1);
                if (last is Border b && b.Tag?.ToString() == "Badge")
                {
                    badgeCounter = Math.Max(1, badgeCounter - 1);
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            drawingCanvas.Children.Clear();
            drawnElements.Clear();
            badgeCounter = 1;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (isDrawingActive)
            {
                ToggleDrawingMode();
            }
        }



        private bool isToolbarDragging = false;
        private Point toolbarDragStartPos;

        private void DragGrip_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isToolbarDragging = true;
            toolbarDragStartPos = e.GetPosition(mainGrid);
            dragGrip.CaptureMouse();
            e.Handled = true;
        }

        private void DragGrip_MouseMove(object sender, MouseEventArgs e)
        {
            if (isToolbarDragging)
            {
                Point currentPos = e.GetPosition(mainGrid);
                double deltaX = currentPos.X - toolbarDragStartPos.X;
                double deltaY = currentPos.Y - toolbarDragStartPos.Y;

                Thickness margin = annotationToolbar.Margin;
                annotationToolbar.HorizontalAlignment = HorizontalAlignment.Left;
                annotationToolbar.VerticalAlignment = VerticalAlignment.Top;

                double currentLeft = (annotationToolbar.ActualWidth > 0 && margin.Left == 0) ? (mainGrid.ActualWidth - annotationToolbar.ActualWidth) / 2 : margin.Left;
                double currentTop = margin.Top;

                annotationToolbar.Margin = new Thickness(Math.Max(0, currentLeft + deltaX), Math.Max(0, currentTop + deltaY), 0, 0);
                toolbarDragStartPos = currentPos;
            }
        }

        private void DragGrip_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isToolbarDragging)
            {
                isToolbarDragging = false;
                try { dragGrip.ReleaseMouseCapture(); } catch { }
                e.Handled = true;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Undo_Click(null, null);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                Exit_Click(null, null);
                e.Handled = true;
            }
            else if (e.Key == Key.P)
            {
                btnToolPencil.IsChecked = true;
                currentTool = ToolType.Pencil;
            }
            else if (e.Key == Key.A)
            {
                btnToolArrow.IsChecked = true;
                currentTool = ToolType.Arrow;
            }
            else if (e.Key == Key.R)
            {
                btnToolRect.IsChecked = true;
                currentTool = ToolType.Rectangle;
            }
            else if (e.Key == Key.E || e.Key == Key.C)
            {
                btnToolEllipse.IsChecked = true;
                currentTool = ToolType.Ellipse;
            }
            else if (e.Key == Key.H)
            {
                btnToolHighlighter.IsChecked = true;
                currentTool = ToolType.Highlighter;
            }
            else if (e.Key == Key.B)
            {
                btnToolBadge.IsChecked = true;
                currentTool = ToolType.Badge;
            }
            else if (e.Key == Key.T)
            {
                btnToolText.IsChecked = true;
                currentTool = ToolType.Text;
            }
        }

        #endregion

        #region Mouse Canvas Drawing

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isDrawingActive) return;

            Point mousePos = e.GetPosition(mainGrid);
            Point toolbarPos = annotationToolbar.TranslatePoint(new Point(0, 0), mainGrid);
            Rect toolbarRect = new Rect(toolbarPos, new Size(annotationToolbar.ActualWidth, annotationToolbar.ActualHeight));
            if (toolbarRect.Contains(mousePos))
            {
                return;
            }

            startPoint = e.GetPosition(drawingCanvas);

            if (currentTool == ToolType.Badge)
            {
                CreateBadge(startPoint);
                return;
            }

            if (currentTool == ToolType.Text)
            {
                CreateTextBox(startPoint);
                return;
            }

            isDragging = true;
            mainGrid.CaptureMouse();
            InitPreviewElement(startPoint);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawingActive || !isDragging || currentPreviewElement == null) return;

            Point currPoint = e.GetPosition(drawingCanvas);
            UpdatePreviewElement(startPoint, currPoint);
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDrawingActive) return;

            if (isDragging)
            {
                isDragging = false;
                try { mainGrid.ReleaseMouseCapture(); } catch { }
                if (currentPreviewElement != null)
                {
                    drawnElements.Add(currentPreviewElement);
                    currentPreviewElement = null;
                }
            }
        }

        #endregion

        #region Shape Helpers

        private void CreateBadge(Point p)
        {
            Border badge = new Border
            {
                Width = 28,
                Height = 28,
                CornerRadius = new CornerRadius(14),
                Background = new SolidColorBrush(currentColor),
                Effect = new DropShadowEffect { BlurRadius = 6, ShadowDepth = 2, Opacity = 0.4 },
                Tag = "Badge"
            };

            TextBlock txt = new TextBlock
            {
                Text = badgeCounter.ToString(),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            badge.Child = txt;

            Canvas.SetLeft(badge, p.X - 14);
            Canvas.SetTop(badge, p.Y - 14);

            drawingCanvas.Children.Add(badge);
            drawnElements.Add(badge);
            badgeCounter++;
        }

        private void CreateTextBox(Point p)
        {
            TextBox tb = new TextBox
            {
                Background = new SolidColorBrush(Color.FromArgb(40, 0, 0, 0)),
                Foreground = new SolidColorBrush(currentColor),
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                BorderBrush = new SolidColorBrush(currentColor),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(4),
                Text = "Text...",
                MinWidth = 80
            };

            Canvas.SetLeft(tb, p.X);
            Canvas.SetTop(tb, p.Y);

            drawingCanvas.Children.Add(tb);
            drawnElements.Add(tb);
            tb.Focus();
            tb.SelectAll();
        }

        private void InitPreviewElement(Point p)
        {
            switch (currentTool)
            {
                case ToolType.Pencil:
                    Polyline poly = new Polyline
                    {
                        Stroke = new SolidColorBrush(currentColor),
                        StrokeThickness = currentThickness,
                        StrokeLineJoin = PenLineJoin.Round,
                        StrokeStartLineCap = PenLineCap.Round,
                        StrokeEndLineCap = PenLineCap.Round
                    };
                    poly.Points.Add(p);
                    currentPreviewElement = poly;
                    break;

                case ToolType.Highlighter:
                    Polyline hl = new Polyline
                    {
                        Stroke = new SolidColorBrush(currentColor),
                        StrokeThickness = currentThickness * 3.5,
                        Opacity = 0.45,
                        StrokeLineJoin = PenLineJoin.Round,
                        StrokeStartLineCap = PenLineCap.Round,
                        StrokeEndLineCap = PenLineCap.Round
                    };
                    hl.Points.Add(p);
                    currentPreviewElement = hl;
                    break;

                case ToolType.Arrow:
                    Path arrowPath = new Path
                    {
                        Stroke = new SolidColorBrush(currentColor),
                        StrokeThickness = currentThickness,
                        Fill = new SolidColorBrush(currentColor),
                        StrokeLineJoin = PenLineJoin.Round,
                        StrokeStartLineCap = PenLineCap.Round,
                        StrokeEndLineCap = PenLineCap.Round
                    };
                    currentPreviewElement = arrowPath;
                    break;

                case ToolType.Rectangle:
                    Rectangle rect = new Rectangle
                    {
                        Stroke = new SolidColorBrush(currentColor),
                        StrokeThickness = currentThickness,
                        Fill = Brushes.Transparent
                    };
                    Canvas.SetLeft(rect, p.X);
                    Canvas.SetTop(rect, p.Y);
                    currentPreviewElement = rect;
                    break;

                case ToolType.Ellipse:
                    Ellipse el = new Ellipse
                    {
                        Stroke = new SolidColorBrush(currentColor),
                        StrokeThickness = currentThickness,
                        Fill = Brushes.Transparent
                    };
                    Canvas.SetLeft(el, p.X);
                    Canvas.SetTop(el, p.Y);
                    currentPreviewElement = el;
                    break;
            }

            if (currentPreviewElement != null)
            {
                drawingCanvas.Children.Add(currentPreviewElement);
            }
        }

        private void UpdatePreviewElement(Point start, Point curr)
        {
            switch (currentTool)
            {
                case ToolType.Pencil:
                    if (currentPreviewElement is Polyline poly)
                    {
                        bool isShiftPressed = (GetKeyState(VK_SHIFT) & 0x8000) != 0;
                        if (isShiftPressed)
                        {
                            double dx = curr.X - start.X;
                            double dy = curr.Y - start.Y;
                            if (dx != 0 || dy != 0)
                            {
                                double angle = Math.Atan2(dy, dx);
                                double angleDeg = angle * 180.0 / Math.PI;
                                double roundedAngle = Math.Round(angleDeg / 45.0) * 45.0;
                                double rad = roundedAngle * Math.PI / 180.0;
                                double dist = Math.Sqrt(dx * dx + dy * dy);
                                Point constrained = new Point(start.X + dist * Math.Cos(rad), start.Y + dist * Math.Sin(rad));

                                poly.Points.Clear();
                                poly.Points.Add(start);
                                poly.Points.Add(constrained);
                            }
                        }
                        else
                        {
                            if (poly.Points.Count == 0 || poly.Points.Last() != curr)
                            {
                                poly.Points.Add(curr);
                            }
                        }
                    }
                    break;

                case ToolType.Highlighter:
                    if (currentPreviewElement is Polyline hlPoly)
                    {
                        if (hlPoly.Points.Count == 0 || hlPoly.Points.Last() != curr)
                        {
                            hlPoly.Points.Add(curr);
                        }
                    }
                    break;

                case ToolType.Arrow:
                    if (currentPreviewElement is Path arrowPath)
                    {
                        arrowPath.Data = CreateArrowGeometry(start, curr, currentThickness);
                    }
                    break;

                case ToolType.Rectangle:
                    if (currentPreviewElement is Rectangle rect)
                    {
                        double x = Math.Min(start.X, curr.X);
                        double y = Math.Min(start.Y, curr.Y);
                        double w = Math.Abs(start.X - curr.X);
                        double h = Math.Abs(start.Y - curr.Y);

                        Canvas.SetLeft(rect, x);
                        Canvas.SetTop(rect, y);
                        rect.Width = Math.Max(1, w);
                        rect.Height = Math.Max(1, h);
                    }
                    break;

                case ToolType.Ellipse:
                    if (currentPreviewElement is Ellipse el)
                    {
                        double x = Math.Min(start.X, curr.X);
                        double y = Math.Min(start.Y, curr.Y);
                        double w = Math.Abs(start.X - curr.X);
                        double h = Math.Abs(start.Y - curr.Y);

                        Canvas.SetLeft(el, x);
                        Canvas.SetTop(el, y);
                        el.Width = Math.Max(1, w);
                        el.Height = Math.Max(1, h);
                    }
                    break;
            }
        }

        private Geometry CreateArrowGeometry(Point p1, Point p2, double thickness)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            double angle = Math.Atan2(dy, dx);
            double arrowLen = Math.Max(14, thickness * 3.5);

            double angle1 = angle + Math.PI - (Math.PI / 6.0);
            double angle2 = angle + Math.PI + (Math.PI / 6.0);

            Point pArrow1 = new Point(p2.X + arrowLen * Math.Cos(angle1), p2.Y + arrowLen * Math.Sin(angle1));
            Point pArrow2 = new Point(p2.X + arrowLen * Math.Cos(angle2), p2.Y + arrowLen * Math.Sin(angle2));

            PathGeometry geom = new PathGeometry();
            PathFigure figure = new PathFigure { StartPoint = p1, IsClosed = false };
            figure.Segments.Add(new LineSegment(p2, true));

            PathFigure headFigure = new PathFigure { StartPoint = p2, IsClosed = true, IsFilled = true };
            headFigure.Segments.Add(new LineSegment(pArrow1, true));
            headFigure.Segments.Add(new LineSegment(pArrow2, true));

            geom.Figures.Add(figure);
            geom.Figures.Add(headFigure);
            return geom;
        }

        #endregion

        private void Window_Closed(object sender, EventArgs e)
        {
            if (s != null)
            {
                s.PropertyChanged -= SettingChanged;
            }
            m = null;
            s = null;
            k = null;
        }
    }
}
