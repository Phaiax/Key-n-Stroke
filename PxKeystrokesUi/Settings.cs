using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PxKeystrokesUi
{
    public partial class Settings : Form
    {
        public Settings(SettingsStore s)
        {
            this.settings = s;
            InitializeComponent();
            UpdateSliderValues();

            switch (settings.LabelAnimation)
            {
                case Style.NoAnimation:
                    rb_style_noani.Checked = true;
                    break;
                case Style.Slide:
                    rb_style_slide.Checked = true;
                    break;
            }

            switch (settings.LabelTextAlignment)
            {
                case TextAlignent.Right:
                    rb_align_right.Checked = true;
                    break;
                case TextAlignent.Center:
                    rb_align_center.Checked = true;
                    break;
                case TextAlignent.Left:
                    rb_align_left.Checked = true;
                    break;
            }

            switch (settings.LabelTextDirection)
            {
                case TextDirection.Up:
                    rb_td_up.Checked = true;
                    break;
                case TextDirection.Down:
                    rb_td_down.Checked = true;
                    break;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://github.com/Phaiax/PxKeystrokesForScreencasts/";
            ProcessStartInfo si = new ProcessStartInfo(url);
            Process.Start(si);
        }

        SettingsStore settings;

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            settings.SaveAll();
        }

        private void rb_align_left_CheckedChanged(object sender, EventArgs e)
        {
            settings.LabelTextAlignment = TextAlignent.Left;
        }

        private void rb_align_center_CheckedChanged(object sender, EventArgs e)
        {
            settings.LabelTextAlignment = TextAlignent.Center;
        }

        private void rb_align_right_CheckedChanged(object sender, EventArgs e)
        {
            settings.LabelTextAlignment = TextAlignent.Right;
        }

        private void rb_td_down_CheckedChanged(object sender, EventArgs e)
        {
            settings.LabelTextDirection = TextDirection.Down;
        }

        private void rb_td_up_CheckedChanged(object sender, EventArgs e)
        {
            settings.LabelTextDirection = TextDirection.Up;
        }

        private void rb_style_noani_CheckedChanged(object sender, EventArgs e)
        {
            settings.LabelAnimation = Style.NoAnimation;
        }

        private void rb_style_slide_CheckedChanged(object sender, EventArgs e)
        {
            settings.LabelAnimation = Style.Slide;
        }

        private void button_textcolor_Click(object sender, EventArgs e)
        {
            picker_textcolor.Color = settings.TextColor;
            if(picker_textcolor.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                settings.TextColor = picker_textcolor.Color;
            }
        }

        private void button_textfont_Click(object sender, EventArgs e)
        {
            fontDialog_text.Font = settings.LabelFont;
            if(fontDialog_text.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                settings.LabelFont = fontDialog_text.Font;
                UpdateSliderValues();
            }
        }

        private void slider_fontsize_Scroll(object sender, EventArgs e)
        {
            float newsize = (float)slider_fontsize.Value / 100f;
            settings.LabelFont = new Font(settings.LabelFont.FontFamily, newsize, settings.LabelFont.Style);
        }

        private void button_backcolor_Click(object sender, EventArgs e)
        {
            picker_backcolor.Color = settings.BackgroundColor;
            if (picker_backcolor.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                settings.BackgroundColor = picker_backcolor.Color;
            }
        }

        private void slider_opacity_Scroll(object sender, EventArgs e)
        {
            settings.Opacity = (float)slider_opacity.Value / 100f;
        }

        private void UpdateSliderValues()
        {
            int fontsize = (int)(settings.LabelFont.SizeInPoints * 100f);
            slider_fontsize.Maximum = Math.Max(slider_fontsize.Maximum, fontsize);
            slider_fontsize.Minimum = Math.Min(slider_fontsize.Minimum, fontsize);
            slider_fontsize.Value = fontsize;

            slider_opacity.Value = (int)(settings.Opacity * 100f);
        }
    }
}
