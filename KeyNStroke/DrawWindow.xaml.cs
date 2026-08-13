using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KeyNStroke
{
    public partial class DrawWindow : Window
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

        private class TabState
        {
            public string Name { get; set; }
            public List<UIElement> DrawnElements { get; } = new List<UIElement>();
            public List<List<UIElementState>> UndoStack { get; } = new List<List<UIElementState>>();
            public List<List<UIElementState>> RedoStack { get; } = new List<List<UIElementState>>();
            public int BadgeCounter { get; set; } = 1;
            public double ZoomFactor { get; set; } = 1.0;
        }

        private class UIElementState
        {
            public UIElement Element { get; set; }
            public double Left { get; set; }
            public double Top { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
            public PointCollection PolylinePoints { get; set; }
            public Geometry PathGeometry { get; set; }
            public string Text { get; set; }
            public string BadgeNumber { get; set; }
            public bool IsDeleted { get; set; }
        }

        private SettingsStore settings;
        private ToolType currentTool = ToolType.Pencil;
        private Color currentColor = (Color)ColorConverter.ConvertFromString("#7EEF84");
        private double currentThickness = 4.0;

        private List<TabState> tabs = new List<TabState>();
        private TabState currentTabState = null;

        // Object editing variables
        private UIElement activeDragElement = null;
        private Point dragStartMousePos;
        private Point dragStartElementPos;
        private List<Point> dragStartPolyPoints = null;
        private Point dragStartArrowP1;
        private Point dragStartArrowP2;
        private bool isDragMoved = false;

        private UIElement currentPreviewElement = null;
        private Point startPoint;
        private bool isDrawing = false;
        private UIElement selectedElement = null;

        public DrawWindow(SettingsStore s)
        {
            InitializeComponent();
            this.settings = s;
            AddNewTab();
            UpdateColorSelectionUI(currentColor);
        }

        private void AddNewTab(ImageSource bgImage = null)
        {
            var tabState = new TabState { Name = "Tab " + (tabs.Count + 1) };
            tabs.Add(tabState);

            var tabItem = new TabItem
            {
                Header = tabState.Name,
                Tag = tabState
            };

            var grid = new Grid { Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0)) };
            grid.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            grid.MouseMove += Canvas_MouseMove;
            grid.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;

            if (bgImage != null)
            {
                grid.Children.Add(new Image 
                { 
                    Source = bgImage, 
                    Stretch = Stretch.Uniform,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                });
            }

            var canvas = new Canvas 
            { 
                Background = Brushes.Transparent,
                RenderTransform = new ScaleTransform(1.0, 1.0)
            };
            grid.Children.Add(canvas);
            tabItem.Content = grid;

            tabControl.Items.Add(tabItem);
            tabControl.SelectedItem = tabItem;
            currentTabState = tabState;
        }

        public void ImportFromAnnotate(List<UIElement> elements, ImageSource bgImage)
        {
            AddNewTab(bgImage);
            var canvas = CurrentCanvas;
            if (canvas == null) return;

            // Lưu trạng thái canvas rỗng trước khi nạp đối tượng vào UndoStack
            SaveUndoState();

            foreach (var elem in elements)
            {
                // Clone hoặc copy element sang canvas mới
                UIElement cloned = CloneElement(elem);
                if (cloned != null)
                {
                    canvas.Children.Add(cloned);
                    currentTabState.DrawnElements.Add(cloned);
                }
            }
        }

        private UIElement CloneElement(UIElement elem)
        {
            if (elem is Polyline poly)
            {
                return new Polyline
                {
                    Stroke = poly.Stroke,
                    StrokeThickness = poly.StrokeThickness,
                    StrokeLineJoin = poly.StrokeLineJoin,
                    StrokeStartLineCap = poly.StrokeStartLineCap,
                    StrokeEndLineCap = poly.StrokeEndLineCap,
                    Points = new PointCollection(poly.Points)
                };
            }
            if (elem is Path path)
            {
                return new Path
                {
                    Stroke = path.Stroke,
                    StrokeThickness = path.StrokeThickness,
                    Fill = path.Fill,
                    StrokeLineJoin = path.StrokeLineJoin,
                    StrokeStartLineCap = path.StrokeStartLineCap,
                    StrokeEndLineCap = path.StrokeEndLineCap,
                    Data = path.Data.Clone()
                };
            }
            if (elem is Rectangle rect)
            {
                var r = new Rectangle
                {
                    Stroke = rect.Stroke,
                    StrokeThickness = rect.StrokeThickness,
                    Fill = rect.Fill,
                    Width = rect.Width,
                    Height = rect.Height
                };
                Canvas.SetLeft(r, Canvas.GetLeft(rect));
                Canvas.SetTop(r, Canvas.GetTop(rect));
                return r;
            }
            if (elem is Ellipse el)
            {
                var e = new Ellipse
                {
                    Stroke = el.Stroke,
                    StrokeThickness = el.StrokeThickness,
                    Fill = el.Fill,
                    Width = el.Width,
                    Height = el.Height
                };
                Canvas.SetLeft(e, Canvas.GetLeft(el));
                Canvas.SetTop(e, Canvas.GetTop(el));
                return e;
            }
            if (elem is TextBox tb)
            {
                var t = new TextBox
                {
                    Background = tb.Background,
                    Foreground = tb.Foreground,
                    FontSize = tb.FontSize,
                    FontWeight = tb.FontWeight,
                    BorderBrush = tb.BorderBrush,
                    BorderThickness = tb.BorderThickness,
                    Padding = tb.Padding,
                    Text = tb.Text,
                    MinWidth = tb.MinWidth
                };
                t.KeyDown += (s, ev) =>
                {
                    if (ev.Key == Key.Enter)
                    {
                        Keyboard.ClearFocus();
                    }
                };
                Canvas.SetLeft(t, Canvas.GetLeft(tb));
                Canvas.SetTop(t, Canvas.GetTop(tb));
                return t;
            }
            if (elem is Border b && b.Tag?.ToString() == "Badge")
            {
                var badge = new Border
                {
                    Width = b.Width,
                    Height = b.Height,
                    CornerRadius = b.CornerRadius,
                    Background = b.Background,
                    Effect = b.Effect,
                    Tag = "Badge"
                };
                if (b.Child is TextBlock txt)
                {
                    badge.Child = new TextBlock
                    {
                        Text = txt.Text,
                        FontSize = txt.FontSize,
                        FontWeight = txt.FontWeight,
                        Foreground = txt.Foreground,
                        HorizontalAlignment = txt.HorizontalAlignment,
                        VerticalAlignment = txt.VerticalAlignment
                    };
                }
                Canvas.SetLeft(badge, Canvas.GetLeft(b));
                Canvas.SetTop(badge, Canvas.GetTop(b));
                return badge;
            }
            return null;
        }

        private Canvas CurrentCanvas
        {
            get
            {
                if (tabControl.SelectedItem is TabItem ti && ti.Content is Grid g)
                {
                    return g.Children.OfType<Canvas>().FirstOrDefault();
                }
                return null;
            }
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedItem is TabItem ti && ti.Tag is TabState state)
            {
                currentTabState = state;
            }
        }

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

        private void NewTab_Click(object sender, RoutedEventArgs e)
        {
            AddNewTab();
        }

        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.Items.Count > 1 && tabControl.SelectedItem is TabItem ti)
            {
                if (ti.Tag is TabState state)
                {
                    tabs.Remove(state);
                }
                tabControl.Items.Remove(ti);
                if (tabControl.Items.Count > 0)
                {
                    tabControl.SelectedIndex = tabControl.Items.Count - 1;
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            var canvas = CurrentCanvas;
            if (canvas != null && currentTabState != null)
            {
                SaveUndoState();
                canvas.Children.Clear();
                currentTabState.DrawnElements.Clear();
                currentTabState.BadgeCounter = 1;
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void SaveUndoState()
        {
            if (currentTabState == null) return;
            var stateList = new List<UIElementState>();
            foreach (var elem in currentTabState.DrawnElements)
            {
                stateList.Add(CaptureElementState(elem));
            }
            currentTabState.UndoStack.Add(stateList);
            currentTabState.RedoStack.Clear();
        }

        private UIElementState CaptureElementState(UIElement elem)
        {
            var state = new UIElementState
            {
                Element = elem,
                Left = Canvas.GetLeft(elem),
                Top = Canvas.GetTop(elem)
            };

            if (elem is FrameworkElement fe)
            {
                state.Width = fe.Width;
                state.Height = fe.Height;
            }
            if (elem is Polyline poly)
            {
                state.PolylinePoints = new PointCollection(poly.Points);
            }
            else if (elem is Path path && path.Data is PathGeometry pg)
            {
                state.PathGeometry = pg.Clone();
            }
            else if (elem is TextBox tb)
            {
                state.Text = tb.Text;
            }
            else if (elem is Border b && b.Tag?.ToString() == "Badge")
            {
                state.Width = b.Width;
                state.Height = b.Height;
                if (b.Child is TextBlock txt)
                {
                    state.BadgeNumber = txt.Text;
                }
            }
            return state;
        }

        private void RestoreStateList(List<UIElementState> stateList)
        {
            var canvas = CurrentCanvas;
            if (canvas == null || currentTabState == null) return;

            canvas.Children.Clear();
            currentTabState.DrawnElements.Clear();

            foreach (var state in stateList)
            {
                if (state.IsDeleted) continue;

                var elem = state.Element;
                Canvas.SetLeft(elem, state.Left);
                Canvas.SetTop(elem, state.Top);

                if (elem is FrameworkElement fe)
                {
                    fe.Width = state.Width;
                    fe.Height = state.Height;
                }
                if (elem is Polyline poly && state.PolylinePoints != null)
                {
                    poly.Points = new PointCollection(state.PolylinePoints);
                }
                else if (elem is Path path && state.PathGeometry != null)
                {
                    path.Data = state.PathGeometry.Clone();
                }
                else if (elem is TextBox tb)
                {
                    tb.Text = state.Text;
                }
                else if (elem is Border b && b.Tag?.ToString() == "Badge")
                {
                    b.Width = state.Width;
                    b.Height = state.Height;
                    if (b.Child is TextBlock txt && state.BadgeNumber != null)
                    {
                        txt.Text = state.BadgeNumber;
                    }
                }

                canvas.Children.Add(elem);
                currentTabState.DrawnElements.Add(elem);
            }
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            PerformUndo();
        }

        private void PerformUndo()
        {
            if (currentTabState == null || currentTabState.UndoStack.Count == 0) return;
            var currentState = currentTabState.DrawnElements.Select(CaptureElementState).ToList();
            currentTabState.RedoStack.Add(currentState);

            var prev = currentTabState.UndoStack[currentTabState.UndoStack.Count - 1];
            currentTabState.UndoStack.RemoveAt(currentTabState.UndoStack.Count - 1);
            RestoreStateList(prev);
        }

        private void PerformRedo()
        {
            if (currentTabState == null || currentTabState.RedoStack.Count == 0) return;
            var currentState = currentTabState.DrawnElements.Select(CaptureElementState).ToList();
            currentTabState.UndoStack.Add(currentState);

            var next = currentTabState.RedoStack[currentTabState.RedoStack.Count - 1];
            currentTabState.RedoStack.RemoveAt(currentTabState.RedoStack.Count - 1);
            RestoreStateList(next);
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var canvas = CurrentCanvas;
            if (canvas == null) return;

            Point mousePos = e.GetPosition(canvas);
            
            // Check if user clicked an existing element to edit/drag
            UIElement clickedElement = null;
            foreach (var elem in currentTabState.DrawnElements)
            {
                Point posInElem = e.GetPosition(elem);
                bool hit = false;
                
                if (elem is Polyline poly)
                {
                    // Tạo một StreamGeometry mô phỏng polyline để gọi StrokeContains
                    StreamGeometry geom = new StreamGeometry();
                    using (StreamGeometryContext ctx = geom.Open())
                    {
                        if (poly.Points.Count > 0)
                        {
                            ctx.BeginFigure(poly.Points[0], false, false);
                            for (int i = 1; i < poly.Points.Count; i++)
                            {
                                ctx.LineTo(poly.Points[i], true, false);
                            }
                        }
                    }
                    // Tăng độ dày ảo lên 15px để dễ bấm trúng nét vẽ mảnh
                    Pen testPen = new Pen(Brushes.Black, Math.Max(poly.StrokeThickness, 15.0));
                    hit = geom.StrokeContains(testPen, posInElem);
                }
                else if (elem is Path path && path.Data != null)
                {
                    Pen testPen = new Pen(Brushes.Black, Math.Max(path.StrokeThickness, 15.0));
                    hit = path.Data.StrokeContains(testPen, posInElem) || path.Data.FillContains(posInElem);
                }
                else
                {
                    hit = elem.InputHitTest(posInElem) != null;
                }

                if (hit)
                {
                    clickedElement = elem;
                    break;
                }
            }

            if (clickedElement != null)
            {
                selectedElement = clickedElement;
                activeDragElement = clickedElement;
                dragStartMousePos = e.GetPosition(canvas);
                dragStartElementPos = new Point(Canvas.GetLeft(clickedElement), Canvas.GetTop(clickedElement));
                isDragMoved = false;

                if (clickedElement is Polyline poly)
                {
                    dragStartPolyPoints = poly.Points.ToList();
                }
                else if (clickedElement is Path path && path.Data is PathGeometry pg && pg.Figures.Count > 0)
                {
                    dragStartArrowP1 = pg.Figures[0].StartPoint;
                    if (pg.Figures[0].Segments.Count > 0 && pg.Figures[0].Segments[0] is LineSegment ls)
                    {
                        dragStartArrowP2 = ls.Point;
                    }
                }
                else
                {
                    dragStartPolyPoints = null;
                }

                // If Badge or Textbox: prepare edit on double click / click
                if (clickedElement is TextBox tb)
                {
                    tb.Focus();
                }
                else if (clickedElement is Border b && b.Tag?.ToString() == "Badge")
                {
                    // Start editing badge number
                    StartEditingBadge(b);
                }

                e.Handled = true;
                return;
            }
            else
            {
                selectedElement = null;
            }

            // Start drawing new element
            SaveUndoState();
            startPoint = mousePos;

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

            isDrawing = true;
            InitPreviewElement(startPoint);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var canvas = CurrentCanvas;
            if (canvas == null) return;

            Point mousePos = e.GetPosition(canvas);

            if (activeDragElement != null && e.LeftButton == MouseButtonState.Pressed)
            {
                double dx = mousePos.X - dragStartMousePos.X;
                double dy = mousePos.Y - dragStartMousePos.Y;
                if (Math.Abs(dx) > 2 || Math.Abs(dy) > 2)
                {
                    isDragMoved = true;
                }

                if (activeDragElement is Polyline poly && dragStartPolyPoints != null)
                {
                    var newPoints = new PointCollection();
                    foreach (var p in dragStartPolyPoints)
                    {
                        newPoints.Add(new Point(p.X + dx, p.Y + dy));
                    }
                    poly.Points = newPoints;
                }
                else if (activeDragElement is Path path && path.Data is PathGeometry pg && pg.Figures.Count > 0)
                {
                    Point newP1 = new Point(dragStartArrowP1.X + dx, dragStartArrowP1.Y + dy);
                    Point newP2 = new Point(dragStartArrowP2.X + dx, dragStartArrowP2.Y + dy);
                    path.Data = CreateArrowGeometry(newP1, newP2, path.StrokeThickness);
                }
                else
                {
                    Canvas.SetLeft(activeDragElement, dragStartElementPos.X + dx);
                    Canvas.SetTop(activeDragElement, dragStartElementPos.Y + dy);
                }
                return;
            }

            if (isDrawing && currentPreviewElement != null)
            {
                UpdatePreviewElement(startPoint, mousePos);
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (activeDragElement != null)
            {
                if (isDragMoved)
                {
                    SaveUndoState();
                }
                activeDragElement = null;
            }

            if (isDrawing)
            {
                isDrawing = false;
                if (currentPreviewElement != null)
                {
                    currentTabState?.DrawnElements.Add(currentPreviewElement);
                    currentPreviewElement = null;
                }
            }
        }

        private void StartEditingBadge(Border badge)
        {
            if (!(badge.Child is TextBlock txt)) return;
            var canvas = CurrentCanvas;
            if (canvas == null) return;

            SaveUndoState();

            TextBox tbEdit = new TextBox
            {
                Width = 40,
                Height = 24,
                Text = txt.Text,
                FontWeight = FontWeights.Bold,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };

            Canvas.SetLeft(tbEdit, Canvas.GetLeft(badge) - 6);
            Canvas.SetTop(tbEdit, Canvas.GetTop(badge) + 2);

            canvas.Children.Add(tbEdit);
            tbEdit.Focus();
            tbEdit.SelectAll();

            tbEdit.KeyDown += (s, ev) =>
            {
                if (ev.Key == Key.Enter)
                {
                    ConfirmBadgeEdit(badge, txt, tbEdit);
                }
            };
            tbEdit.LostFocus += (s, ev) =>
            {
                ConfirmBadgeEdit(badge, txt, tbEdit);
            };
        }

        private void ConfirmBadgeEdit(Border badge, TextBlock txt, TextBox tbEdit)
        {
            var canvas = CurrentCanvas;
            if (canvas == null || !canvas.Children.Contains(tbEdit)) return;

            string newText = tbEdit.Text.Trim();
            if (!string.IsNullOrEmpty(newText))
            {
                txt.Text = newText;
                if (int.TryParse(newText, out int parsedVal))
                {
                    currentTabState.BadgeCounter = Math.Max(currentTabState.BadgeCounter, parsedVal + 1);
                }
            }
            canvas.Children.Remove(tbEdit);
        }

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
                Text = currentTabState.BadgeCounter.ToString(),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            badge.Child = txt;

            Canvas.SetLeft(badge, p.X - 14);
            Canvas.SetTop(badge, p.Y - 14);

            CurrentCanvas.Children.Add(badge);
            currentTabState.DrawnElements.Add(badge);
            currentTabState.BadgeCounter++;
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

            tb.KeyDown += (s, ev) =>
            {
                if (ev.Key == Key.Enter)
                {
                    Keyboard.ClearFocus();
                }
            };

            CurrentCanvas.Children.Add(tb);
            currentTabState.DrawnElements.Add(tb);
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
                CurrentCanvas.Children.Add(currentPreviewElement);
            }
        }

        private void UpdatePreviewElement(Point start, Point curr)
        {
            switch (currentTool)
            {
                case ToolType.Pencil:
                case ToolType.Highlighter:
                    if (currentPreviewElement is Polyline poly)
                    {
                        if (poly.Points.Count == 0 || poly.Points.Last() != curr)
                        {
                            poly.Points.Add(curr);
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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                PerformUndo();
                e.Handled = true;
            }
            else if (e.Key == Key.Y && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                PerformRedo();
                e.Handled = true;
            }
            else if ((e.Key == Key.OemPlus || e.Key == Key.Add) && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ZoomIn();
                e.Handled = true;
            }
            else if ((e.Key == Key.OemMinus || e.Key == Key.Subtract) && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ZoomOut();
                e.Handled = true;
            }
            else if (e.Key == Key.Delete)
            {
                if (selectedElement != null && currentTabState != null)
                {
                    var canvas = CurrentCanvas;
                    if (canvas != null)
                    {
                        SaveUndoState();
                        canvas.Children.Remove(selectedElement);
                        currentTabState.DrawnElements.Remove(selectedElement);
                        selectedElement = null;
                        e.Handled = true;
                    }
                }
            }
            else if (e.Key == Key.Escape)
            {
                this.WindowState = WindowState.Minimized;
                e.Handled = true;
            }
        }

        private void ZoomIn()
        {
            if (currentTabState == null) return;
            double newZoom = Math.Min(10.0, currentTabState.ZoomFactor + 0.1);
            ApplyZoom(newZoom);
        }

        private void ZoomOut()
        {
            if (currentTabState == null) return;
            double newZoom = Math.Max(0.3, currentTabState.ZoomFactor - 0.1);
            ApplyZoom(newZoom);
        }

        private void ApplyZoom(double factor)
        {
            if (currentTabState == null) return;
            currentTabState.ZoomFactor = factor;
            var canvas = CurrentCanvas;
            if (canvas != null && canvas.RenderTransform is ScaleTransform st)
            {
                st.ScaleX = factor;
                st.ScaleY = factor;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = WindowState.Minimized;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ((App)Application.Current).onDrawWindowClosed();
        }
    }
}
