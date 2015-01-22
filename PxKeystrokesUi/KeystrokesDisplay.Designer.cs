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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeystrokesDisplay));
            this.bn_close = new System.Windows.Forms.Button();
            this.bn_resize = new System.Windows.Forms.Button();
            this.bn_settings = new System.Windows.Forms.Button();
            this.panel_textposhelper = new System.Windows.Forms.Panel();
            this.bn_panel_resize = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.notifyIcon_main = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifyIcon_contextmenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportErrorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contributeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpHoldCtrlAltShiftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel_textposhelper.SuspendLayout();
            this.notifyIcon_contextmenu.SuspendLayout();
            this.SuspendLayout();
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
            this.bn_resize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bn_resize_MouseDown);
            this.bn_resize.MouseMove += new System.Windows.Forms.MouseEventHandler(this.bn_resize_MouseMove);
            this.bn_resize.MouseUp += new System.Windows.Forms.MouseEventHandler(this.moveresize_MouseUp);
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
            // panel_textposhelper
            // 
            this.panel_textposhelper.BackColor = System.Drawing.Color.Gray;
            this.panel_textposhelper.Controls.Add(this.bn_panel_resize);
            this.panel_textposhelper.Controls.Add(this.label2);
            this.panel_textposhelper.Controls.Add(this.label1);
            this.panel_textposhelper.Location = new System.Drawing.Point(26, 43);
            this.panel_textposhelper.Name = "panel_textposhelper";
            this.panel_textposhelper.Size = new System.Drawing.Size(200, 100);
            this.panel_textposhelper.TabIndex = 20;
            this.panel_textposhelper.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel_textposhelper_MouseDown);
            this.panel_textposhelper.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel_textposhelper_MouseMove);
            this.panel_textposhelper.MouseUp += new System.Windows.Forms.MouseEventHandler(this.moveresize_MouseUp);
            // 
            // bn_panel_resize
            // 
            this.bn_panel_resize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bn_panel_resize.BackColor = System.Drawing.Color.DarkGray;
            this.bn_panel_resize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bn_panel_resize.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bn_panel_resize.Location = new System.Drawing.Point(172, 69);
            this.bn_panel_resize.Name = "bn_panel_resize";
            this.bn_panel_resize.Size = new System.Drawing.Size(38, 37);
            this.bn_panel_resize.TabIndex = 21;
            this.bn_panel_resize.Text = "⇲";
            this.bn_panel_resize.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.bn_panel_resize.UseVisualStyleBackColor = false;
            this.bn_panel_resize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bn_panel_resize_MouseDown);
            this.bn_panel_resize.MouseMove += new System.Windows.Forms.MouseEventHandler(this.bn_panel_resize_MouseMove);
            this.bn_panel_resize.MouseUp += new System.Windows.Forms.MouseEventHandler(this.moveresize_MouseUp);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "to adjust text position";
            this.label2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel_textposhelper_MouseDown);
            this.label2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel_textposhelper_MouseMove);
            this.label2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.moveresize_MouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Move and resize this";
            this.label1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel_textposhelper_MouseDown);
            this.label1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel_textposhelper_MouseMove);
            this.label1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.moveresize_MouseUp);
            // 
            // notifyIcon_main
            // 
            this.notifyIcon_main.BalloonTipText = "xfxfn";
            this.notifyIcon_main.BalloonTipTitle = "xfgn";
            this.notifyIcon_main.ContextMenuStrip = this.notifyIcon_contextmenu;
            this.notifyIcon_main.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon_main.Icon")));
            this.notifyIcon_main.Text = "PyKeystrokesForScreencasts";
            this.notifyIcon_main.Visible = true;
            // 
            // notifyIcon_contextmenu
            // 
            this.notifyIcon_contextmenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpHoldCtrlAltShiftToolStripMenuItem,
            this.reportErrorsToolStripMenuItem,
            this.contributeToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.notifyIcon_contextmenu.Name = "notifyIcon_contextmenu";
            this.notifyIcon_contextmenu.Size = new System.Drawing.Size(221, 114);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // reportErrorsToolStripMenuItem
            // 
            this.reportErrorsToolStripMenuItem.Name = "reportErrorsToolStripMenuItem";
            this.reportErrorsToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.reportErrorsToolStripMenuItem.Text = "Report Errors";
            this.reportErrorsToolStripMenuItem.Click += new System.EventHandler(this.reportErrorsToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // contributeToolStripMenuItem
            // 
            this.contributeToolStripMenuItem.Name = "contributeToolStripMenuItem";
            this.contributeToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.contributeToolStripMenuItem.Text = "Contribute";
            this.contributeToolStripMenuItem.Click += new System.EventHandler(this.contributeToolStripMenuItem_Click);
            // 
            // helpHoldCtrlAltShiftToolStripMenuItem
            // 
            this.helpHoldCtrlAltShiftToolStripMenuItem.Name = "helpHoldCtrlAltShiftToolStripMenuItem";
            this.helpHoldCtrlAltShiftToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.helpHoldCtrlAltShiftToolStripMenuItem.Text = "Help: Hold Ctrl + Alt + Shift";
            // 
            // KeystrokesDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(268, 188);
            this.Controls.Add(this.panel_textposhelper);
            this.Controls.Add(this.bn_settings);
            this.Controls.Add(this.bn_resize);
            this.Controls.Add(this.bn_close);
            this.Location = new System.Drawing.Point(20, 0);
            this.Name = "KeystrokesDisplay";
            this.ShowInTaskbar = false;
            this.Text = "Win8KeystrokesForScreencasts";
            this.Shown += new System.EventHandler(this.KeystrokesDisplay_Shown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bn_move_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.bn_move_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.moveresize_MouseUp);
            this.panel_textposhelper.ResumeLayout(false);
            this.panel_textposhelper.PerformLayout();
            this.notifyIcon_contextmenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bn_close;
        private System.Windows.Forms.Button bn_resize;
        private System.Windows.Forms.Button bn_settings;
        private System.Windows.Forms.Panel panel_textposhelper;
        private System.Windows.Forms.Button bn_panel_resize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NotifyIcon notifyIcon_main;
        private System.Windows.Forms.ContextMenuStrip notifyIcon_contextmenu;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportErrorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contributeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpHoldCtrlAltShiftToolStripMenuItem;
    }
}

