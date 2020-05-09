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
        Storyboard windowOpacity;

        public KeystrokeDisplay(IKeystrokeEventProvider k, SettingsStore s)
        {
            InitializeComponent();
            InitializeAnimations();

            this.k = k;
            //this.k.KeystrokeEvent += k_KeystrokeEvent;

            this.settings = s;
            this.settings.PropertyChanged += settingChanged;

            this.settings.OnSettingChangedAll();
        }

        private void InitializeAnimations()
        {
            var anim = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromMilliseconds(800)),
                RepeatBehavior = RepeatBehavior.Forever,
                AutoReverse = true
            };
            windowOpacity = new Storyboard();
            windowOpacity.Children.Add(anim);
            Storyboard.SetTarget(anim, this);
            Storyboard.SetTargetProperty(anim, new PropertyPath(Window.OpacityProperty));
        }

        private void settingChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "BackgroundColor":
                    //this.BackColor = settings.BackgroundColor;
                    break;
                case "Opacity":
                    //this.Opacity = settings.Opacity;
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

        private void Window_Closing(object sender, CancelEventArgs e)
        {

        }

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



    }
}
