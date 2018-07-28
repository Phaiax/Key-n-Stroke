using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;


namespace PxKeystrokesUi
{
    public partial class KeystrokesDisplay : Form
    {
        enum FadeStatuses
        {
            FadingIn,
            FadingOut,
            Visible,
            Hidden
        }

        FadeStatuses FadeStatus { get; set; }

        IKeystrokeEventProvider k;

        Settings SettingsForm;

        List<TweenLabel> tweenLabels = new List<TweenLabel>(5);
        bool LastHistoryLineIsText = false;
        bool LastHistoryLineRequiredNewLineAfterwards = false;
        int NumberOfDeletionsAllowed = 0;

        SettingsStore settings;

        #region init (Constructor)

        public KeystrokesDisplay(IKeystrokeEventProvider k, SettingsStore s)
        {
            InitializeComponent();

            this.k = k;
            this.k.KeystrokeEvent += k_KeystrokeEvent;

            this.settings = s;
            this.settings.settingChanged += settingChanged;

            this.settings.OnSettingChangedAll();

            this.TopMost = true;
            this.FormClosing += Form1_FormClosing;

            addWelcomeInfo();

            NativeMethodsSWP.SetWindowTopMost(this.Handle);
            ActivateDisplayOnlyMode(true);

            if (settings.EnableWindowFade)
            {
                Opacity = 0;
                FadeStatus = FadeStatuses.Hidden;
            }
            else
            {
                Opacity = 1;
                FadeStatus = FadeStatuses.Visible;
            }

            settings.settingChanged += (SettingsChangedEventArgs e) =>
            {
                if (settings.EnableWindowFade && tweenLabels.Count == 0 && Opacity > 0)
                {
                    FadeOut();
                }
                else
                {
                    Opacity = 1;
                    FadeStatus = FadeStatuses.Visible;
                }
            };
        }

        private void KeystrokesDisplay_Shown(object sender, EventArgs e)
        {
            this.settings.OnSettingChangedAll();
        }

        #endregion

        #region keystroke handler

        void k_KeystrokeEvent(KeystrokeEventArgs e)
        {
            CheckForSettingsMode(e);
            if (e.ShouldBeDisplayed)
            {
                if (settings.EnableWindowFade && Opacity < 1)
                {
                    FadeIn();
                }

                if(!e.RequiresNewLine
                    && NumberOfDeletionsAllowed > 0
                    && LastHistoryLineIsText
                    && !LastHistoryLineRequiredNewLineAfterwards
                    && e.NoModifiers
                    && e.Key == Keys.Back)
                {
                    Log.e("BS", "delete last char");
                    Log.e("BS", "NumberOfDeletionsAllowed " + NumberOfDeletionsAllowed.ToString());
                    if (!removeLastChar())
                    {
                        // again
                        Log.e("BS", " failed");
                        NumberOfDeletionsAllowed = 0;
                        k_KeystrokeEvent(e);
                        return;
                    }
                }
                else if (e.RequiresNewLine
                    || !addingWouldFitInCurrentLine(e.ToString(false))
                    || !LastHistoryLineIsText
                    || LastHistoryLineRequiredNewLineAfterwards)
                {
                    addNextLine(e.ToString(false));
                    NumberOfDeletionsAllowed = e.Deletable ? 1 : 0;
                    Log.e("BS", "NumberOfDeletionsAllowed " + NumberOfDeletionsAllowed.ToString());
                }
                else
                {
                    Log.e("BS", "add to line ");
                    addToLine(e.ToString(false));
                    if (e.Deletable)
                        NumberOfDeletionsAllowed += 1;
                    else
                        NumberOfDeletionsAllowed = 0;
                    Log.e("BS", "NumberOfDeletionsAllowed " + NumberOfDeletionsAllowed.ToString());
                }

                LastHistoryLineIsText = e.StrokeType == KeystrokeType.Text;
                LastHistoryLineRequiredNewLineAfterwards = e.RequiresNewLineAfterwards;
            }
        }


        void addWelcomeInfo()
        {
            MessageBox.Show("PyKeystrokesForScreencasts:\r\n\r\nHold Ctrl + Alt + Shift to move and resize. \n\rUse the tray icon to access settings.");
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
                NativeMethodsGWL.HideFromAltTab(this.Handle);

                moveresize_MouseUp(null, null); // disable all dragging

                this.bn_close.Visible = false;
                this.bn_resize.Visible = false;
                this.bn_settings.Visible = false;
                this.panel_textposhelper.Visible = false;

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

                moveresize_MouseUp(null, null); // disable all dragging

                this.bn_close.Visible = true;
                this.bn_resize.Visible = true;
                this.bn_settings.Visible = true;
                this.panel_textposhelper.Visible = true;
                BringSettingsControlsToFront();

                foreach (TweenLabel T in tweenLabels)
                {
                    T.Visible = false;
                }

                SettingsModeActivated = true;
            }
        }

