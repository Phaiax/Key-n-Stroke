using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PxKeystrokesUi
{
    public partial class ButtonIndicator : Form
    {
        IMouseRawEventProvider m;
        SettingsStore s;

        public ButtonIndicator(IMouseRawEventProvider m, SettingsStore s)
        {
            InitializeComponent();

            this.m = m;
            this.s = s;
            FormClosed += CursorIndicator_FormClosed;

            NativeMethodsSWP.SetWindowTopMost(this.Handle);
            SetFormStyles();

            m.MouseEvent += m_MouseEvent;
            s.settingChanged += settingChanged;
            DoubleClickIconTimer.Tick += leftDoubleClickIconTimeout_Tick;
            DoubleClickIconTimer.Interval = 750;

            BackColor = Color.Lavender;
            TransparencyKey = Color.Lavender;
        }

        private void ButtonIndicator_Load(object sender, EventArgs e)
        {
            panel_mouse.BackgroundImage = ImageResources.BMouse;
            pb_left.Image = ImageResources.BLeft;
            pb_right.Image = ImageResources.BRight;
            pb_middle.Image = ImageResources.BMiddle;
            pb_left_double.Image = ImageResources.BLeftDouble;
            pb_right_double.Image = ImageResources.BRightDouble;
            pb_wheel.Image = ImageResources.BWheel;

            RecalcOffset();
            UpdateSize();
        }

        Point cursorPosition;
        MouseRawEventArgs lastDblClk;

        void m_MouseEvent(MouseRawEventArgs raw_e)
        {
            cursorPosition = raw_e.Position;
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
                    UpdatePosition();
                    break;
                case MouseAction.Wheel:
                    break;
                default:
                    break;
            }
        }


        private void IndicateDoubleClick(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.LButton:
                    panel_mouse.Visible = true;
                    pb_left.Visible = false;
                    pb_left_double.Visible = true;                
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
            pb_left_double.Visible = false;
            HideMouseIfNoButtonPressed();
        }

        private void ShowButton(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.LButton:
                    panel_mouse.Visible = true;
                    pb_left.Visible = true;
                    pb_left_double.Visible = false;
                    UpdatePosition();
                    break;
                case MouseButton.RButton:
                    panel_mouse.Visible = true;
                    pb_right.Visible = true;
                    UpdatePosition();
                    break;
                case MouseButton.MButton:
                    panel_mouse.Visible = true;
                    pb_middle.Visible = true;
                    UpdatePosition();
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

        private void HideButton(MouseRawEventArgs raw_e)
        {
            switch (raw_e.Button)
            {
                case MouseButton.LButton:
                    pb_left.Visible = false;
                    if (pb_left_double.Visible)
                    {
                        if( raw_e.Msllhookstruct.time - lastDblClk.Msllhookstruct.time > DoubleClickIconTimer.Interval)
                        {
                            pb_left_double.Visible = false;
                        }
                        else
                        {
                            DoubleClickIconTimer.Stop();
                            DoubleClickIconTimer.Start();
                        }
                    }
                    break;
                case MouseButton.RButton:
                    pb_right.Visible = false;
                    break;
                case MouseButton.MButton:
                    pb_middle.Visible = false;
                    break;
                case MouseButton.XButton:
                    break;
                case MouseButton.None:
                    break;
                default:
                    break;
            }
            HideMouseIfNoButtonPressed();
        }

        void HideMouseIfNoButtonPressed()
        {
            if( !pb_left.Visible 
                && !pb_right.Visible 
                && !pb_middle.Visible 
                && !pb_left_double.Visible 
                && !pb_right_double.Visible 
                && !pb_wheel.Visible)
            {
                panel_mouse.Visible = false;
            }
        }

        void CursorIndicator_FormClosed(object sender, FormClosedEventArgs e)
        {
            m.MouseEvent -= m_MouseEvent;
            s.settingChanged -= settingChanged;
            m = null;
            s = null;
        }

        void SetFormStyles()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Opacity = 0.8;
            NativeMethodsGWL.ClickThrough(this.Handle);

            UpdateSize();
            UpdatePosition();
        }

        

        void UpdateSize()
        {
            float sizefactor = s.ButtonIndicatorSize;
            Size picSize = new Size( (int)(ImageResources.BMouse.Width * sizefactor),
                                     (int)(ImageResources.BMouse.Height * sizefactor));
            panel_mouse.Size = picSize;
            panel_mouse.Location = new Point(0, 0);
            pb_left.Size = picSize;
            pb_left.Location = new Point(0, 0);
            pb_right.Size = picSize;
            pb_right.Location = new Point(0, 0);
            pb_middle.Size = picSize;
            pb_middle.Location = new Point(0, 0);
            pb_left_double.Size = picSize;
            pb_left_double.Location = new Point(0, 0);
            pb_right_double.Size = picSize;
            pb_right_double.Location = new Point(0, 0);
            pb_wheel.Size = picSize;
            pb_wheel.Location = new Point(0, 0);
            
            this.Size = picSize;
            Log.e("BI", "size change");
        }

        Size offset = new Size(0, 0);

        void RecalcOffset()
        {
            offset.Width = (int)( s.ButtonIndicatorPositionDistance * Math.Sin(s.ButtonIndicatorPositionAngle));
            offset.Height = (int)( s.ButtonIndicatorPositionDistance * Math.Cos(s.ButtonIndicatorPositionAngle)); 
        }

        void UpdatePosition()
        {
            if (OnlyDblClkIconVisible())
                return;
            Point buttonIndicatorCenter = Point.Subtract(cursorPosition, offset);
            this.Location = Point.Subtract(buttonIndicatorCenter, new Size(this.Size.Width / 2, this.Size.Height / 2));
            //this.Location = cursorPosition;
        }

        private bool OnlyDblClkIconVisible()
        {
            return !pb_left.Visible 
            && !pb_right.Visible 
            && !pb_middle.Visible 
            && !pb_wheel.Visible
            && (pb_left_double.Visible 
                || pb_right_double.Visible);
        }

        private void settingChanged(SettingsChangedEventArgs e)
        {
            switch (e.Name)
            {
                case "ButtonIndicator":
                    break;
                case "ButtonIndicatorPositionAngle":
                    RecalcOffset();
                    UpdatePosition();
                    break;
                case "ButtonIndicatorPositionDistance":
                    RecalcOffset();
                    UpdatePosition();
                    break;
                case "ButtonIndicatorSize":
                    UpdateSize();
                    UpdatePosition();
                    break;
            }
        }




    }
}
