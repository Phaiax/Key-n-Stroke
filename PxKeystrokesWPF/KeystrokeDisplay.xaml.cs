using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PxKeystrokesWPF
{
    /// <summary>
    /// Interaktionslogik für KeystrokeDisplay.xaml
    /// </summary>
    public partial class KeystrokeDisplay : Window
    {
        SettingsStore settings;
        IKeystrokeEventProvider k;
        IntPtr windowHandle;
        Brush OrigInnerPanelBackgroundColor;

        public KeystrokeDisplay(IKeystrokeEventProvider k, SettingsStore s)
        {
            InitializeComponent();
            InitializeAnimations();

            this.k = k;

            this.settings = s;
            this.settings.EnableSettingsMode = false;
            this.settings.PropertyChanged += settingChanged;
            this.settings.CallPropertyChangedForAllProperties();

            this.buttonResizeWindow.Settings = s;
            this.buttonResizeInnerPanel.Settings = s;

            //addWelcomeInfo();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Window handle is available
            InitPeriodicTopmostTimer();
            windowHandle = new WindowInteropHelper(this).Handle;

            this.k.KeystrokeEvent += k_KeystrokeEvent;

            OrigInnerPanelBackgroundColor = innerPanel.Background;
            ActivateDisplayOnlyMode(true);

            if (settings.EnableWindowFade)
            {
                FadeOut();
            }
        }

        #region periodically make TopMost
        DispatcherTimer makeTopMostTimer = new DispatcherTimer();
        void InitPeriodicTopmostTimer()
        {
            makeTopMostTimer.Tick += (object sender, EventArgs e) =>
            {
                IntPtr handle = new WindowInteropHelper(this).Handle;
                NativeMethodsWindow.SetWindowTopMost(handle);
            };
            makeTopMostTimer.Interval = TimeSpan.FromSeconds(1.0);

            settings.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == "PeriodicTopmost")
                {
                    if (settings.PeriodicTopmost)
                    {
                        makeTopMostTimer.Start();
                    }
                    else
                    {
                        makeTopMostTimer.Stop();
                    }
                }
            };
        }
        #endregion

        #region keystroke handler

        void k_KeystrokeEvent(KeystrokeEventArgs e)
        {
            CheckForSettingsMode(e);
            if (e.ShouldBeDisplayed)
            {
                if (settings.EnableWindowFade && !SettingsModeActivated)
                {
                    FadeIn();
                }

                if (!e.RequiresNewLine
                    && NumberOfDeletionsAllowed > 0
                    && LastHistoryLineIsText
                    && !LastHistoryLineRequiredNewLineAfterwards
                    && e.NoModifiers
                    && e.Key == System.Windows.Input.Key.Back)
                {
                    Log.e("BS", $"delete last char -> {labels[labels.Count - 1].text}");
                    Log.e("BS", "NumberOfDeletionsAllowed " + NumberOfDeletionsAllowed.ToString());
                    if (!removeLastChar())
                    {
                        // again
                        Log.e("BS", " failed");
                        NumberOfDeletionsAllowed = 0;
                        k_KeystrokeEvent(e);
                        return;
                    }
                }
                else if (e.RequiresNewLine
                    || !addingWouldFitInCurrentLine(e.ToString(false))
                    || !LastHistoryLineIsText
                    || LastHistoryLineRequiredNewLineAfterwards)
                {
                    addNextLine(e.ToString(false));
                    Log.e("BS", $"new line: {e.ToString(false)} -> {labels[labels.Count - 1].text}");
                    NumberOfDeletionsAllowed = e.Deletable ? 1 : 0;
                    Log.e("BS", "NumberOfDeletionsAllowed " + NumberOfDeletionsAllowed.ToString());
                }
                else
                {
                    addToLine(e.ToString(false));
                    Log.e("BS", $"add to line: {e.ToString(false)} -> {labels[labels.Count-1].text}");
                    if (e.Deletable)
                        NumberOfDeletionsAllowed += 1;
                    else
                        NumberOfDeletionsAllowed = 0;
                    Log.e("BS", "NumberOfDeletionsAllowed " + NumberOfDeletionsAllowed.ToString());
                }

                LastHistoryLineIsText = e.StrokeType == KeystrokeType.Text;
                LastHistoryLineRequiredNewLineAfterwards = e.RequiresNewLineAfterwards;

            }
        }


        #endregion

        #region Opacity Animation

        DoubleAnimation windowOpacityAnim;
        Storyboard windowOpacitySB;
        double minOpacityWhenVisible = 0.1;

        private void InitializeAnimations()
        {
            windowOpacityAnim = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromMilliseconds(200)),
            };
            windowOpacitySB = new Storyboard();
            windowOpacitySB.Children.Add(windowOpacityAnim);
            Storyboard.SetTarget(windowOpacityAnim, this);
            Storyboard.SetTargetProperty(windowOpacityAnim, new PropertyPath(Window.OpacityProperty));
        }

        private void FadeOut()
        {
            ToOpacity(0.0, true);
        }

        private void FadeIn()
        {
            // Opacity now via background color
            //ToOpacity(Math.Max(minOpacityWhenVisible, settings.Opacity), true);
            ToOpacity(1.0, true);
        }

        private void FullOpacity()
        {
            ToOpacity(1.0, false);
        }

        private void ToOpacity(double targetOpacity, bool fade)
        {
            if (Math.Abs(Opacity - targetOpacity) > 0.0001)
            {
                if (fade)
                {
                    // Fixme: Animation is restarting for every single keypress on fade-in
                    windowOpacitySB.Stop();
                    windowOpacityAnim.From = this.Opacity;
                    windowOpacityAnim.To = targetOpacity;
                    Log.e("OPACITY", $"Restart Anim, from {Opacity} to {targetOpacity}.");
                    windowOpacitySB.Begin(this);
                } else
                {
                    // https://docs.microsoft.com/de-de/dotnet/framework/wpf/graphics-multimedia/how-to-set-a-property-after-animating-it-with-a-storyboard
                    this.BeginAnimation(Window.OpacityProperty, null); // Break connection between storyboard and property
                    Opacity = targetOpacity;
                    Log.e("OPACITY", $"Remov anim, to {targetOpacity}. {Opacity}");
                }
            }
        }
        #endregion




        #region settingsChanged

        private void settingChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "BackgroundColor":
                    backgroundGrid.Background = new SolidColorBrush(UIHelper.ToMediaColor(settings.BackgroundColor));
                    break;
                case "Opacity":
                    //ToOpacity(Math.Max(minOpacityWhenVisible, settings.Opacity), false);
                    break;
                case "WindowLocation":
                    this.Left = settings.WindowLocation.X;
                    this.Top = settings.WindowLocation.Y;
                    Log.e("KD", String.Format("Apply X: {0}", settings.WindowLocation.X));
                    break;
                case "WindowSize":
                    this.Width = settings.WindowSize.Width;
                    this.Height = settings.WindowSize.Height;
                    break;
                case "PanelLocation":
                    this.innerPanel.Margin = new Thickness(settings.PanelLocation.X, settings.PanelLocation.Y, 0.0, 0.0);
                    break;
                case "PanelSize":
                    this.innerPanel.Width = settings.PanelSize.Width;
                    this.innerPanel.Height = settings.PanelSize.Height;
                    break;
                case "LabelFont":
                case "LabelColor":
                case "LabelTextAlignment":
                case "LineDistance":
                    UpdateLabelStyles();
                    break;
                case "LabelTextDirection":
                    if (labelStack.VerticalAlignment == VerticalAlignment.Top && settings.LabelTextDirection == TextDirection.Up)
                    {
                        labelStack.VerticalAlignment = VerticalAlignment.Bottom;
                        labelStack.Children.Clear();
                        for(int i = 0; i < labels.Count; i++)
                        {
                            labelStack.Children.Add(labels[i].label);
                        }
                        UpdateLabelStyles();
                    } else if (labelStack.VerticalAlignment == VerticalAlignment.Bottom && settings.LabelTextDirection == TextDirection.Down)
                    {
                        labelStack.VerticalAlignment = VerticalAlignment.Top;
                        labelStack.Children.Clear();
                        for (int i = labels.Count - 1; i >= 0; i--)
                        {
                            labelStack.Children.Add(labels[i].label);
                        }
                        UpdateLabelStyles();
                    }
                    break;
                case "HistoryLength":
                    TruncateHistory();
                    break;
                case "EnableWindowFade":
                    if (settings.EnableWindowFade && labels.Count == 0)
                    {
                        FadeOut();
                    }
                    else
                    {
                        FadeIn();
                    }
                    break;
                case "KeystrokeHistorySettingsModeShortcut":
                    SetSettingsModeShortcut(settings.KeystrokeHistorySettingsModeShortcut);
                    break;
                case "EnableSettingsMode":
                    if (settings.EnableSettingsMode)
                    {
                        ActivateSettingsMode();
                    } else
                    {
                        ActivateDisplayOnlyMode(false);
                    }
                    break;
            }
        }

        #endregion


        #region Settings Mode

        public static readonly String[] AvailableKeysForShortcut = {
            "none",
            "Ctrl",
            "Alt",
            "Shift",
            "LCtrl",
            "LAlt",
            "LShift",
            "RCtrl",
            "RAlt",
            "RShift",
            "LWin",
            "RWin",
        };
        public String[] SettingsModeShortcut;

        public bool SetSettingsModeShortcut(string shortcut)
        {
            if (ValidateShortcutSetting(shortcut))
            {
                SettingsModeShortcut = shortcut.Split('+');
                return true;
            }
            if (SettingsModeShortcut.Length == 0)
            {
                SettingsModeShortcut = settings.KeystrokeHistorySettingsModeShortcutDefault.Split('+');
            }
            return false;
        }

        public static bool ValidateShortcutSetting(string shortcut)
        {
            var allok = true;
            foreach (var key in shortcut.Split('+'))
            {
                if (!KeystrokeDisplay.AvailableKeysForShortcut.Contains(key))
                {
                    allok = false;
                }
            }
            return allok;
        }

        private void CheckForSettingsMode(KeystrokeEventArgs e)
        {
            var activated = true;
            foreach (var key in SettingsModeShortcut)
            {
                if (key == "none") activated = false;
                else if (key == "Ctrl" && !e.Ctrl) activated = false;
                else if (key == "Alt" && !e.Alt) activated = false;
                else if (key == "Shift" && !e.Shift) activated = false;
                else if (key == "LCtrl" && !e.LCtrl) activated = false;
                else if (key == "LAlt" && !e.LAlt) activated = false;
                else if (key == "LShift" && !e.LShift) activated = false;
                else if (key == "RCtrl" && !e.RCtrl) activated = false;
                else if (key == "RAlt" && !e.RAlt) activated = false;
                else if (key == "RShift" && !e.RShift) activated = false;
                else if (key == "LWin" && !e.LWin) activated = false;
                else if (key == "RWin" && !e.RWin) activated = false;
            }
            if (activated)
                ActivateSettingsMode();
            else
                ActivateDisplayOnlyMode(false);
        }

        bool SettingsModeActivated = false;

        void ActivateDisplayOnlyMode(bool force)
        {
            if (SettingsModeActivated || force)
            {
                FadeIn();

                NativeMethodsGWL.ClickThrough(this.windowHandle);
                NativeMethodsGWL.HideFromAltTab(this.windowHandle);

                InnerPanelIsDragging = false;

                buttonClose.Visibility = Visibility.Hidden;
                buttonResizeInnerPanel.Visibility = Visibility.Hidden;
                buttonResizeWindow.Visibility = Visibility.Hidden;
                buttonSettings.Visibility = Visibility.Hidden;
                innerPanel.Background = new SolidColorBrush(Color.FromArgb(0,0,0,0));
                backgroundGrid.Background = new SolidColorBrush(UIHelper.ToMediaColor(settings.BackgroundColor));

                SettingsModeActivated = false;

                settings.SaveAll();
            }
        }

        void ActivateSettingsMode()
        {
            if (!SettingsModeActivated)
            {
                NativeMethodsGWL.CatchClicks(this.windowHandle);

                FullOpacity();

                InnerPanelIsDragging = false;

                buttonClose.Visibility = Visibility.Visible;
                buttonResizeInnerPanel.Visibility = Visibility.Visible;
                buttonResizeWindow.Visibility = Visibility.Visible;
                buttonSettings.Visibility = Visibility.Visible;
                innerPanel.Background = OrigInnerPanelBackgroundColor;
                backgroundGrid.Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

                foreach (LabelData d in labels)
                {
                    d.label.Visibility = Visibility.Hidden;
                }

                SettingsModeActivated = true;
            }
        }



        #endregion

        #region Dragging of Window and innerPanel

        private void backgroundGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
            e.Handled = true;

        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            settings.WindowLocation = new Point(this.Left, this.Top);
        }

        private bool InnerPanelIsDragging = false;
        private Point InnerPanelDragStartCursorPosition;
        private Point InnerPanelDragStartPosition;

        private void innerPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            InnerPanelDragStartCursorPosition = e.GetPosition(this);
            InnerPanelIsDragging = true;
            InnerPanelDragStartPosition = new Point(innerPanel.Margin.Left, innerPanel.Margin.Top);
            e.Handled = true;
        }

        private void innerPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (InnerPanelIsDragging)
            {
                Point current = e.GetPosition(this);
                Point diff = new Point(
                    current.X - InnerPanelDragStartCursorPosition.X,
                    current.Y - InnerPanelDragStartCursorPosition.Y);
                innerPanel.Margin = new Thickness(InnerPanelDragStartPosition.X + diff.X,
                    InnerPanelDragStartPosition.Y + diff.Y,
                    0,
                    0);
            }
            e.Handled = true;
        }

        private void innerPanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            InnerPanelIsDragging = false;
            settings.PanelLocation = new Point(innerPanel.Margin.Left, innerPanel.Margin.Top);
        }


        #endregion

        #region Button Click  and Window Close Events

        private void buttonSettings_Click(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).showSettingsWindow();
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {

        }
        #endregion

        #region display and animate Label

        class LabelData
        {
            public Label label;
            public string text;
            public Storyboard storyboard;
            public DispatcherTimer historyTimeout;
        }

        List<LabelData> labels = new List<LabelData>(5);
        bool LastHistoryLineIsText = false;
        bool LastHistoryLineRequiredNewLineAfterwards = false;
        int NumberOfDeletionsAllowed = 0;

        void addToLine(string chars)
        {
            LabelData pack = labels[labels.Count - 1];
            if (pack.historyTimeout != null)
            {
                pack.historyTimeout.Stop();
                if (settings.EnableHistoryTimeout)
                {
                    pack.historyTimeout.Interval = TimeSpan.FromSeconds(settings.HistoryTimeout);
                    pack.historyTimeout.Start();
                }
            }
            pack.text = pack.text + chars;
            pack.label.Content = pack.text.Replace("_", "__");
            //T.Refresh();
        }

        private bool removeLastChar()
        {
            if (labels.Count == 0)
            {
                return false;
            }

            LabelData pack = labels[labels.Count - 1];
            var content = ((string)pack.label.Content);
            if (content.Length == 0)
                return false;
            pack.text = pack.text.Substring(0, pack.text.Length - 1);
            pack.label.Content = pack.text.Replace("_", "__");
            NumberOfDeletionsAllowed -= 1;
            return true;
        }

        void addNextLine(string chars)
        {
            Label next = new Label();
            next.Content = chars.Replace("_", "__"); ;
            ApplyLabelStyle(next);

            var pack = new LabelData
            {
                label = next,
                text = chars,
                storyboard = null,
                historyTimeout = null,
            };

            if (settings.LabelAnimation == PxKeystrokesWPF.Style.Slide)
            {
                Storyboard showLabelSB = new Storyboard();
                var fadeInAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 1.0,
                    Duration = new Duration(TimeSpan.FromMilliseconds(200)),
                };
                showLabelSB.Children.Add(fadeInAnimation);
                Storyboard.SetTarget(fadeInAnimation, next);
                Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(Label.OpacityProperty));

                Thickness targetMargin = next.Margin; // from ApplyLabelStyle
                if (settings.LabelTextDirection == TextDirection.Down)
                {
                    next.Margin = new Thickness(0, 0, 0, -next.Height);
                }
                else
                {
                    next.Margin = new Thickness(0, -next.Height, 0, 0);
                }

                var pushUpwardsAnimation = new ThicknessAnimation
                {
                    From = next.Margin,
                    To = targetMargin,
                    Duration = new Duration(TimeSpan.FromMilliseconds(200))
                };
                showLabelSB.Children.Add(pushUpwardsAnimation);
                Storyboard.SetTarget(pushUpwardsAnimation, next);
                Storyboard.SetTargetProperty(pushUpwardsAnimation, new PropertyPath(Label.MarginProperty));

                pack.storyboard = showLabelSB;
                pack.storyboard.Begin(pack.label);
            }

            if (settings.LabelTextDirection == TextDirection.Up)
            {
                labelStack.Children.Add(next);
            } else
            {
                labelStack.Children.Insert(0, next);
            }


            if (settings.EnableHistoryTimeout)
            {
                pack.historyTimeout = FireOnce(settings.HistoryTimeout, () =>
                {
                    fadeOutLabel(pack);
                });
            }
            labels.Add(pack);

            TruncateHistory();
        }

        void TruncateHistory()
        {
            while (labels.Count > settings.HistoryLength)
            {
                var toRemove = labels[0];
                Log.e("LABELREMOVAL", $"Truncate {toRemove.label.Content}. Currently in list: {labels.Count}");
                fadeOutLabel(toRemove);
                labels.Remove(toRemove);
            }
        }

        void fadeOutLabel(LabelData toRemove)
        {
            if (toRemove.historyTimeout != null)
            {
                toRemove.historyTimeout.Stop();
            }
            if (toRemove.storyboard != null)
            {
                toRemove.storyboard.Remove(toRemove.label);
            }

            if (settings.LabelAnimation == PxKeystrokesWPF.Style.Slide)
            {
                Storyboard hideLabelSB = new Storyboard();
                var fadeOutAnimation = new DoubleAnimation
                {
                    From = toRemove.label.Opacity,
                    To = 0,
                    Duration = new Duration(TimeSpan.FromMilliseconds(200)),
                };
                hideLabelSB.Children.Add(fadeOutAnimation);
                Storyboard.SetTarget(fadeOutAnimation, toRemove.label);
                Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(Label.OpacityProperty));
                fadeOutAnimation.Completed += (object sender, EventArgs e) =>
                {
                    Log.e("LABELREMOVAL", $"{toRemove.label.Content}: Fade out completed");
                    hideLabelSB.Remove(toRemove.label);
                    labelStack.Children.Remove(toRemove.label);

                    if (settings.EnableWindowFade && labels.Count == 0)
                    {
                        Log.e("LABELREMOVAL", $"{toRemove.label.Content}: Fade out completed -> no more labels -> Window wade out");
                        FadeOut();
                    }
                };
                hideLabelSB.Begin(toRemove.label);
                labels.Remove(toRemove);
            } else
            {
                labelStack.Children.Remove(toRemove.label);
                labels.Remove(toRemove);
                if (settings.EnableWindowFade && labels.Count == 0)
                {
                    Log.e("LABELREMOVAL", $"{toRemove.label.Content}: Fade out completed -> no more labels -> Window wade out");
                    FadeOut();
                }
            }
        }


        /// <summary>
        /// Fires an action once after "seconds"
        /// </summary>
        public static DispatcherTimer FireOnce(double timeout, Action onElapsed)
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(timeout) };

            var handler = new EventHandler((s, args) =>
            {
                if (timer.IsEnabled)
                {
                    onElapsed();
                }
                timer.Stop();
            });

            timer.Tick += handler;
            timer.Start();
            return timer;
        }

        bool addingWouldFitInCurrentLine(string s)
        {
            if (labels.Count == 0)
                return false;

            var label = labels[labels.Count - 1].label;
            var text = (String) label.Content;
            var formattedText = new FormattedText(
                text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(label.FontFamily, label.FontStyle, label.FontWeight, label.FontStretch),
                label.FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                1);

            return formattedText.Width < label.ActualWidth - 20;
        }

        void ApplyLabelStyle(Label label)
        {
            label.Height = 120;
            label.BeginAnimation(Label.MarginProperty, null);
            if (settings.LabelTextDirection == TextDirection.Down)
            {
                label.Margin = new Thickness(0, 0, 0, -label.Height + settings.LineDistance);
                label.VerticalContentAlignment = VerticalAlignment.Top;
            }
            else
            {
                label.Margin = new Thickness(0, -label.Height + settings.LineDistance, 0, 0);
                label.VerticalContentAlignment = VerticalAlignment.Bottom;
            }

            label.BeginAnimation(Label.OpacityProperty, null);
            label.Opacity = 1.0;
            label.Foreground = new SolidColorBrush(UIHelper.ToMediaColor(settings.LabelColor));
            label.FontSize = settings.LabelFont.Size;
            label.FontFamily = settings.LabelFont.Family;
            label.FontStretch = settings.LabelFont.Stretch;
            label.FontStyle = settings.LabelFont.Style;
            label.FontWeight = settings.LabelFont.Weight;

            if (settings.LabelTextAlignment == TextAlignment.Left)
            {
                label.HorizontalContentAlignment = HorizontalAlignment.Left;
            }
            else if (settings.LabelTextAlignment == TextAlignment.Center)
            {
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
            }
            else
            {
                label.HorizontalContentAlignment = HorizontalAlignment.Right;
            }
        }

        void UpdateLabelStyles()
        {
            foreach (var pack in labels)
            {
                ApplyLabelStyle(pack.label);
            }
        }





        #endregion


    }
}