        private void BringSettingsControlsToFront()
        {
            this.panel_textposhelper.BringToFront();
            this.bn_close.BringToFront();
            this.bn_resize.BringToFront();
            this.bn_settings.BringToFront();
        }

        private void settingChanged(SettingsChangedEventArgs e)
        {
            switch (e.Name)
            {
                case "BackgroundColor":
                    this.BackColor = settings.BackgroundColor;
                    break;
                case "Opacity":
                    this.Opacity = settings.Opacity;
                    break;
                case "WindowLocation":
                    this.Location = settings.WindowLocation;
                    Log.e("KD", String.Format("Apply X: {0}", settings.WindowLocation.X));
                    break;
                case "WindowSize":
                    this.Size = settings.WindowSize;
                    break;
                case "PanelLocation":
                    this.panel_textposhelper.Location = settings.PanelLocation;
                    break;
                case "PanelSize":
                    this.panel_textposhelper.Size = settings.PanelSize;
                    break;
            }
        }

        void ShowSettingsDialog()
        {
            if (SettingsForm != null)
            {
                SettingsForm.Dispose();
            }
            SettingsForm = new Settings(settings);
            SettingsForm.Show(this);
        }

        #endregion

        #region display and animate Label


        void addToLine(string chars)
        {
            TweenLabel T = tweenLabels[tweenLabels.Count - 1];
            T.Text += chars;
            T.Text = HttpUtility.UrlDecode(T.Text, Encoding.UTF8);
            T.Refresh();
        }

        private bool removeLastChar()
        {
            if(tweenLabels.Count == 0)
            {
                return false;
            }

            TweenLabel T = tweenLabels[tweenLabels.Count - 1];
            if (T.Text.Length == 0)
                return false;
            T.Text = T.Text.Substring(0, T.Text.Length - 1);
            NumberOfDeletionsAllowed -= 1;
            T.Text = HttpUtility.UrlDecode(T.Text, Encoding.UTF8);
            T.Refresh();
            return true;
        }

