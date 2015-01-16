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
    public partial class KeystrokesDisplay : Form
    {
        IKeystrokeEventProvider k;

        List<string> keystroke_history = new List<string>(5);
        bool LastHistoryLineIsText = false;
        bool LastHistoryLineRequiredNewLineAfterwards = false;

        int MAXLEN = 15;
        int MAXHISTORYLEN = 5;
        
        public KeystrokesDisplay(IKeystrokeEventProvider k)
        {
            this.k = k;
            this.k.KeystrokeEvent += k_KeystrokeEvent;

            InitializeComponent();
            this.TopMost = true;
            this.FormClosing += Form1_FormClosing;
            this.picker_textcolor.Color = label_keyhistory.ForeColor;
            this.picker_backcolor.Color = this.BackColor;

            keystroke_history.Add("");
            keystroke_history.Add("");
            keystroke_history.Add("");
            keystroke_history.Add("Press Ctrl + Alt + Shift");
            keystroke_history.Add("  to edit or close");

            slider_fontsize_Scroll(null, null); // update Fontsize
            updateLabel();

            NativeMethodsSWP.SetWindowTopMost(this.Handle);
            ActivateDisplayOnlyMode();
        }

        void k_KeystrokeEvent(KeystrokeEventArgs e)
        {
            CheckForSettingsMode(e);
            if (e.ShouldBeDisplayed)
            {
                string last = keystroke_history[keystroke_history.Count - 1];

                if (e.RequiresNewLine 
                    || last.Length >= MAXLEN
                    || !LastHistoryLineIsText
                    || LastHistoryLineRequiredNewLineAfterwards)
                {
                    keystroke_history.Add(e.ToString(true));
                }
                else
                {
                    // Replace
                    keystroke_history.RemoveAt(keystroke_history.Count - 1);
                    string replacedline = last + e.ToString(true);
                    keystroke_history.Add(replacedline);
                }
                LastHistoryLineIsText = e.StrokeType == KeystrokeType.Text;
                LastHistoryLineRequiredNewLineAfterwards = e.RequiresNewLineAfterwards;
                updateLabel();
            }
        }

        #region Settings Mode

        private void CheckForSettingsMode(KeystrokeEventArgs e)
        {
            if (e.Ctrl && e.Shift && e.Alt)
                ActivateSettingsMode();
            else
                ActivateDisplayOnlyMode();
        }

        void ActivateDisplayOnlyMode()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Opacity = slider_opacity.Value / 100.0;

            NativeMethodsGWL.ClickThrough(this.Handle);

            this.label_fontsize.Visible = false;
            this.label_opacity.Visible = false;
            this.slider_fontsize.Visible = false;
            this.slider_opacity.Visible = false;

            this.label_color.Visible = false;
            this.label_positioning.Visible = false;
            this.btn_setcenter.Visible = false;
            this.btn_setleft.Visible = false;
            this.btn_setright.Visible = false;
            this.btn_toggle_direction.Visible = false;
            this.button_backcolor.Visible = false;
            this.button_textcolor.Visible = false;
        }

        void ActivateSettingsMode()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;

            NativeMethodsGWL.CatchClicks(this.Handle);

            this.label_fontsize.Visible = true;
            this.label_opacity.Visible = true;
            this.slider_fontsize.Visible = true;
            this.slider_opacity.Visible = true;

            this.label_color.Visible = true;
            this.label_positioning.Visible = true;
            this.btn_setcenter.Visible = true;
            this.btn_setleft.Visible = true;
            this.btn_setright.Visible = true;
            this.btn_toggle_direction.Visible = true;
            this.button_backcolor.Visible = true;
            this.button_textcolor.Visible = true;
        }

        #endregion

        void cutHistory()
        {
            while (keystroke_history.Count > MAXHISTORYLEN)
            {
                keystroke_history.RemoveAt(0);
            }
        }

        void updateLabel()
        {
            cutHistory();
            string t = string.Join("\n", keystroke_history.ToArray());
            this.label_keyhistory.Text = t;
        }

        #region Form Events (Close, Buttons, Sliders)

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            k.KeystrokeEvent -= k_KeystrokeEvent;
        }

        private void slider_fontsize_Scroll(object sender, EventArgs e)
        {
            this.label_keyhistory.Font = new Font("Arial", this.slider_fontsize.Value / 10.0f);
        }

        private void slider_opacity_Scroll(object sender, EventArgs e)
        {

        }

        bool fromTop = true;

        private void btn_toggle_direction_Click(object sender, EventArgs e)
        {
            fromTop = !fromTop;
            btn_toggle_direction.Text = fromTop ? "From Top" : "From Bottom";
        }

        enum HPos
        {
            Left,
            Right,
            Center
        }

        HPos HorizontalPositioning = HPos.Left;

        private void btn_setleft_Click(object sender, EventArgs e)
        {
            HorizontalPositioning = HPos.Left;
        }

        private void btn_setcenter_Click(object sender, EventArgs e)
        {
            HorizontalPositioning = HPos.Center;
        }

        private void btn_setright_Click(object sender, EventArgs e)
        {
            HorizontalPositioning = HPos.Right;
        }

        private void button_textcolor_Click(object sender, EventArgs e)
        {
            picker_textcolor.ShowDialog(this);
            UpdateColors();
        }

        private void button_backcolor_Click(object sender, EventArgs e)
        {
            picker_backcolor.ShowDialog(this);
            UpdateColors();
        }

        private void UpdateColors()
        {
            this.BackColor = picker_backcolor.Color;
            this.label_keyhistory.ForeColor = picker_textcolor.Color;
        }

        #endregion

    }
}
