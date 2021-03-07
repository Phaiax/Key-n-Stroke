using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PxKeystrokesWPF;

namespace PxKeystrokesWPF
{
    public partial class ButtonIndicator1 : Form
    {
        IMouseRawEventProvider m;
        SettingsStore s;
        ImageResources.ComposeOptions c;
        Size bitmapsize;

        public ButtonIndicator1(IMouseRawEventProvider m, SettingsStore s)
        {
            InitializeComponent();

            this.m = m;
            this.s = s;
            FormClosed += ButtonIndicator_FormClosed;

            HideMouseIfNoButtonPressed();

            c.dpi = 96;
            UpdatePosition(NativeMethodsMouse.CursorPosition);

            NativeMethodsWindow.SetWindowTopMost(this.Handle);
            SetFormStyles();

            m.MouseEvent += m_MouseEvent;
            s.PropertyChanged += settingChanged;
            DoubleClickIconTimer.Tick += leftDoubleClickIconTimeout_Tick;
            DoubleClickIconTimer.Interval = 750;

            WheelIconTimer.Interval = 750;
            WheelIconTimer.Tick += WheelIconTimer_Tick;


            Redraw();
        }

        private void Redraw()
        {
            Bitmap scaledAndComposedBitmap = ImageResources.Compose(c);
            this.bitmapsize = scaledAndComposedBitmap.Size;
            IntPtr handle;
            try
            {
                handle = this.Handle; // May be already disposed if leftDoubleClickIconTimeout triggers
            }
            catch (System.ObjectDisposedException)
            {
                throw;
            }
            NativeMethodsDC.SetBitmapForWindow(handle,
                                               this.Location,
                                               scaledAndComposedBitmap,
                                               1.0f);   // opacity
            NativeMethodsWindow.PrintDpiAwarenessInfo();
        }

        private void ButtonIndicator_Load(object sender, EventArgs e)
        {
            Log.e("SOME", "ButtonIndicator => Load");

            RecalcOffset();
            UpdateSize();
        }

        MouseRawEventArgs lastDblClk;

        void m_MouseEvent(MouseRawEventArgs raw_e)
        {
            switch (raw_e.Action)
            {
                case MouseAction.Up:
                    HideButton(raw_e);
                    break;
                case MouseAction.Down:
                    ShowButton(raw_e.Button);
                    break;
                case MouseAction.DblClk:
                    lastDblClk = raw_e;
                    IndicateDoubleClick(raw_e.Button);
                    break;
                case MouseAction.Move:
                    UpdatePosition(raw_e.Position);
                    break;
                case MouseAction.Wheel:
                    IndicateWheel(raw_e);
                    break;
                default:
                    break;
            }
        }

        Timer WheelIconTimer = new Timer();

        private void IndicateWheel(MouseRawEventArgs raw_e)
        {
            c.addBMouse = true;
            Log.e("WHEEL", "Display " + raw_e.wheelDelta.ToString());
            WheelIconTimer.Stop();
            WheelIconTimer.Start();
            if (raw_e.wheelDelta < 0)
            {
                c.addBWheelDown = true;
                c.addBWheelUp = false;
            }
            else if (raw_e.wheelDelta > 0)
            {
                c.addBWheelUp = true;
                c.addBWheelDown = false;
            }
            Redraw();
        }

        void WheelIconTimer_Tick(object sender, EventArgs e)
        {
            WheelIconTimer.Stop();
            Log.e("WHEEL", "Hide ");
            c.addBWheelDown = false;
            c.addBWheelUp = false;
            HideMouseIfNoButtonPressed();
            Redraw();
        }

        private void IndicateDoubleClick(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.LButton:
                    c.addBMouse = true;
                    c.addBLeft = false;
                    c.addBLeftDouble = true;
                    doubleClickReleased = false;
                    Redraw();
                    break;
                case MouseButton.RButton:
                    break;
                default:
                    break;
            }
        }

        void leftDoubleClickIconTimeout_Tick(object sender, EventArgs e)
        {
            ((Timer)sender).Stop();
            c.addBLeftDouble = false;
            HideMouseIfNoButtonPressed();
            Redraw();
        }