        void addNextLine(string chars)
        {
            TweenLabel nTL = TweenLabel.getNewLabel(this, settings);
            nTL.Size = getLabelSize();
            nTL.Location = getLabelStartPosition();
            nTL.historyTimeout += nTL_historyTimeout;

            nTL.Text = chars;
            this.Controls.Add(nTL);
            nTL.BringToFront();
            if (SettingsModeActivated)
                BringSettingsControlsToFront();

            int u = 0;
            foreach (Control T in this.Controls)
            {
                if(T.GetType() == typeof(TweenLabel) && T != nTL){
                    u++;
                    ((TweenLabel)T).TweenMove(getLabelMoveDirection());
                }
                
            }

            nTL.tweenFadeIn();
            tweenLabels.Add(nTL);

            while (tweenLabels.Count > settings.HistoryLength)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Dele{0} {1} u{2}", 
                    tweenLabels.Count, this.Controls.Count, u));

                tweenLabels[0].FadeOutAndRecycle(null);
                tweenLabels.RemoveAt(0);
            }

        }

        void nTL_historyTimeout(TweenLabel l)
        {
            tweenLabels.Remove(l);

            if(settings.EnableWindowFade && tweenLabels.Count == 0)
            {
                FadeOut();
            }
        }

        bool addingWouldFitInCurrentLine(string s)
        {
            if(tweenLabels.Count == 0)
                return false;

            return tweenLabels[tweenLabels.Count - 1].AddingWouldFit(s);
        }

        Point getLabelStartPosition()
        {
            if (settings.LabelTextDirection == TextDirection.Down)
            {
                return panel_textposhelper.Location;
            }
            else
            {
                return new Point(panel_textposhelper.Location.X,
                    panel_textposhelper.Location.Y +
                    panel_textposhelper.Size.Height -
                    settings.LineDistance);
            }
        }

        Size getLabelSize()
        {
            return new Size(panel_textposhelper.Width, settings.LineDistance);
        }

        Point getLabelMoveDirection()
        {
            if (settings.LabelTextDirection == TextDirection.Down)
            {
                return new Point(0, settings.LineDistance);
            }
            else
            {
                return new Point(0, -settings.LineDistance);
            }
        }

        #endregion

        #region Form Events (Close, Buttons)

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            k.KeystrokeEvent -= k_KeystrokeEvent;
            settings.settingChanged -= settingChanged;

            settings.SaveAll();
        }

        private void bn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bn_settings_Click(object sender, EventArgs e)
        {
            ShowSettingsDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSettingsDialog();
        }

        private void contributeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UrlOpener.OpenGithub();
        }

        private void reportErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UrlOpener.OpenGithubIssues();
        }

        #endregion

               
        #region Moving

        private bool MoveWindowDragging = false;
        private bool MovePanelDragging = false;
        private Point MoveDragCursorPoint;
        private Point MoveDragFormPoint;

        private void bn_move_MouseDown(object sender, MouseEventArgs e)
        {
            MoveWindowDragging = true;
            MovePanelDragging = false;
            MoveDragCursorPoint = Cursor.Position;
            MoveDragFormPoint = this.Location;
        }

        private void bn_move_MouseMove(object sender, MouseEventArgs e)
        {
            if (MoveWindowDragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(MoveDragCursorPoint));
                settings.WindowLocation = Point.Add(MoveDragFormPoint, new Size(dif));
            }
        }

        private void moveresize_MouseUp(object sender, MouseEventArgs e)
        {
            MoveWindowDragging = false;
            MovePanelDragging = false;
            ResizeWindowDragging = false;
            ResizePanelDragging = false;
        }

        #endregion

        #region resizing

        private bool ResizeWindowDragging = false;
        private bool ResizePanelDragging = false;
        private Point ResizeDragCursorPoint;
        private Size ResizeDragFormPoint;

        private void bn_resize_MouseDown(object sender, MouseEventArgs e)
        {
            ResizeWindowDragging = true;
            ResizePanelDragging = false;
            ResizeDragCursorPoint = Cursor.Position;
            ResizeDragFormPoint = this.Size;
        }

        private void bn_resize_MouseMove(object sender, MouseEventArgs e)
        {
            if (ResizeWindowDragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(ResizeDragCursorPoint));
                settings.WindowSize = new Size(Point.Add(new Point(ResizeDragFormPoint), new Size(dif)));
            }
        }

        #endregion



        #region moving inner panel

        private void panel_textposhelper_MouseDown(object sender, MouseEventArgs e)
        {
            MoveWindowDragging = false;
            MovePanelDragging = true;
            MoveDragCursorPoint = Cursor.Position;
            MoveDragFormPoint = panel_textposhelper.Location;
        }

        private void panel_textposhelper_MouseMove(object sender, MouseEventArgs e)
        {
            if (MovePanelDragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(MoveDragCursorPoint));
                settings.PanelLocation = Point.Add(MoveDragFormPoint, new Size(dif));
            }
        }


        #endregion

        #region resizing inner panel

        private void bn_panel_resize_MouseDown(object sender, MouseEventArgs e)
        {
            ResizeWindowDragging = false;
            ResizePanelDragging = true;
            ResizeDragCursorPoint = Cursor.Position;
            ResizeDragFormPoint = panel_textposhelper.Size;
        }

        private void bn_panel_resize_MouseMove(object sender, MouseEventArgs e)
        {
            if (ResizePanelDragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(ResizeDragCursorPoint));
                settings.PanelSize = new Size(Point.Add(new Point(ResizeDragFormPoint), new Size(dif)));
            }

        }


        #endregion







        public void FadeIn()
        {
            if (FadeStatus == FadeStatuses.Hidden || FadeStatus == FadeStatuses.FadingOut)
            {
                FadeStatus = FadeStatuses.FadingIn;
                this.Visible = true;
                Timer T = new Timer();
                T.Tick += FadeInHandler;
                T.Interval = 40; // bit more than 30 Hz
                T.Start();
            }
        }

        void FadeInHandler(object sender, EventArgs e)
        {
            if (FadeStatus == FadeStatuses.Visible || FadeStatus == FadeStatuses.FadingOut)
            {
                ((Timer)sender).Tick -= FadeInHandler;
                ((Timer)sender).Stop();
                return;
            }

            this.Opacity += 0.12f;
            if (Opacity >= 1)
            {
                Timer T = (Timer)sender;
                ((Timer)sender).Tick -= FadeInHandler;
                T.Stop();
                Opacity = 1;
                FadeStatus = FadeStatuses.Visible;
            }
            this.Refresh();
        }

        public void FadeOut()
        {
            if (FadeStatus == FadeStatuses.Visible || FadeStatus == FadeStatuses.FadingIn)
            {
                FadeStatus = FadeStatuses.FadingOut;
                this.Visible = true;
                Timer T = new Timer();
                T.Tick += FadeOutHandler;
                T.Interval = 40; // bit more than 30 Hz
                T.Start();
            }
        }

        void FadeOutHandler(object sender, EventArgs e)
        {
            if (FadeStatus == FadeStatuses.Hidden || FadeStatus == FadeStatuses.FadingIn)
            {
                ((Timer)sender).Tick -= FadeOutHandler;
                ((Timer)sender).Stop();
                return;
            }

            this.Opacity -= 0.12f;
            if (Opacity <= 0)
            {
                Timer T = (Timer)sender;
                ((Timer)sender).Tick -= FadeOutHandler;
                T.Stop();
                Opacity = 0;
                FadeStatus = FadeStatuses.Hidden;
            }
            this.Refresh();
        }



    }
}
