namespace PxKeystrokesUi
{
    partial class KeystrokesDisplay
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.label_keyhistory = new System.Windows.Forms.Label();
            this.bn_close = new System.Windows.Forms.Button();
            this.bn_resize = new System.Windows.Forms.Button();
            this.bn_settings = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label_keyhistory
            // 
            this.label_keyhistory.AutoSize = true;
            this.label_keyhistory.ForeColor = System.Drawing.Color.White;
            this.label_keyhistory.Location = new System.Drawing.Point(12, 9);
            this.label_keyhistory.Name = "label_keyhistory";
            this.label_keyhistory.Size = new System.Drawing.Size(29, 13);
            this.label_keyhistory.TabIndex = 14;
            this.label_keyhistory.Text = "label";
            this.label_keyhistory.MouseEnter += new System.EventHandler(this.label_keyhistory_MouseEnter);
            this.label_keyhistory.MouseLeave += new System.EventHandler(this.label_keyhistory_MouseLeave);
            // 
            // bn_close
            // 
            this.bn_close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bn_close.BackColor = System.Drawing.Color.DarkRed;
            this.bn_close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bn_close.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bn_close.Location = new System.Drawing.Point(197, -28);
            this.bn_close.Margin = new System.Windows.Forms.Padding(0);
            this.bn_close.Name = "bn_close";
            this.bn_close.Size = new System.Drawing.Size(40, 47);
            this.bn_close.TabIndex = 15;
            this.bn_close.Text = "x";
            this.bn_close.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.bn_close.UseVisualStyleBackColor = false;
            this.bn_close.Click += new System.EventHandler(this.bn_close_Click);
            // 
            // bn_resize
            // 
            this.bn_resize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bn_resize.BackColor = System.Drawing.Color.DarkGray;
            this.bn_resize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bn_resize.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bn_resize.Location = new System.Drawing.Point(240, 156);
            this.bn_resize.Name = "bn_resize";
            this.bn_resize.Size = new System.Drawing.Size(38, 37);
            this.bn_resize.TabIndex = 16;
            this.bn_resize.Text = "⇲";
            this.bn_resize.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.bn_resize.UseVisualStyleBackColor = false;
            this.bn_resize.Click += new System.EventHandler(this.bn_resize_Click);
            this.bn_resize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bn_resize_MouseDown);
            this.bn_resize.MouseMove += new System.Windows.Forms.MouseEventHandler(this.bn_resize_MouseMove);
            this.bn_resize.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bn_resize_MouseUp);
            // 
            // bn_settings
            // 
            this.bn_settings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bn_settings.BackColor = System.Drawing.Color.DarkGray;
            this.bn_settings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bn_settings.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bn_settings.Location = new System.Drawing.Point(159, -8);
            this.bn_settings.Margin = new System.Windows.Forms.Padding(0);
            this.bn_settings.Name = "bn_settings";
            this.bn_settings.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.bn_settings.Size = new System.Drawing.Size(38, 34);
            this.bn_settings.TabIndex = 18;
            this.bn_settings.Text = "⚙";
            this.bn_settings.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.bn_settings.UseVisualStyleBackColor = false;
            this.bn_settings.Click += new System.EventHandler(this.bn_settings_Click);
            // 
            // KeystrokesDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(268, 188);
            this.Controls.Add(this.bn_settings);
            this.Controls.Add(this.bn_resize);
            this.Controls.Add(this.bn_close);
            this.Controls.Add(this.label_keyhistory);
            this.Location = new System.Drawing.Point(20, 0);
            this.Name = "KeystrokesDisplay";
            this.ShowInTaskbar = false;
            this.Text = "Win8KeystrokesForScreencasts";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bn_move_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.bn_move_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bn_move_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_keyhistory;
        private System.Windows.Forms.Button bn_close;
        private System.Windows.Forms.Button bn_resize;
        private System.Windows.Forms.Button bn_settings;
    }
}

