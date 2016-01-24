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
            this.pb_left = new System.Windows.Forms.PictureBox();
            this.pb_wheel = new System.Windows.Forms.PictureBox();
            this.pb_right = new System.Windows.Forms.PictureBox();
            this.pb_left_double = new System.Windows.Forms.PictureBox();
            this.pb_right_double = new System.Windows.Forms.PictureBox();
            this.pb_middle = new System.Windows.Forms.PictureBox();
            this.panel_mouse = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pb_left)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_wheel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_right)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_left_double)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_right_double)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_middle)).BeginInit();
            this.panel_mouse.SuspendLayout();
            this.SuspendLayout();
            // 
            // pb_left
            // 
            this.pb_left.BackColor = System.Drawing.Color.Transparent;
            this.pb_left.Location = new System.Drawing.Point(3, 20);
            this.pb_left.Name = "pb_left";
            this.pb_left.Size = new System.Drawing.Size(62, 62);
            this.pb_left.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pb_left.TabIndex = 1;
            this.pb_left.TabStop = false;
            this.pb_left.Visible = false;
            // 
            // pb_wheel
            // 
            this.pb_wheel.BackColor = System.Drawing.Color.Transparent;
            this.pb_wheel.Location = new System.Drawing.Point(71, 88);
            this.pb_wheel.Name = "pb_wheel";
            this.pb_wheel.Size = new System.Drawing.Size(62, 62);
            this.pb_wheel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pb_wheel.TabIndex = 2;
            this.pb_wheel.TabStop = false;
            this.pb_wheel.Visible = false;
            // 
            // pb_right
            // 
            this.pb_right.BackColor = System.Drawing.Color.Transparent;
            this.pb_right.Location = new System.Drawing.Point(151, 20);
            this.pb_right.Name = "pb_right";
            this.pb_right.Size = new System.Drawing.Size(62, 62);
            this.pb_right.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pb_right.TabIndex = 3;
            this.pb_right.TabStop = false;
            this.pb_right.Visible = false;
            // 
            // pb_left_double
            // 
            this.pb_left_double.BackColor = System.Drawing.Color.Transparent;
            this.pb_left_double.Location = new System.Drawing.Point(3, 88);
            this.pb_left_double.Name = "pb_left_double";
            this.pb_left_double.Size = new System.Drawing.Size(62, 62);
            this.pb_left_double.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pb_left_double.TabIndex = 4;
            this.pb_left_double.TabStop = false;
            this.pb_left_double.Visible = false;
            // 
            // pb_right_double
            // 
            this.pb_right_double.BackColor = System.Drawing.Color.Transparent;
            this.pb_right_double.Location = new System.Drawing.Point(151, 88);
            this.pb_right_double.Name = "pb_right_double";
            this.pb_right_double.Size = new System.Drawing.Size(62, 62);
            this.pb_right_double.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pb_right_double.TabIndex = 5;
            this.pb_right_double.TabStop = false;
            this.pb_right_double.Visible = false;
            // 
            // pb_middle
            // 
            this.pb_middle.BackColor = System.Drawing.Color.Transparent;
            this.pb_middle.Location = new System.Drawing.Point(83, 20);
            this.pb_middle.Name = "pb_middle";
            this.pb_middle.Size = new System.Drawing.Size(62, 62);
            this.pb_middle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pb_middle.TabIndex = 6;
            this.pb_middle.TabStop = false;
            this.pb_middle.Visible = false;
            // 
            // panel_mouse
            // 
            this.panel_mouse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel_mouse.Controls.Add(this.pb_left);
            this.panel_mouse.Controls.Add(this.pb_middle);
            this.panel_mouse.Controls.Add(this.pb_wheel);
            this.panel_mouse.Controls.Add(this.pb_right_double);
            this.panel_mouse.Controls.Add(this.pb_right);
            this.panel_mouse.Controls.Add(this.pb_left_double);
            this.panel_mouse.Location = new System.Drawing.Point(12, 26);
            this.panel_mouse.Name = "panel_mouse";
            this.panel_mouse.Size = new System.Drawing.Size(260, 189);
            this.panel_mouse.TabIndex = 7;
            // 
            // ButtonIndicator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.panel_mouse);
            this.Name = "ButtonIndicator";
            this.Text = "CursorIndicator";
            this.Load += new System.EventHandler(this.ButtonIndicator_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pb_left)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_wheel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_right)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_left_double)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_right_double)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_middle)).EndInit();
            this.panel_mouse.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pb_left;
        private System.Windows.Forms.PictureBox pb_wheel;
        private System.Windows.Forms.PictureBox pb_right;
        private System.Windows.Forms.PictureBox pb_left_double;
        private System.Windows.Forms.PictureBox pb_right_double;
        private System.Windows.Forms.PictureBox pb_middle;
        private System.Windows.Forms.Panel panel_mouse;



    }
}