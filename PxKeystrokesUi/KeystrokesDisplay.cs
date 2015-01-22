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

        Settings SettingsForm;

        List<string> keystroke_history = new List<string>(5);
        bool LastHistoryLineIsText = false;
        bool LastHistoryLineRequiredNewLineAfterwards = false;

        int MAXLEN = 15;
        int MAXHISTORYLEN = 5;

        SettingsStore settings;

        #region init (Constructor)

        public KeystrokesDisplay(IKeystrokeEventProvider k, SettingsStore s)
        {
            InitializeComponent();

            this.k = k;
            this.k.KeystrokeEvent += k_KeystrokeEvent;

            this.settings = s;
            this.settings.settingChanged += settingChanged;

            Timer T = new Timer();
            T.Interval = 10;
            T.Tick += T_DeferredSettingsActivation;
            T.Start();

            this.TopMost = true;
            this.FormClosing += Form1_FormClosing;

            keystroke_history.Add("");
            keystroke_history.Add("");
            keystroke_history.Add("");
            keystroke_history.Add("Press Ctrl + Alt + Shift");
            keystroke_history.Add("  to edit or close");

            updateLabel();

            NativeMethodsSWP.SetWindowTopMost(this.Handle);
            ActivateDisplayOnlyMode(true);
        }

        void T_DeferredSettingsActivation(object sender, EventArgs e)
        {
            ((Timer)sender).Stop();
            this.settings.OnSettingChangedAll();
        }

        #endregion

        #region keystroke handler

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

        #endregion

        #region Settings Mode

        private void CheckForSettingsMode(KeystrokeEventArgs e)
        {
            if (e.Ctrl && e.Shift && e.Alt)
                ActivateSettingsMode();
            else
                ActivateDisplayOnlyMode(false);
        }

        bool SettingsModeActivated = false;

        void ActivateDisplayOnlyMode(bool force)
        {
            if (SettingsModeActivated || force)
            {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                this.Opacity = settings.Opacity;

                NativeMethodsGWL.ClickThrough(this.Handle);

                ResizeDragging = false;
                MoveDragging = false;

                this.bn_close.Visible = false;
                this.bn_resize.Visible = false;
                this.bn_settings.Visible = false;

                SettingsModeActivated = false;

                settings.SaveAll();
            }
        }

        void ActivateSettingsMode()
        {
            if (!SettingsModeActivated)
            {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                NativeMethodsGWL.CatchClicks(this.Handle);

                this.Opacity = 1;

                ResizeDragging = false;
                MoveDragging = false;

                this.bn_close.Visible = true;
                this.bn_resize.Visible = true;
                this.bn_settings.Visible = true;

                SettingsModeActivated = true;
            }
        }

        private void settingChanged(SettingsChangedEventArgs e)
        {
            switch (e.Name)
            {
                case "LabelFont":
                    label_keyhistory.Font = settings.LabelFont;
                    break;
                case "TextColor":
                    label_keyhistory.ForeColor = settings.TextColor;
                    break;
                case "BackgroundColor":
                    this.BackColor = settings.BackgroundColor;
                    break;
                case "Opacity":
                    this.Opacity = settings.Opacity;
                    break;
                case "WindowLocation":
                    this.Location = settings.WindowLocation;
                    System.Diagnostics.Debug.WriteLine(String.Format("Apply X: {0}", settings.WindowLocation.X));
                    break;
                case "WindowSize":
                    this.Size = settings.WindowSize;
                    break;

            }
        }

        #endregion

        #region display and animate Label

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

        #endregion

        #region Form Events (Close, Buttons)

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            k.KeystrokeEvent -= k_KeystrokeEvent;
            settings.SaveAll();
        }

        private void bn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bn_settings_Click(object sender, EventArgs e)
        {
            if (SettingsForm != null)
            {
                SettingsForm.Dispose();
            }
            SettingsForm = new Settings(settings);
            SettingsForm.Show(this);
        }

        #endregion

        #region Moving

        private bool MoveDragging = false;
        private Point MoveDragCursorPoint;
        private Point MoveDragFormPoint;

        private void bn_move_MouseDown(object sender, MouseEventArgs e)
        {
            MoveDragging = true;
            MoveDragCursorPoint = Cursor.Position;
            MoveDragFormPoint = this.Location;
        }

        private void bn_move_MouseMove(object sender, MouseEventArgs e)
        {
            if (MoveDragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(MoveDragCursorPoint));
                settings.WindowLocation = Point.Add(MoveDragFormPoint, new Size(dif));
            }
        }

        private void bn_move_MouseUp(object sender, MouseEventArgs e)
        {
            MoveDragging = false;
        }

        #endregion

        #region resizing

        private bool ResizeDragging = false;
        private Point ResizeDragCursorPoint;
        private Size ResizeDragFormPoint;

        private void bn_resize_MouseDown(object sender, MouseEventArgs e)
        {
            ResizeDragging = true;
            ResizeDragCursorPoint = Cursor.Position;
            ResizeDragFormPoint = this.Size;
        }

        private void bn_resize_MouseMove(object sender, MouseEventArgs e)
        {
            if (ResizeDragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(ResizeDragCursorPoint));
                settings.WindowSize = new Size(Point.Add(new Point(ResizeDragFormPoint), new Size(dif)));
            }
        }

        private void bn_resize_MouseUp(object sender, MouseEventArgs e)
        {
            ResizeDragging = false;
        }

        #endregion

        #region label highlight on mouse over

        private void label_keyhistory_MouseEnter(object sender, EventArgs e)
        {
            if(SettingsModeActivated)
            {
                ((Label)sender).BackColor = Color.Gray;
            }
        }

        private void label_keyhistory_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = Color.Transparent;
        }

        #endregion

    }
}
