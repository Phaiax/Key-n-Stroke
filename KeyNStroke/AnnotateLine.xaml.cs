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
using static KeyNStroke.NativeMethodsMouse;

namespace KeyNStroke
{
    /// <summary>
    /// Interaktionslogik für AnnotateLine.xaml
    /// </summary>
    public partial class AnnotateLine : Window
    {

        IMouseRawEventProvider m;
        SettingsStore s;
        IntPtr windowHandle;
        bool isHidden;
        bool isDown;
        POINT startCursorPosition = new POINT(0, 0);
        POINT endCursorPosition = new POINT(0, 0);

        public AnnotateLine(IMouseRawEventProvider m, SettingsStore s)
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
            windowHandle = new WindowInteropHelper(this).Handle;
            SetFormStyles();
        }

        private void m_MouseEvent(MouseRawEventArgs raw_e)
        {
            if (raw_e.Action == MouseAction.Move)
            {
                if (isDown)
                {
                    endCursorPosition = raw_e.Position;
                    UpdatePositionAndSize();
                }
            }
            else if (raw_e.Action == MouseAction.Down)
            {
                isDown = true;
                startCursorPosition = raw_e.Position;
                endCursorPosition = raw_e.Position;
            }
            else if (raw_e.Action == MouseAction.Up)
            {
                isDown = false;
            }
        }
        private void settingChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                /*witch (e.PropertyName)
                {
                }*/
            }));
        }

        void SetFormStyles()
        {
            Log.e("CI", $"WindowHandle={windowHandle}");
            NativeMethodsGWL.ClickThrough(windowHandle);
            NativeMethodsGWL.HideFromAltTab(windowHandle);

            UpdatePositionAndSize();
        }


        void UpdatePositionAndSize()
        {
            if (isDown)
            {
                this.Show();
                Int32 xmin = Math.Min(startCursorPosition.X, endCursorPosition.X);
                Int32 ymin = Math.Min(startCursorPosition.Y, endCursorPosition.Y);
                Int32 h = Math.Abs(startCursorPosition.Y - endCursorPosition.Y);
                Int32 w = Math.Abs(startCursorPosition.X - endCursorPosition.X);
                bool horizontal = w >= h;

                IntPtr monitor = NativeMethodsWindow.MonitorFromPoint(startCursorPosition, NativeMethodsWindow.MonitorOptions.MONITOR_DEFAULTTONEAREST);
                uint adpiX = 0, adpiY = 0;
                NativeMethodsWindow.GetDpiForMonitor(monitor, NativeMethodsWindow.DpiType.MDT_EFFECTIVE_DPI, ref adpiX, ref adpiY);
                Log.e("AL", $"apix={adpiX} adpiy={adpiY} aw={ActualWidth} ah={ActualHeight} cx={startCursorPosition.X} cy={startCursorPosition.Y}");


                if (horizontal)
                {
                    this.Width = w / (double)adpiX * 96.0;
                    this.Height = 4;
                    NativeMethodsWindow.SetWindowPosition(windowHandle, (int)xmin, (int)startCursorPosition.Y - 2);
                }
                else
                {
                    this.Width = 4;
                    this.Height = h / (double)adpiY * 96.0;
                    NativeMethodsWindow.SetWindowPosition(windowHandle, (int)startCursorPosition.X - 2, ymin);
                }

            }
            else
            {
                this.Hide();
            }
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (m != null)
            {
                m.MouseEvent -= m_MouseEvent;
            }
            if (s != null)
                s.PropertyChanged -= settingChanged;
            m = null;
            s = null;
        }
    }
}
