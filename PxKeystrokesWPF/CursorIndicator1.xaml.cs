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
        System.Drawing.Point cursorPosition;

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
            cursorPosition = raw_e.Position;
            if (raw_e.Action == MouseAction.Move)
                UpdatePosition();
        }


        void SetFormStyles()
        {
            //this.Opacity = s.CursorIndicatorOpacity;
            Log.e("CI", $"WindowHandle={windowHandle}");
            NativeMethodsGWL.ClickThrough(windowHandle);
            NativeMethodsGWL.HideFromAltTab(windowHandle);

            UpdateSize();
            UpdatePosition();
        }

        void UpdateSize()
        {
            this.Width = s.CursorIndicatorSize;
            this.Height = s.CursorIndicatorSize;
        }

        void UpdatePosition()
        {
            //var size = this.PointToScreen(new Point(this.ActualWidth, this.ActualHeight));
            //Log.e("CI", $"Size: {size.X} {size.Y}");
            NativeMethodsMouse.POINT cursorPosition = new NativeMethodsMouse.POINT(0, 0);
            NativeMethodsMouse.GetCursorPos(ref cursorPosition);
            IntPtr monitor = NativeMethodsWindow.MonitorFromPoint(cursorPosition, NativeMethodsWindow.MonitorOptions.MONITOR_DEFAULTTONEAREST);
            uint adpiX = 0, adpiY = 0;
            NativeMethodsWindow.GetDpiForMonitor(monitor, NativeMethodsWindow.DpiType.MDT_ANGULAR_DPI, ref adpiX, ref adpiY);
            NativeMethodsWindow.SetWindowPosition(windowHandle, 
                    (int)(cursorPosition.X - (this.ActualWidth / 2) * (double)adpiX / 96.0),
                    (int)(cursorPosition.Y - (this.ActualHeight / 2) * (double)adpiY / 96.0));
        }

        private void settingChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "EnableCursorIndicator":
                    break;
                case "CursorIndicatorOpacity":
                    //this.Opacity = s.CursorIndicatorOpacity;
                    break;
                case "CursorIndicatorSize":
                    UpdateSize();
                    break;
                case "CursorIndicatorColor":
                    var c = UIHelper.ToMediaColor(s.CursorIndicatorColor);
                    circle.Fill = new SolidColorBrush(Color.FromArgb((byte) (255 * (1 - s.CursorIndicatorOpacity)), c.R, c.G, c.B);
                    break;
            }
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
