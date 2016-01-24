namespace PxKeystrokesUi
{
    partial class ButtonIndicator
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pb_mouse = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb_mouse)).BeginInit();
            this.SuspendLayout();
            // 
            // pb_mouse
            // 
            this.pb_mouse.BackColor = System.Drawing.Color.Transparent;
            this.pb_mouse.Location = new System.Drawing.Point(76, 49);
            this.pb_mouse.Name = "pb_mouse";
            this.pb_mouse.Size = new System.Drawing.Size(62, 62);
            this.pb_mouse.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pb_mouse.TabIndex = 0;
            this.pb_mouse.TabStop = false;
            // 
            // ButtonIndicator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.pb_mouse);
            this.Name = "ButtonIndicator";
            this.Text = "CursorIndicator";
            this.Load += new System.EventHandler(this.ButtonIndicator_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pb_mouse)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pb_mouse;



    }
}