        private void ShowButton(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.LButton:
                    c.addBMouse = true;
                    c.addBLeft = true;
                    c.addBLeftDouble = false;
                    //UpdatePosition();
                    Redraw();
                    break;
                case MouseButton.RButton:
                    c.addBMouse = true;
                    c.addBRight = true;
                    //UpdatePosition();
                    Redraw();
                    break;
                case MouseButton.MButton:
                    c.addBMouse = true;
                    c.addBMiddle = true;
                    //UpdatePosition();
                    Redraw();
                    break;
                case MouseButton.XButton:
                    break;
                case MouseButton.None:
                    break;
                default:
                    break;
            }
        }

        Timer DoubleClickIconTimer = new Timer();
        bool doubleClickReleased = true;

        private void HideButton(MouseRawEventArgs raw_e)
        {
            switch (raw_e.Button)
            {
                case MouseButton.LButton:
                    c.addBLeft = false;
                    doubleClickReleased = true;
                    if (c.addBLeftDouble)
                    {
                        if (raw_e.Msllhookstruct.time - lastDblClk.Msllhookstruct.time > DoubleClickIconTimer.Interval)
                        {
                            c.addBLeftDouble = false;
                        }
                        else
                        {
                            DoubleClickIconTimer.Stop();
                            DoubleClickIconTimer.Start();
                        }
                    }
                    break;
                case MouseButton.RButton:
                    c.addBRight = false;
                    break;
                case MouseButton.MButton:
                    c.addBMiddle = false;
                    break;
                case MouseButton.XButton:
                    break;
                case MouseButton.None:
                    break;
                default:
                    break;
            }
            HideMouseIfNoButtonPressed();
            Redraw();
        }

        void HideMouseIfNoButtonPressed()
        {
            if (   !c.addBLeft
                && !c.addBRight
                && !c.addBMiddle
                && !c.addBLeftDouble
                && !c.addBRightDouble
                && !c.addBWheelDown
                && !c.addBWheelUp)
            {
                c.addBMouse = false;
            }
        }

        void ButtonIndicator_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m != null)
                m.MouseEvent -= m_MouseEvent;
            if (s != null)
                s.PropertyChanged -= settingChanged;
            m = null;
            s = null;
        }

        void SetFormStyles()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            //this.Opacity = 0.8;
            NativeMethodsGWL.ClickThrough(this.Handle); // This also makes the window style WS_EX_LAYERED
            NativeMethodsGWL.HideFromAltTab(this.Handle);

            UpdateSize();
            UpdatePosition(NativeMethodsMouse.CursorPosition);
        }



        void UpdateSize()
        {
            ImageResources.ApplyScalingFactor(s.ButtonIndicatorScaling);
            Log.e("BI", "size change");
            Redraw();
        }

        Size offset = new Size(0, 0);

        void RecalcOffset()
        {
            offset.Width = (int)(s.ButtonIndicatorPositionDistance * Math.Sin(s.ButtonIndicatorPositionAngle / 10.0f));
            offset.Height = (int)(s.ButtonIndicatorPositionDistance * Math.Cos(s.ButtonIndicatorPositionAngle / 10.0f));
        }

        void UpdatePosition(NativeMethodsMouse.POINT cursorPosition)
        {
            if (OnlyDblClkIconVisible() && doubleClickReleased)
                return;
            IntPtr monitor = NativeMethodsWindow.MonitorFromPoint(cursorPosition, NativeMethodsWindow.MonitorOptions.MONITOR_DEFAULTTONEAREST);
            uint adpiX = 0, adpiY = 0;
            NativeMethodsWindow.GetDpiForMonitor(monitor, NativeMethodsWindow.DpiType.MDT_EFFECTIVE_DPI, ref adpiX, ref adpiY);
            c.dpi = adpiX;
            NativeMethodsWindow.SetWindowPosition(this.Handle, cursorPosition.X - this.bitmapsize.Width/2 + offset.Width, cursorPosition.Y - this.bitmapsize.Height/2 + offset.Height);

        }

        private bool OnlyDblClkIconVisible()
        {
            return !c.addBLeft
                && !c.addBRight
                && !c.addBMiddle
                && !c.addBWheelUp
                && !c.addBWheelDown
                && (c.addBLeftDouble
               || c.addBRightDouble);
        }

        private void settingChanged(object sender, PropertyChangedEventArgs e)
        {
            Log.e("SOME", "ButtonIndicator => settingChanged");
            switch (e.PropertyName)
            {
                case "ButtonIndicator":
                    break;
                case "ButtonIndicatorPositionAngle":
                    RecalcOffset();
                    UpdatePosition(NativeMethodsMouse.CursorPosition);
                    break;
                case "ButtonIndicatorPositionDistance":
                    RecalcOffset();
                    UpdatePosition(NativeMethodsMouse.CursorPosition);
                    break;
                case "ButtonIndicatorSize":
                    UpdateSize();
                    UpdatePosition(NativeMethodsMouse.CursorPosition);
                    break;
            }
        }


    }
}
