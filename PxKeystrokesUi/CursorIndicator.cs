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

            m.MouseEvent += m_MouseEvent;
        }

        void m_MouseEvent(MouseRawEventArgs raw_e)
        {
            label1.Text = raw_e.Point.X.ToString();
            label2.Text = raw_e.Point.Y.ToString();
        }

        void CursorIndicator_FormClosed(object sender, FormClosedEventArgs e)
        {
            m.MouseEvent -= m_MouseEvent;
            m = null;
            s = null;
        }

        
    }
}
