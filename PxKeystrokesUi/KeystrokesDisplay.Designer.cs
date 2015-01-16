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
            this.slider_fontsize = new System.Windows.Forms.TrackBar();
            this.label_fontsize = new System.Windows.Forms.Label();
            this.label_opacity = new System.Windows.Forms.Label();
            this.slider_opacity = new System.Windows.Forms.TrackBar();
            this.btn_toggle_direction = new System.Windows.Forms.Button();
            this.btn_setright = new System.Windows.Forms.Button();
            this.btn_setcenter = new System.Windows.Forms.Button();
            this.btn_setleft = new System.Windows.Forms.Button();
            this.label_positioning = new System.Windows.Forms.Label();
            this.picker_textcolor = new System.Windows.Forms.ColorDialog();
            this.picker_backcolor = new System.Windows.Forms.ColorDialog();
            this.label_color = new System.Windows.Forms.Label();
            this.button_textcolor = new System.Windows.Forms.Button();
            this.button_backcolor = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.slider_fontsize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.slider_opacity)).BeginInit();
            this.SuspendLayout();
            // 
            // label_keyhistory
            // 
            this.label_keyhistory.AutoSize = true;
            this.label_keyhistory.Location = new System.Drawing.Point(12, 9);
            this.label_keyhistory.Name = "label_keyhistory";
            this.label_keyhistory.Size = new System.Drawing.Size(29, 13);
            this.label_keyhistory.TabIndex = 0;
            this.label_keyhistory.Text = "label";
            // 
            // slider_fontsize
            // 
            this.slider_fontsize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.slider_fontsize.Location = new System.Drawing.Point(155, 20);
            this.slider_fontsize.Maximum = 160;
            this.slider_fontsize.Minimum = 100;
            this.slider_fontsize.Name = "slider_fontsize";
            this.slider_fontsize.Size = new System.Drawing.Size(104, 45);
            this.slider_fontsize.TabIndex = 1;
            this.slider_fontsize.TickStyle = System.Windows.Forms.TickStyle.None;
            this.slider_fontsize.Value = 140;
            this.slider_fontsize.Scroll += new System.EventHandler(this.slider_fontsize_Scroll);
            // 
            // label_fontsize
            // 
            this.label_fontsize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_fontsize.AutoSize = true;
            this.label_fontsize.ForeColor = System.Drawing.Color.White;
            this.label_fontsize.Location = new System.Drawing.Point(152, 8);
            this.label_fontsize.Name = "label_fontsize";
            this.label_fontsize.Size = new System.Drawing.Size(46, 13);
            this.label_fontsize.TabIndex = 2;
            this.label_fontsize.Text = "Fontsize";
            // 
            // label_opacity
            // 
            this.label_opacity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_opacity.AutoSize = true;
            this.label_opacity.ForeColor = System.Drawing.Color.White;
            this.label_opacity.Location = new System.Drawing.Point(152, 68);
            this.label_opacity.Name = "label_opacity";
            this.label_opacity.Size = new System.Drawing.Size(43, 13);
            this.label_opacity.TabIndex = 3;
            this.label_opacity.Text = "Opacity";
            // 
            // slider_opacity
            // 
            this.slider_opacity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.slider_opacity.Location = new System.Drawing.Point(155, 84);
            this.slider_opacity.Maximum = 100;
            this.slider_opacity.Minimum = 10;
            this.slider_opacity.Name = "slider_opacity";
            this.slider_opacity.Size = new System.Drawing.Size(104, 45);
            this.slider_opacity.TabIndex = 4;
            this.slider_opacity.TickStyle = System.Windows.Forms.TickStyle.None;
            this.slider_opacity.Value = 80;
            this.slider_opacity.Scroll += new System.EventHandler(this.slider_opacity_Scroll);
            // 
            // btn_toggle_direction
            // 
            this.btn_toggle_direction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_toggle_direction.Location = new System.Drawing.Point(71, 29);
            this.btn_toggle_direction.Name = "btn_toggle_direction";
            this.btn_toggle_direction.Size = new System.Drawing.Size(75, 23);
            this.btn_toggle_direction.TabIndex = 5;
            this.btn_toggle_direction.Text = "From Top";
            this.btn_toggle_direction.UseVisualStyleBackColor = true;
            this.btn_toggle_direction.Click += new System.EventHandler(this.btn_toggle_direction_Click);
            // 
            // btn_setright
            // 
            this.btn_setright.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_setright.Location = new System.Drawing.Point(128, 58);
            this.btn_setright.Name = "btn_setright";
            this.btn_setright.Size = new System.Drawing.Size(21, 23);
            this.btn_setright.TabIndex = 6;
            this.btn_setright.Text = "R";
            this.btn_setright.UseVisualStyleBackColor = true;
            this.btn_setright.Click += new System.EventHandler(this.btn_setright_Click);
            // 
            // btn_setcenter
            // 
            this.btn_setcenter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_setcenter.Location = new System.Drawing.Point(100, 58);
            this.btn_setcenter.Name = "btn_setcenter";
            this.btn_setcenter.Size = new System.Drawing.Size(22, 23);
            this.btn_setcenter.TabIndex = 7;
            this.btn_setcenter.Text = "C";
            this.btn_setcenter.UseVisualStyleBackColor = true;
            this.btn_setcenter.Click += new System.EventHandler(this.btn_setcenter_Click);
            // 
            // btn_setleft
            // 
            this.btn_setleft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_setleft.Location = new System.Drawing.Point(71, 58);
            this.btn_setleft.Name = "btn_setleft";
            this.btn_setleft.Size = new System.Drawing.Size(23, 23);
            this.btn_setleft.TabIndex = 8;
            this.btn_setleft.Text = "L";
            this.btn_setleft.UseVisualStyleBackColor = true;
            this.btn_setleft.Click += new System.EventHandler(this.btn_setleft_Click);
            // 
            // label_positioning
            // 
            this.label_positioning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_positioning.AutoSize = true;
            this.label_positioning.ForeColor = System.Drawing.Color.White;
            this.label_positioning.Location = new System.Drawing.Point(68, 9);
            this.label_positioning.Name = "label_positioning";
            this.label_positioning.Size = new System.Drawing.Size(82, 13);
            this.label_positioning.TabIndex = 9;
            this.label_positioning.Text = "Text Positioning";
            // 
            // label_color
            // 
            this.label_color.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_color.AutoSize = true;
            this.label_color.ForeColor = System.Drawing.Color.White;
            this.label_color.Location = new System.Drawing.Point(67, 93);
            this.label_color.Name = "label_color";
            this.label_color.Size = new System.Drawing.Size(31, 13);
            this.label_color.TabIndex = 10;
            this.label_color.Text = "Color";
            // 
            // button_textcolor
            // 
            this.button_textcolor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_textcolor.Location = new System.Drawing.Point(104, 93);
            this.button_textcolor.Name = "button_textcolor";
            this.button_textcolor.Size = new System.Drawing.Size(46, 23);
            this.button_textcolor.TabIndex = 11;
            this.button_textcolor.Text = "Text";
            this.button_textcolor.UseVisualStyleBackColor = true;
            this.button_textcolor.Click += new System.EventHandler(this.button_textcolor_Click);
            // 
            // button_backcolor
            // 
            this.button_backcolor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_backcolor.Location = new System.Drawing.Point(70, 122);
            this.button_backcolor.Name = "button_backcolor";
            this.button_backcolor.Size = new System.Drawing.Size(80, 23);
            this.button_backcolor.TabIndex = 12;
            this.button_backcolor.Text = "Background";
            this.button_backcolor.UseVisualStyleBackColor = true;
            this.button_backcolor.Click += new System.EventHandler(this.button_backcolor_Click);
            // 
            // KeystrokesDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.ClientSize = new System.Drawing.Size(268, 188);
            this.Controls.Add(this.button_backcolor);
            this.Controls.Add(this.button_textcolor);
            this.Controls.Add(this.label_color);
            this.Controls.Add(this.label_positioning);
            this.Controls.Add(this.btn_setleft);
            this.Controls.Add(this.btn_setcenter);
            this.Controls.Add(this.btn_setright);
            this.Controls.Add(this.btn_toggle_direction);
            this.Controls.Add(this.slider_opacity);
            this.Controls.Add(this.label_opacity);
            this.Controls.Add(this.label_fontsize);
            this.Controls.Add(this.slider_fontsize);
            this.Controls.Add(this.label_keyhistory);
            this.Name = "KeystrokesDisplay";
            this.ShowInTaskbar = false;
            this.Text = "Win8KeystrokesForScreencasts";
            ((System.ComponentModel.ISupportInitialize)(this.slider_fontsize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.slider_opacity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_keyhistory;
        private System.Windows.Forms.TrackBar slider_fontsize;
        private System.Windows.Forms.Label label_fontsize;
        private System.Windows.Forms.Label label_opacity;
        private System.Windows.Forms.TrackBar slider_opacity;
        private System.Windows.Forms.Button btn_toggle_direction;
        private System.Windows.Forms.Button btn_setright;
        private System.Windows.Forms.Button btn_setcenter;
        private System.Windows.Forms.Button btn_setleft;
        private System.Windows.Forms.Label label_positioning;
        private System.Windows.Forms.ColorDialog picker_textcolor;
        private System.Windows.Forms.ColorDialog picker_backcolor;
        private System.Windows.Forms.Label label_color;
        private System.Windows.Forms.Button button_textcolor;
        private System.Windows.Forms.Button button_backcolor;
    }
}

