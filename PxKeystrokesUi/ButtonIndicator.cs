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

            BackColor = Color.Lavender;
            TransparencyKey = Color.Lavender;
        }

        private void ButtonIndicator_Load(object sender, EventArgs e)
        {
            pb_mouse.Image = ImageResources.mouse;
            RecalcOffset();
            UpdateSize();
        }

        Point cursorPosition;

        void m_MouseEvent(MouseRawEventArgs raw_e)
        {
            cursorPosition = raw_e.Position;
            UpdatePosition();
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
            Size picSize = new Size( (int)(ImageResources.mouse.Width * sizefactor),
                                     (int)(ImageResources.mouse.Height * sizefactor));
            pb_mouse.Size = picSize;
            pb_mouse.Location = new Point(0, 0);
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
            Point buttonIndicatorCenter = Point.Subtract(cursorPosition, offset);
            this.Location = Point.Subtract(buttonIndicatorCenter, new Size(this.Size.Width / 2, this.Size.Height / 2));
            //this.Location = cursorPosition;
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
