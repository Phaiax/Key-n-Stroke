using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public KeystrokeDisplay(IKeystrokeEventProvider k, SettingsStore s)
        {
            InitializeComponent();
            InitializeAnimations();

            windowHandle = new WindowInteropHelper(this).Handle;

            this.k = k;
            this.k.KeystrokeEvent += k_KeystrokeEvent;

            this.settings = s;
            this.settings.PropertyChanged += settingChanged;

            this.settings.OnSettingChangedAll();

            //addWelcomeInfo();

            ActivateDisplayOnlyMode(true);

            if (settings.EnableWindowFade)
            {
                FadeOut();
            }

            settings.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                if (settings.EnableWindowFade/*TODO &&  tweenLabels.Count == 0 */)
                {
                    FadeOut();
                }
                else
                {
                    FadeIn();
                }
            };
        }

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
                /*
                if (!e.RequiresNewLine
                    && NumberOfDeletionsAllowed > 0
                    && LastHistoryLineIsText
                    && !LastHistoryLineRequiredNewLineAfterwards
                    && e.NoModifiers
                    && e.Key == System.Windows.Input.Key.Back)
                {
                    Log.e("BS", "delete last char");
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
                    NumberOfDeletionsAllowed = e.Deletable ? 1 : 0;
                    Log.e("BS", "NumberOfDeletionsAllowed " + NumberOfDeletionsAllowed.ToString());
                }
                else
                {
                    Log.e("BS", "add to line ");
                    addToLine(e.ToString(false));
                    if (e.Deletable)
                        NumberOfDeletionsAllowed += 1;
                    else
                        NumberOfDeletionsAllowed = 0;
                    Log.e("BS", "NumberOfDeletionsAllowed " + NumberOfDeletionsAllowed.ToString());
                }

                LastHistoryLineIsText = e.StrokeType == KeystrokeType.Text;
                LastHistoryLineRequiredNewLineAfterwards = e.RequiresNewLineAfterwards;
                */
            }
        }


        #endregion

        #region Animations

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
            ToOpacity(Math.Max(minOpacityWhenVisible, settings.Opacity / 100.0f), true);
        }

        private void FullOpacity()
        {
            ToOpacity(1.0, false);
        }

        private void ToOpacity(double targetOpacity, bool fade)
        {
            if (Opacity != targetOpacity)
            {
                if (fade)
                {
                    windowOpacitySB.Stop();
                    windowOpacityAnim.From = this.Opacity;
                    windowOpacityAnim.To = targetOpacity;
                    windowOpacitySB.Begin(this);
                } else
                {
                    // https://docs.microsoft.com/de-de/dotnet/framework/wpf/graphics-multimedia/how-to-set-a-property-after-animating-it-with-a-storyboard
                    this.BeginAnimation(Window.OpacityProperty, null); // Break connection between storyboard and property
                    Opacity = targetOpacity;
                }
            }
        }
        #endregion


        private void Window_Closing(object sender, CancelEventArgs e)
        {

        }

        #region settingsChanged, Dialog

        private void settingChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "BackgroundColor":
                    //this.BackColor = settings.BackgroundColor;
                    break;
                case "Opacity":
                    ToOpacity(Math.Max(minOpacityWhenVisible, settings.Opacity / 100.0f), false);
                    break;
                case "WindowLocation":
                    //this.Location = settings.WindowLocation;
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
            }
        }


        void ShowSettingsDialog()
        {
            Settings1 settings1 = new Settings1(settings);
            settings1.ShowDialog();
        }

        #endregion


        #region Settings Mode

        private void CheckForSettingsMode(KeystrokeEventArgs e)
        {
            if (e.Ctrl && e.Shift && e.Alt)
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

                this.buttonClose.Visibility = Visibility.Hidden;
                this.buttonResizeInnerPanel.Visibility = Visibility.Hidden;
                this.buttonResizeWindow.Visibility = Visibility.Hidden;
                this.buttonSettings.Visibility = Visibility.Hidden;
                //this.panel_textposhelper.Visibility = Visibility.Hidden; TODO

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

                this.buttonClose.Visibility = Visibility.Visible;
                this.buttonResizeInnerPanel.Visibility = Visibility.Visible;
                this.buttonResizeWindow.Visibility = Visibility.Visible;
                this.buttonSettings.Visibility = Visibility.Visible;
                //this.panel_textposhelper.Visibility = Visibility.Visible; TODO

                /* TODO foreach (TweenLabel T in tweenLabels)
                {
                    T.Visibility = Visibility.Hidden;
                }*/

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
        }


        #endregion

        #region Button Click Events

        private void buttonSettings_Click(object sender, RoutedEventArgs e)
        {
            ShowSettingsDialog();
        }

        #endregion
    }
}
