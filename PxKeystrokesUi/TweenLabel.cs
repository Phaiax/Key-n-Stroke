using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PxKeystrokesUi
{
    class TweenLabel : Control
    {
        #region Recycling

        static Stack<TweenLabel> unusedLabels = new Stack<TweenLabel>(15);
        
        static int debug_nextId = 1;
        int debug_id;

        void PrintDebug(String msg)
        {
            Log.e("TL", debug_id.ToString() + " y:" + this.Location.Y + " '" + this.Text + "' " + msg);
        }

        public static TweenLabel getNewLabel(Form form, SettingsStore s)
        {
            if (unusedLabels.Count > 0)
            {
                return unusedLabels.Pop().Init(form, s);
            }
            TweenLabel t = new TweenLabel().Init(form, s);
            t.debug_id = debug_nextId;
            debug_nextId += 1;
            return t;
        }

        public void Recycle()
        {
            PrintDebug("recyle");
            this.Parent.Controls.Remove(this);
            this.Parent = null;
            this.Visible = false;
            attachedToTimer = false;
            if (TweenLabel.moveTimer != null)
                TweenLabel.moveTimer.Tick -= T_Move;
            this.historyTimeout = null;
            TweenLabel.unusedLabels.Push(this);
        }

        #endregion

        SettingsStore settings;

        private TweenLabel Init(Form form, SettingsStore s)
        {
            PrintDebug("revive");
            if (settings == null)
            {
                settings = s;
                settings.settingChanged += settingChanged;
                this.TextChanged += TweenLabel_TextChanged;
                this.Font = settings.LabelFont;
                this.ForeColor = settings.TextColor;
                this.DoubleBuffered = true;
            }

            return this;
        }

        void TweenLabel_TextChanged(object sender, EventArgs e)
        {
            ResetHistoryTimeoutTimer();
        }


        private void settingChanged(SettingsChangedEventArgs e)
        {
            switch (e.Name)
            {
                case "LabelFont":
                    this.Font = settings.LabelFont;
                    break;
                case "TextColor":
                    this.ForeColor = settings.TextColor;
                    break;
                case "HistoryTimeout":
                    ResetHistoryTimeoutTimer();
                    break;
            }
        }

        public void tweenFadeIn()
        {
            PrintDebug("fade in start");
            this.Opacity = 0;
            this.Visible = true;
            Timer T = new Timer();
            T.Tick += T_FadeIn;
            T.Interval = 40; // bit more than 30 Hz
            T.Start();
        }

        void T_FadeIn(object sender, EventArgs e)
        {
            this.Opacity += 0.12f;
            if (Opacity >= 1)
            {
                Timer T = (Timer)sender;
                T.Stop();
                Opacity = 1;
                PrintDebug("fade in finished");
                ResetHistoryTimeoutTimer();
            }
            this.Refresh();
        }

        Timer fadeOutTimer;
        Func<TweenLabel, bool> fadeOutOnFinish;

        public void FadeOutAndRecycle(Func<TweenLabel, bool> onFinish)
        {
            CancelTimeoutTimer();
            if (fadeOutTimer == null)
            {
                fadeOutTimer = new Timer();
                fadeOutTimer.Tick += T_FadeOut;
            }
            if (!fadeOutTimer.Enabled)
            {
                this.Opacity = 1;
                fadeOutOnFinish = onFinish;
                fadeOutTimer.Interval = 40; // bit more than 30 Hz
                PrintDebug("fade out start");
                fadeOutTimer.Start();
                CancelTimeoutTimer();
            }
        }

        void T_FadeOut(object sender, EventArgs e)
        {
            this.Opacity -= 0.04f;
            if (Opacity <= 0)
            {
                fadeOutTimer.Stop();
                Opacity = 0;
                if (fadeOutOnFinish != null)
                {
                    fadeOutOnFinish(this);
                    fadeOutOnFinish = null;
                }
                PrintDebug("fade out finish");
                this.Recycle();
                return;
            }
            this.Refresh();
        }

        bool IsFadingOut()
        {
            return fadeOutTimer != null && fadeOutTimer.Enabled;
        }

        Point moveStartLocation;
        Point moveDir;
        float movePercent;
        bool attachedToTimer = false;
        static Timer moveTimer; // one timer for all for synchronous
        static int moveTimerUserCount = 0;

        public void TweenMove(Point newMoveDir)
        {
            if (moveTimer == null)
            {
                TweenLabel.moveTimer = new Timer();
                TweenLabel.moveTimer.Interval = 40; // bit more than 30 Hz
                TweenLabel.moveTimer.Start();
            }
            if (!attachedToTimer)
            {
                TweenLabel.moveTimer.Tick += T_Move;
                TweenLabel.moveTimerUserCount++;
                Log.e("TLMove", "Handler attached");

                movePercent = 0;
                moveStartLocation = this.Location;
                moveDir = newMoveDir;

                attachedToTimer = true;
            }
            else
            {
                movePercent = (float)moveDir.Y * movePercent / ((float)newMoveDir.Y + moveDir.Y);
                moveDir = Point.Add(moveDir, new Size(newMoveDir));
            }
            
        }

        void T_Move(object sender, EventArgs e)
        {
            movePercent += 0.2f;
            if (movePercent >= 1)
            {
                Timer T = (Timer)sender;
                T.Tick -= T_Move;
                Log.e("TLMove", "Handler removed");


                attachedToTimer = false;
                moveTimerUserCount -= 1;
                this.Location = Point.Add(moveStartLocation, new Size(moveDir));
                return;
            }
            this.Location = Point.Add(moveStartLocation, new Size((int)(moveDir.X * movePercent), (int)(moveDir.Y * movePercent)));
        }

        float opacity = 1f;

        public float Opacity
        {
            get { return opacity; }
            set { opacity = value; }
        }

        public void DrawString(System.Drawing.Graphics G)
        {
            Color C = Color.FromArgb(Math.Min((int)(opacity * 255f), 255), this.ForeColor);
            SolidBrush drawBrush = new SolidBrush(C);
            StringFormat drawFormat = new StringFormat();
            SizeF StringSize = G.MeasureString(this.Text, this.Font);
            int x = 0;
            int y = 0;

            switch (settings.LabelTextAlignment)
            {
                case TextAlignent.Left:
                    x = 0;
                    break;
                case TextAlignent.Right:
                    x = this.Width - (int)StringSize.Width;
                    break;
                case TextAlignent.Center:
                    x = (int)(((float)this.Width - StringSize.Width) / 2.0f);
                    break;
            }

            // vertical center
            y = (int)(((float)this.Height - StringSize.Height) / 2.0f);

            using (Bitmap buffer = new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics graphics = Graphics.FromImage(buffer))
                {
                    TextRenderer.DrawText(graphics, this.Text, this.Font, new Point(x, y),
                                    C, Color.Transparent, TextFormatFlags.NoPadding);

                }

                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = opacity;
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                //now draw the image  
                G.DrawImage(buffer, new Rectangle(0, 0, buffer.Width, buffer.Height), 0, 0, buffer.Width, buffer.Height, GraphicsUnit.Pixel, attributes);
                //G.DrawImageUnscaled(buffer, this.ClientRectangle);

            }

            //TextRenderer.DrawText(G, this.Text, this.Font, new Point(x, y),
            //    C, Color.Transparent, TextFormatFlags.NoPadding);
            
            //G.DrawString(this.Text, this.Font, drawBrush, x, y, drawFormat);
            drawBrush.Dispose();

            // Declare and instantiate a new pen.
            //System.Drawing.Pen myPen = new System.Drawing.Pen(Color.Aqua);

            // Draw an aqua rectangle in the rectangle represented by the control.
            //G.DrawRectangle(myPen, new Rectangle(new Point(0, 0),
            //   Size.Subtract(this.Size, new Size(1, 1))));
        }

        public bool AddingWouldFit(string additionalChars)
        {
            //SizeF StringSize = this.CreateGraphics().MeasureString(
            //                        this.Text + additionalChars, this.Font);
            Size StringSize2 = TextRenderer.MeasureText(this.CreateGraphics(),
                                    this.Text, this.Font, this.Size, TextFormatFlags.NoPadding);

            //return StringSize.Width < (this.Width - 2);
            return StringSize2.Width < (this.Width - 25);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            DrawString(e.Graphics);
        }

        Timer timeoutTimer;
        public delegate void HistoryTimeOutEvent(TweenLabel l);
        public event HistoryTimeOutEvent historyTimeout;

        private void ResetHistoryTimeoutTimer()
        {
            PrintDebug("reset history timeout");
            CancelTimeoutTimer();
            if (!settings.EnableHistoryTimeout || IsFadingOut())
                return;
            if (timeoutTimer == null)
            {
                timeoutTimer = new Timer();
                timeoutTimer.Tick += timeoutTimer_Tick;
            }

            timeoutTimer.Interval = settings.HistoryTimeout;
            timeoutTimer.Start();
        }

        private void CancelTimeoutTimer()
        {
            if (timeoutTimer != null)
            {
                timeoutTimer.Stop();
            }
        }

        void timeoutTimer_Tick(object sender, EventArgs e)
        {
            PrintDebug("fire history timeout");
            CancelTimeoutTimer();
            if (historyTimeout != null)
            {
                historyTimeout.Invoke(this);
            }
            FadeOutAndRecycle(null);
        }

    }
}
