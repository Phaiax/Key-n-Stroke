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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PxKeystrokesWPF
{
    /// <summary>
    /// Interaktionslogik für CursorIndicator1.xaml
    /// </summary>
    public partial class CursorIndicator1 : Window
    {
        IMouseRawEventProvider m;
        SettingsStore s;
        IntPtr windowHandle;

        public CursorIndicator1(IMouseRawEventProvider m, SettingsStore s)
        {
            InitializeComponent();

            this.m = m;
            this.s = s;

            s.PropertyChanged += settingChanged;

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            m.MouseEvent += m_MouseEvent;
            windowHandle = new WindowInteropHelper(this).Handle;
            SetFormStyles();
        }

        void m_MouseEvent(MouseRawEventArgs raw_e)
        {
            if (raw_e.Action == MouseAction.Move)
                UpdatePosition(raw_e.Position);
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
                m.MouseEvent -= m_MouseEvent;
            if (s != null)
                s.PropertyChanged -= settingChanged;
            m = null;
            s = null;
        }
    }
}
