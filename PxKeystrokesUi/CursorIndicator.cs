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
    public partial class CursorIndicator : Form
    {
        IMouseRawEventProvider m;
        SettingsStore s;

        public CursorIndicator(IMouseRawEventProvider m, SettingsStore s)
        {
            InitializeComponent();

            this.m = m;
            this.s = s;
            FormClosed += CursorIndicator_FormClosed;

            NativeMethodsSWP.SetWindowTopMost(this.Handle);
            SetFormStyles();

            m.MouseEvent += m_MouseEvent;
            Paint += CursorIndicator_Paint;

            BackColor = Color.Lavender;
            TransparencyKey = Color.Lavender;
        }

        void CursorIndicator_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = this.CreateGraphics();
            Console.WriteLine(g.Clip.GetBounds(g).ToString());
            Console.WriteLine(g.ClipBounds.ToString());
            Pen p = new Pen(Color.Red, 7);
            g.FillEllipse(p.Brush, 0, 0, this.Width, this.Height);
        }

        Point cursorPosition;

        void m_MouseEvent(MouseRawEventArgs raw_e)
        {
            cursorPosition = raw_e.Position;
            //label1.Text = raw_e.Position.X.ToString();
            //label2.Text = raw_e.Position.Y.ToString();
            UpdatePosition();
        }

        void CursorIndicator_FormClosed(object sender, FormClosedEventArgs e)
        {
            m.MouseEvent -= m_MouseEvent;
            m = null;
            s = null;
        }

        void SetFormStyles()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Opacity = 0.8;
            NativeMethodsGWL.ClickThrough(this.Handle);
            
            this.Size = new Size(50, 50);
            this.Invalidate(new Rectangle(0, 0, this.Size.Width, this.Size.Height));

            UpdatePosition();
        }

        void UpdatePosition()
        {
            this.Location = Point.Subtract(cursorPosition, new Size(this.Size.Width / 2, this.Size.Height / 2));
            //this.Location = cursorPosition;
        }
        
    }
}
