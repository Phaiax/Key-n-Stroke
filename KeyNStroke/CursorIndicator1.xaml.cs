using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using static KeyNStroke.NativeMethodsMouse;

namespace KeyNStroke
{
    /// <summary>
    /// Interaktionslogik für CursorIndicator1.xaml
    /// </summary>
    public partial class CursorIndicator1 : Window
    {
        IMouseRawEventProvider m;
        SettingsStore s;
        IntPtr windowHandle;
        bool isHidden;

        public CursorIndicator1(IMouseRawEventProvider m, SettingsStore s)
        {
            InitializeComponent();

            this.m = m;
            this.s = s;

            s.PropertyChanged += settingChanged;
            this.isHidden = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            m.MouseEvent += m_MouseEvent;
            m.CursorEvent += m_CursorEvent;
            windowHandle = new WindowInteropHelper(this).Handle;
            SetFormStyles();
        }

        void m_MouseEvent(MouseRawEventArgs raw_e)
        {
            if (raw_e.Action == MouseAction.Move)
                UpdatePosition(raw_e.Position);
        }

        void m_CursorEvent(bool visible)
        {
            if (visible)
            {
                if (isHidden)
                {
                    this.Show();
                    isHidden = false;
                }
            }
            else
            {
                if (!isHidden)
                {
                    this.Hide();
                    isHidden = true;
                }
            }
        }

        void SetFormStyles()
        {
            //this.Opacity = s.CursorIndicatorOpacity;
            Log.e("CI", $"WindowHandle={windowHandle}");
            NativeMethodsGWL.ClickThrough(windowHandle);
            NativeMethodsGWL.HideFromAltTab(windowHandle);

            UpdateSize();
            UpdatePosition(NativeMethodsMouse.CursorPosition);
        }

        void UpdateSize()
        {
            this.Width = s.CursorIndicatorSize;
            this.Height = s.CursorIndicatorSize;
        }

        void UpdatePosition(NativeMethodsMouse.POINT cursorPosition)
        {
            IntPtr monitor = NativeMethodsWindow.MonitorFromPoint(cursorPosition, NativeMethodsWindow.MonitorOptions.MONITOR_DEFAULTTONEAREST);
            uint adpiX = 0, adpiY = 0;
            NativeMethodsWindow.GetDpiForMonitor(monitor, NativeMethodsWindow.DpiType.MDT_EFFECTIVE_DPI, ref adpiX, ref adpiY);
            Log.e("CI", $"apix={adpiX} adpiy={adpiY} aw={ActualWidth} ah={ActualHeight} cx={cursorPosition.X} cy={cursorPosition.Y}");
            NativeMethodsWindow.SetWindowPosition(windowHandle, 
                    (int)(cursorPosition.X - (this.ActualWidth / 2) * (double)adpiX / 96.0),
                    (int)(cursorPosition.Y - (this.ActualHeight / 2) * (double)adpiY / 96.0));
        }

        private void settingChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action) (() =>
            {
                switch (e.PropertyName)
                {
                    case "EnableCursorIndicator":
                        break;
                    case "CursorIndicatorOpacity":
                        UpdateColor();
                        break;
                    case "CursorIndicatorSize":
                        UpdateSize();
                        break;
                    case "CursorIndicatorColor":
                        UpdateColor();
                        break;
                }
            }));
        }

        private void UpdateColor()
        {
            var c = UIHelper.ToMediaColor(s.CursorIndicatorColor);
            circle.Fill = new SolidColorBrush(Color.FromArgb((byte)(255 * (1 - s.CursorIndicatorOpacity)), c.R, c.G, c.B));
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            if (m != null)
            {
                m.MouseEvent -= m_MouseEvent;
                m.CursorEvent -= m_CursorEvent;
            }
            if (s != null)
                s.PropertyChanged -= settingChanged;
            m = null;
            s = null;
        }
    }
}
