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
        IKeystrokeEventProvider k;
        SettingsStore s;
        IntPtr windowHandle;
        bool isDown;
        POINT startCursorPosition = new POINT(0, 0);
        POINT endCursorPosition = new POINT(0, 0);
        bool nextClickDraws = false;
        bool nextClickHides = false;

        public AnnotateLine(IMouseRawEventProvider m, IKeystrokeEventProvider k, SettingsStore s)
        {
            InitializeComponent();

            this.m = m;
            this.s = s;
            this.k = k;

            s.PropertyChanged += settingChanged;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            m.MouseEvent += m_MouseEvent;
            this.k.KeystrokeEvent += m_KeystrokeEvent;
            windowHandle = new WindowInteropHelper(this).Handle;
            SetFormStyles();
        }

        #region Shortcut

        public string AnnotateLineShortcut;

        void m_KeystrokeEvent(KeystrokeEventArgs e)
        {
            string pressed = e.ShortcutIdentifier();
            CheckForTrigger(pressed);
        }

        private void CheckForTrigger(string pressed)
        {
            if (AnnotateLineShortcut != null && pressed == AnnotateLineShortcut)
            {
                nextClickDraws = true;
            }
        }

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

        #endregion

        private void m_MouseEvent(MouseRawEventArgs raw_e)
        {
            if (!isDown && nextClickHides && raw_e.Action == MouseAction.Down)
            {
                raw_e.preventDefault = true;
                nextClickHides = false;
                this.Hide();
            }

            if (isDown && raw_e.Action == MouseAction.Move)
            {
                endCursorPosition = raw_e.Position;
                UpdatePositionAndSize();
            }
            else if (!isDown && raw_e.Action == MouseAction.Down && nextClickDraws)
            {
                isDown = true;
                raw_e.preventDefault = true;
                nextClickDraws = false;
                startCursorPosition = raw_e.Position;
                endCursorPosition = raw_e.Position;
            }
            else if (isDown && raw_e.Action == MouseAction.Up)
            {
                isDown = false;
                nextClickHides = true;
            }
        }

        private void settingChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                switch (e.PropertyName)
                {
                    case "AnnotateLineShortcutTrigger":
                        nextClickDraws = true;
                        break;
                    case "AnnotateLineColor":
                        UpdateColor();
                        break;
                    case "AnnotateLineShortcut":
                        SetAnnotateLineShortcut(s.AnnotateLineShortcut);
                        break;
                }
            }));
        }

        void SetFormStyles()
        {
            Log.e("CI", $"WindowHandle={windowHandle}");
            NativeMethodsGWL.ClickThrough(windowHandle);
            NativeMethodsGWL.HideFromAltTab(windowHandle);

            UpdatePositionAndSize();
        }

        void UpdateColor()
        {
            line.Fill = new SolidColorBrush(UIHelper.ToMediaColor(s.AnnotateLineColor));
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
            if (k != null)
            {
                k.KeystrokeEvent -= m_KeystrokeEvent;
            }
            if (s != null)
            {
                s.PropertyChanged -= settingChanged;
            }
            m = null;
            s = null;
            k = null;
        }
    }
}
