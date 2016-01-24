namespace PxKeystrokesUi
{
    partial class Settings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.button_backcolor = new System.Windows.Forms.Button();
            this.button_textcolor = new System.Windows.Forms.Button();
            this.slider_opacity = new System.Windows.Forms.TrackBar();
            this.label_opacity = new System.Windows.Forms.Label();
            this.label_fontsize = new System.Windows.Forms.Label();
            this.slider_fontsize = new System.Windows.Forms.TrackBar();
            this.rb_align_left = new System.Windows.Forms.RadioButton();
            this.rb_align_center = new System.Windows.Forms.RadioButton();
            this.rb_align_right = new System.Windows.Forms.RadioButton();
            this.groupBox_text_alignment = new System.Windows.Forms.GroupBox();
            this.gb_textdir = new System.Windows.Forms.GroupBox();
            this.rb_td_up = new System.Windows.Forms.RadioButton();
            this.rb_td_down = new System.Windows.Forms.RadioButton();
            this.gb_style = new System.Windows.Forms.GroupBox();
            this.rb_style_slide = new System.Windows.Forms.RadioButton();
            this.rb_style_noani = new System.Windows.Forms.RadioButton();
            this.gb_text = new System.Windows.Forms.GroupBox();
            this.label_historycount = new System.Windows.Forms.Label();
            this.label_verticalDistance = new System.Windows.Forms.Label();
            this.nud_historycount = new System.Windows.Forms.NumericUpDown();
            this.nud_verticalDistance = new System.Windows.Forms.NumericUpDown();
            this.button_textfont = new System.Windows.Forms.Button();
            this.gb_background = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.picker_textcolor = new System.Windows.Forms.ColorDialog();
            this.picker_backcolor = new System.Windows.Forms.ColorDialog();
            this.fontDialog_text = new System.Windows.Forms.FontDialog();
            this.bn_reset_position = new System.Windows.Forms.Button();
            this.bn_reset_all = new System.Windows.Forms.Button();
            this.gb_mouse = new System.Windows.Forms.GroupBox();
            this.cb_cursorindicator = new System.Windows.Forms.CheckBox();
            this.label_ci_opacity = new System.Windows.Forms.Label();
            this.slider_ci_opacity = new System.Windows.Forms.TrackBar();
            this.slider_ci_size = new System.Windows.Forms.TrackBar();
            this.label_ci_size = new System.Windows.Forms.Label();
            this.button_ci_color = new System.Windows.Forms.Button();
            this.picker_ci_color = new System.Windows.Forms.ColorDialog();
            this.label_history_timeout = new System.Windows.Forms.Label();
            this.slider_history_timeout = new System.Windows.Forms.TrackBar();
            this.cb_enableHistoryTimeout = new System.Windows.Forms.CheckBox();
            this.label_timeout_display = new System.Windows.Forms.Label();
            this.gb_buttonindicator = new System.Windows.Forms.GroupBox();
            this.label_bi_angle = new System.Windows.Forms.Label();
            this.slider_bi_angle = new System.Windows.Forms.TrackBar();
            this.slider_bi_distance = new System.Windows.Forms.TrackBar();
            this.label_bi_distance = new System.Windows.Forms.Label();
            this.rb_bi_disable = new System.Windows.Forms.RadioButton();
            this.rb_bi_icon = new System.Windows.Forms.RadioButton();
            this.slider_bi_size = new System.Windows.Forms.TrackBar();
            this.label_bi_size = new System.Windows.Forms.Label();
            this.cb_backspace = new System.Windows.Forms.CheckBox();
            this.cb_bi_history = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.slider_opacity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.slider_fontsize)).BeginInit();
            this.groupBox_text_alignment.SuspendLayout();
            this.gb_textdir.SuspendLayout();
            this.gb_style.SuspendLayout();
            this.gb_text.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_historycount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_verticalDistance)).BeginInit();
            this.gb_background.SuspendLayout();
            this.gb_mouse.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.slider_ci_opacity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.slider_ci_size)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.slider_history_timeout)).BeginInit();
            this.gb_buttonindicator.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.slider_bi_angle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.slider_bi_distance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.slider_bi_size)).BeginInit();
            this.SuspendLayout();
            // 
            // button_backcolor
            // 
            this.button_backcolor.Location = new System.Drawing.Point(6, 19);
            this.button_backcolor.Name = "button_backcolor";
            this.button_backcolor.Size = new System.Drawing.Size(171, 23);
            this.button_backcolor.TabIndex = 25;
            this.button_backcolor.Text = "Background color";
            this.button_backcolor.UseVisualStyleBackColor = true;
            this.button_backcolor.Click += new System.EventHandler(this.button_backcolor_Click);
            // 
            // button_textcolor
            // 
            this.button_textcolor.Location = new System.Drawing.Point(6, 19);
            this.button_textcolor.Name = "button_textcolor";
            this.button_textcolor.Size = new System.Drawing.Size(80, 23);
            this.button_textcolor.TabIndex = 24;
            this.button_textcolor.Text = "Text color";
            this.button_textcolor.UseVisualStyleBackColor = true;
            this.button_textcolor.Click += new System.EventHandler(this.button_textcolor_Click);
            // 
            // slider_opacity
            // 
            this.slider_opacity.Location = new System.Drawing.Point(68, 55);
            this.slider_opacity.Maximum = 100;
            this.slider_opacity.Minimum = 10;
            this.slider_opacity.Name = "slider_opacity";
            this.slider_opacity.Size = new System.Drawing.Size(104, 45);
            this.slider_opacity.TabIndex = 17;
            this.slider_opacity.TickStyle = System.Windows.Forms.TickStyle.None;
            this.slider_opacity.Value = 80;
            this.slider_opacity.Scroll += new System.EventHandler(this.slider_opacity_Scroll);
            // 
            // label_opacity
            // 
            this.label_opacity.AutoSize = true;
            this.label_opacity.Location = new System.Drawing.Point(16, 55);
            this.label_opacity.Name = "label_opacity";
            this.label_opacity.Size = new System.Drawing.Size(43, 13);
            this.label_opacity.TabIndex = 16;
            this.label_opacity.Text = "Opacity";
            // 
            // label_fontsize
            // 
            this.label_fontsize.AutoSize = true;
            this.label_fontsize.Location = new System.Drawing.Point(16, 58);
            this.label_fontsize.Name = "label_fontsize";
            this.label_fontsize.Size = new System.Drawing.Size(46, 13);
            this.label_fontsize.TabIndex = 15;
            this.label_fontsize.Text = "Fontsize";
            // 
            // slider_fontsize
            // 
            this.slider_fontsize.Location = new System.Drawing.Point(68, 55);
            this.slider_fontsize.Maximum = 3000;
            this.slider_fontsize.Minimum = 800;
            this.slider_fontsize.Name = "slider_fontsize";
            this.slider_fontsize.Size = new System.Drawing.Size(104, 45);
            this.slider_fontsize.TabIndex = 14;
            this.slider_fontsize.TickStyle = System.Windows.Forms.TickStyle.None;
            this.slider_fontsize.Value = 800;
            this.slider_fontsize.Scroll += new System.EventHandler(this.slider_fontsize_Scroll);
            // 
            // rb_align_left
            // 
            this.rb_align_left.AutoSize = true;
            this.rb_align_left.Location = new System.Drawing.Point(6, 19);
            this.rb_align_left.Name = "rb_align_left";
            this.rb_align_left.Size = new System.Drawing.Size(39, 17);
            this.rb_align_left.TabIndex = 26;
            this.rb_align_left.TabStop = true;
            this.rb_align_left.Text = "left";
            this.rb_align_left.UseVisualStyleBackColor = true;
            this.rb_align_left.CheckedChanged += new System.EventHandler(this.rb_align_left_CheckedChanged);
            // 
            // rb_align_center
            // 
            this.rb_align_center.AutoSize = true;
            this.rb_align_center.Location = new System.Drawing.Point(49, 42);
            this.rb_align_center.Name = "rb_align_center";
            this.rb_align_center.Size = new System.Drawing.Size(55, 17);
            this.rb_align_center.TabIndex = 27;
            this.rb_align_center.TabStop = true;
            this.rb_align_center.Text = "center";
            this.rb_align_center.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rb_align_center.UseVisualStyleBackColor = true;
            this.rb_align_center.CheckedChanged += new System.EventHandler(this.rb_align_center_CheckedChanged);
            // 
            // rb_align_right
            // 
            this.rb_align_right.AutoSize = true;
            this.rb_align_right.Location = new System.Drawing.Point(99, 65);
            this.rb_align_right.Name = "rb_align_right";
            this.rb_align_right.Size = new System.Drawing.Size(45, 17);
            this.rb_align_right.TabIndex = 28;
            this.rb_align_right.TabStop = true;
            this.rb_align_right.Text = "right";
            this.rb_align_right.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rb_align_right.UseVisualStyleBackColor = true;
            this.rb_align_right.CheckedChanged += new System.EventHandler(this.rb_align_right_CheckedChanged);
            // 
            // groupBox_text_alignment
            // 
            this.groupBox_text_alignment.Controls.Add(this.rb_align_left);
            this.groupBox_text_alignment.Controls.Add(this.rb_align_right);
            this.groupBox_text_alignment.Controls.Add(this.rb_align_center);
            this.groupBox_text_alignment.Location = new System.Drawing.Point(12, 12);
            this.groupBox_text_alignment.Name = "groupBox_text_alignment";
            this.groupBox_text_alignment.Size = new System.Drawing.Size(150, 89);
            this.groupBox_text_alignment.TabIndex = 29;
            this.groupBox_text_alignment.TabStop = false;
            this.groupBox_text_alignment.Text = "Text Alignment";
            // 
            // gb_textdir
            // 
            this.gb_textdir.Controls.Add(this.rb_td_up);
            this.gb_textdir.Controls.Add(this.rb_td_down);
            this.gb_textdir.Location = new System.Drawing.Point(13, 108);
            this.gb_textdir.Name = "gb_textdir";
            this.gb_textdir.Size = new System.Drawing.Size(149, 63);
            this.gb_textdir.TabIndex = 30;
            this.gb_textdir.TabStop = false;
            this.gb_textdir.Text = "Text Direction";
            // 
            // rb_td_up
            // 
            this.rb_td_up.AutoSize = true;
            this.rb_td_up.Location = new System.Drawing.Point(7, 39);
            this.rb_td_up.Name = "rb_td_up";
            this.rb_td_up.Size = new System.Drawing.Size(37, 17);
            this.rb_td_up.TabIndex = 1;
            this.rb_td_up.TabStop = true;
            this.rb_td_up.Text = "up";
            this.rb_td_up.UseVisualStyleBackColor = true;
            this.rb_td_up.CheckedChanged += new System.EventHandler(this.rb_td_up_CheckedChanged);
            // 
            // rb_td_down
            // 
            this.rb_td_down.AutoSize = true;
            this.rb_td_down.Location = new System.Drawing.Point(7, 16);
            this.rb_td_down.Name = "rb_td_down";
            this.rb_td_down.Size = new System.Drawing.Size(51, 17);
            this.rb_td_down.TabIndex = 0;
            this.rb_td_down.TabStop = true;
            this.rb_td_down.Text = "down";
            this.rb_td_down.UseVisualStyleBackColor = true;
            this.rb_td_down.CheckedChanged += new System.EventHandler(this.rb_td_down_CheckedChanged);
            // 
            // gb_style
            // 
            this.gb_style.Controls.Add(this.rb_style_slide);
            this.gb_style.Controls.Add(this.rb_style_noani);
            this.gb_style.Location = new System.Drawing.Point(12, 178);
            this.gb_style.Name = "gb_style";
            this.gb_style.Size = new System.Drawing.Size(150, 71);
            this.gb_style.TabIndex = 31;
            this.gb_style.TabStop = false;
            this.gb_style.Text = "Style";
            // 
            // rb_style_slide
            // 
            this.rb_style_slide.AutoSize = true;
            this.rb_style_slide.Location = new System.Drawing.Point(8, 42);
            this.rb_style_slide.Name = "rb_style_slide";
            this.rb_style_slide.Size = new System.Drawing.Size(46, 17);
            this.rb_style_slide.TabIndex = 3;
            this.rb_style_slide.TabStop = true;
            this.rb_style_slide.Text = "slide";
            this.rb_style_slide.UseVisualStyleBackColor = true;
            this.rb_style_slide.CheckedChanged += new System.EventHandler(this.rb_style_slide_CheckedChanged);
            // 
            // rb_style_noani
            // 
            this.rb_style_noani.AutoSize = true;
            this.rb_style_noani.Location = new System.Drawing.Point(8, 19);
            this.rb_style_noani.Name = "rb_style_noani";
            this.rb_style_noani.Size = new System.Drawing.Size(85, 17);
            this.rb_style_noani.TabIndex = 2;
            this.rb_style_noani.TabStop = true;
            this.rb_style_noani.Text = "no animation";
            this.rb_style_noani.UseVisualStyleBackColor = true;
            this.rb_style_noani.CheckedChanged += new System.EventHandler(this.rb_style_noani_CheckedChanged);
            // 
            // gb_text
            // 
            this.gb_text.Controls.Add(this.cb_backspace);
            this.gb_text.Controls.Add(this.label_timeout_display);
            this.gb_text.Controls.Add(this.cb_enableHistoryTimeout);
            this.gb_text.Controls.Add(this.slider_history_timeout);
            this.gb_text.Controls.Add(this.label_history_timeout);
            this.gb_text.Controls.Add(this.label_historycount);
            this.gb_text.Controls.Add(this.label_verticalDistance);
            this.gb_text.Controls.Add(this.nud_historycount);
            this.gb_text.Controls.Add(this.nud_verticalDistance);
            this.gb_text.Controls.Add(this.button_textfont);
            this.gb_text.Controls.Add(this.button_textcolor);
            this.gb_text.Controls.Add(this.slider_fontsize);
            this.gb_text.Controls.Add(this.label_fontsize);
            this.gb_text.Location = new System.Drawing.Point(168, 12);
            this.gb_text.Name = "gb_text";
            this.gb_text.Size = new System.Drawing.Size(183, 274);
            this.gb_text.TabIndex = 32;
            this.gb_text.TabStop = false;
            this.gb_text.Text = "Text";
            // 
            // label_historycount
            // 
            this.label_historycount.AutoSize = true;
            this.label_historycount.Location = new System.Drawing.Point(16, 124);
            this.label_historycount.Name = "label_historycount";
            this.label_historycount.Size = new System.Drawing.Size(70, 13);
            this.label_historycount.TabIndex = 39;
            this.label_historycount.Text = "History Count";
            // 
            // label_verticalDistance
            // 
            this.label_verticalDistance.AutoSize = true;
            this.label_verticalDistance.Location = new System.Drawing.Point(16, 98);
            this.label_verticalDistance.Name = "label_verticalDistance";
            this.label_verticalDistance.Size = new System.Drawing.Size(87, 13);
            this.label_verticalDistance.TabIndex = 38;
            this.label_verticalDistance.Text = "Vertical Distance";
            // 
            // nud_historycount
            // 
            this.nud_historycount.Location = new System.Drawing.Point(109, 122);
            this.nud_historycount.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nud_historycount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_historycount.Name = "nud_historycount";
            this.nud_historycount.Size = new System.Drawing.Size(63, 20);
            this.nud_historycount.TabIndex = 37;
            this.nud_historycount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_historycount.ValueChanged += new System.EventHandler(this.nud_historycount_ValueChanged);
            // 
            // nud_verticalDistance
            // 
            this.nud_verticalDistance.Location = new System.Drawing.Point(109, 96);
            this.nud_verticalDistance.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nud_verticalDistance.Name = "nud_verticalDistance";
            this.nud_verticalDistance.Size = new System.Drawing.Size(63, 20);
            this.nud_verticalDistance.TabIndex = 36;
            this.nud_verticalDistance.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nud_verticalDistance.ValueChanged += new System.EventHandler(this.nud_verticalDistance_ValueChanged);
            // 
            // button_textfont
            // 
            this.button_textfont.Location = new System.Drawing.Point(92, 19);
            this.button_textfont.Name = "button_textfont";
            this.button_textfont.Size = new System.Drawing.Size(80, 23);
            this.button_textfont.TabIndex = 25;
            this.button_textfont.Text = "Text font";
            this.button_textfont.UseVisualStyleBackColor = true;
            this.button_textfont.Click += new System.EventHandler(this.button_textfont_Click);
            // 
            // gb_background
            // 
            this.gb_background.Controls.Add(this.button_backcolor);
            this.gb_background.Controls.Add(this.slider_opacity);
            this.gb_background.Controls.Add(this.label_opacity);
            this.gb_background.Location = new System.Drawing.Point(358, 177);
            this.gb_background.Name = "gb_background";
            this.gb_background.Size = new System.Drawing.Size(183, 109);
            this.gb_background.TabIndex = 33;
            this.gb_background.TabStop = false;
            this.gb_background.Text = "Background";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 292);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(224, 13);
            this.label1.TabIndex = 34;
            this.label1.Text = "Press Ctrl + Shift + Alt to reveal setting buttons";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(9, 319);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(241, 13);
            this.linkLabel1.TabIndex = 35;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Issues? Want to have a look at the source code?";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // bn_reset_position
            // 
            this.bn_reset_position.Location = new System.Drawing.Point(12, 256);
            this.bn_reset_position.Name = "bn_reset_position";
            this.bn_reset_position.Size = new System.Drawing.Size(86, 23);
            this.bn_reset_position.TabIndex = 36;
            this.bn_reset_position.Text = "Reset position";
            this.bn_reset_position.UseVisualStyleBackColor = true;
            this.bn_reset_position.Click += new System.EventHandler(this.bn_reset_position_Click);
            // 
            // bn_reset_all
            // 
            this.bn_reset_all.Location = new System.Drawing.Point(104, 256);
            this.bn_reset_all.Name = "bn_reset_all";
            this.bn_reset_all.Size = new System.Drawing.Size(58, 23);
            this.bn_reset_all.TabIndex = 37;
            this.bn_reset_all.Text = "Reset all";
            this.bn_reset_all.UseVisualStyleBackColor = true;
            this.bn_reset_all.Click += new System.EventHandler(this.bn_reset_all_Click);
            // 
            // gb_mouse
            // 
            this.gb_mouse.Controls.Add(this.button_ci_color);
            this.gb_mouse.Controls.Add(this.label_ci_size);
            this.gb_mouse.Controls.Add(this.slider_ci_size);
            this.gb_mouse.Controls.Add(this.slider_ci_opacity);
            this.gb_mouse.Controls.Add(this.label_ci_opacity);
            this.gb_mouse.Controls.Add(this.cb_cursorindicator);
            this.gb_mouse.Location = new System.Drawing.Point(358, 13);
            this.gb_mouse.Name = "gb_mouse";
            this.gb_mouse.Size = new System.Drawing.Size(183, 158);
            this.gb_mouse.TabIndex = 38;
            this.gb_mouse.TabStop = false;
            this.gb_mouse.Text = "Mouse";
            // 
            // cb_cursorindicator
            // 
            this.cb_cursorindicator.AutoSize = true;
            this.cb_cursorindicator.Location = new System.Drawing.Point(7, 18);
            this.cb_cursorindicator.Name = "cb_cursorindicator";
            this.cb_cursorindicator.Size = new System.Drawing.Size(135, 17);
            this.cb_cursorindicator.TabIndex = 0;
            this.cb_cursorindicator.Text = "Indicate cursor position";
            this.cb_cursorindicator.UseVisualStyleBackColor = true;
            this.cb_cursorindicator.CheckedChanged += new System.EventHandler(this.cb_cursorindicator_CheckedChanged);
            // 
            // label_ci_opacity
            // 
            this.label_ci_opacity.AutoSize = true;
            this.label_ci_opacity.Location = new System.Drawing.Point(20, 52);
            this.label_ci_opacity.Name = "label_ci_opacity";
            this.label_ci_opacity.Size = new System.Drawing.Size(43, 13);
            this.label_ci_opacity.TabIndex = 1;
            this.label_ci_opacity.Text = "Opacity";
            // 
            // slider_ci_opacity
            // 
            this.slider_ci_opacity.Location = new System.Drawing.Point(68, 46);
            this.slider_ci_opacity.Maximum = 100;
            this.slider_ci_opacity.Minimum = 10;
            this.slider_ci_opacity.Name = "slider_ci_opacity";
            this.slider_ci_opacity.Size = new System.Drawing.Size(104, 45);
            this.slider_ci_opacity.TabIndex = 40;
            this.slider_ci_opacity.TickStyle = System.Windows.Forms.TickStyle.None;
            this.slider_ci_opacity.Value = 80;
            this.slider_ci_opacity.Scroll += new System.EventHandler(this.slider_ci_opacity_Scroll);
            // 
            // slider_ci_size
            // 
            this.slider_ci_size.Location = new System.Drawing.Point(68, 85);
            this.slider_ci_size.Maximum = 200;
            this.slider_ci_size.Minimum = 4;
            this.slider_ci_size.Name = "slider_ci_size";
            this.slider_ci_size.Size = new System.Drawing.Size(104, 45);
            this.slider_ci_size.TabIndex = 41;
            this.slider_ci_size.TickStyle = System.Windows.Forms.TickStyle.None;
            this.slider_ci_size.Value = 100;
            this.slider_ci_size.Scroll += new System.EventHandler(this.slider_ci_size_Scroll);
            // 
            // label_ci_size
            // 
            this.label_ci_size.AutoSize = true;
            this.label_ci_size.Location = new System.Drawing.Point(20, 92);
            this.label_ci_size.Name = "label_ci_size";
            this.label_ci_size.Size = new System.Drawing.Size(27, 13);
            this.label_ci_size.TabIndex = 42;
            this.label_ci_size.Text = "Size";
            // 
            // button_ci_color
            // 
            this.button_ci_color.Location = new System.Drawing.Point(6, 118);
            this.button_ci_color.Name = "button_ci_color";
            this.button_ci_color.Size = new System.Drawing.Size(171, 23);
            this.button_ci_color.TabIndex = 26;
            this.button_ci_color.Text = "Color";
            this.button_ci_color.UseVisualStyleBackColor = true;
            this.button_ci_color.Click += new System.EventHandler(this.button_ci_color_Click);
            // 
            // label_history_timeout
            // 
            this.label_history_timeout.AutoSize = true;
            this.label_history_timeout.Location = new System.Drawing.Point(16, 183);
            this.label_history_timeout.Name = "label_history_timeout";
            this.label_history_timeout.Size = new System.Drawing.Size(45, 13);
            this.label_history_timeout.TabIndex = 40;
            this.label_history_timeout.Text = "Timeout";
            // 
            // slider_history_timeout
            // 
            this.slider_history_timeout.Location = new System.Drawing.Point(68, 180);
            this.slider_history_timeout.Maximum = 120000;
            this.slider_history_timeout.Minimum = 1000;
            this.slider_history_timeout.Name = "slider_history_timeout";
            this.slider_history_timeout.Size = new System.Drawing.Size(104, 45);
            this.slider_history_timeout.TabIndex = 41;
            this.slider_history_timeout.TickStyle = System.Windows.Forms.TickStyle.None;
            this.slider_history_timeout.Value = 1000;
            this.slider_history_timeout.Scroll += new System.EventHandler(this.slider_history_timeout_Scroll);
            // 
            // cb_enableHistoryTimeout
            // 
            this.cb_enableHistoryTimeout.AutoSize = true;
            this.cb_enableHistoryTimeout.Location = new System.Drawing.Point(6, 151);
            this.cb_enableHistoryTimeout.Name = "cb_enableHistoryTimeout";
            this.cb_enableHistoryTimeout.Size = new System.Drawing.Size(95, 17);
            this.cb_enableHistoryTimeout.TabIndex = 43;
            this.cb_enableHistoryTimeout.Text = "History timeout";
            this.cb_enableHistoryTimeout.UseVisualStyleBackColor = true;
            this.cb_enableHistoryTimeout.CheckedChanged += new System.EventHandler(this.cb_enableHistoryTimeout_CheckedChanged);
            // 
            // label_timeout_display
            // 
            this.label_timeout_display.AutoSize = true;
            this.label_timeout_display.Location = new System.Drawing.Point(118, 152);
            this.label_timeout_display.Name = "label_timeout_display";
            this.label_timeout_display.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label_timeout_display.Size = new System.Drawing.Size(18, 13);
            this.label_timeout_display.TabIndex = 44;
            this.label_timeout_display.Text = "5s";
            // 
            // gb_buttonindicator
            // 
            this.gb_buttonindicator.Controls.Add(this.cb_bi_history);
            this.gb_buttonindicator.Controls.Add(this.label_bi_size);
            this.gb_buttonindicator.Controls.Add(this.rb_bi_icon);
            this.gb_buttonindicator.Controls.Add(this.rb_bi_disable);
            this.gb_buttonindicator.Controls.Add(this.label_bi_angle);
            this.gb_buttonindicator.Controls.Add(this.slider_bi_angle);
            this.gb_buttonindicator.Controls.Add(this.slider_bi_distance);
            this.gb_buttonindicator.Controls.Add(this.label_bi_distance);
            this.gb_buttonindicator.Controls.Add(this.slider_bi_size);
            this.gb_buttonindicator.Location = new System.Drawing.Point(547, 13);
            this.gb_buttonindicator.Name = "gb_buttonindicator";
            this.gb_buttonindicator.Size = new System.Drawing.Size(183, 273);
            this.gb_buttonindicator.TabIndex = 43;
            this.gb_buttonindicator.TabStop = false;
            this.gb_buttonindicator.Text = "Mouse Clicks";
            // 
            // label_bi_angle
            // 
            this.label_bi_angle.AutoSize = true;
            this.label_bi_angle.Location = new System.Drawing.Point(20, 155);
            this.label_bi_angle.Name = "label_bi_angle";
            this.label_bi_angle.Size = new System.Drawing.Size(49, 13);
            this.label_bi_angle.TabIndex = 42;
            this.label_bi_angle.Text = "Direction";
            // 
            // slider_bi_angle
            // 
            this.slider_bi_angle.Location = new System.Drawing.Point(68, 148);
            this.slider_bi_angle.Maximum = 65;
            this.slider_bi_angle.Name = "slider_bi_angle";
            this.slider_bi_angle.Size = new System.Drawing.Size(104, 45);
            this.slider_bi_angle.TabIndex = 41;
            this.slider_bi_angle.TickStyle = System.Windows.Forms.TickStyle.None;
            this.slider_bi_angle.Value = 65;
            this.slider_bi_angle.Scroll += new System.EventHandler(this.slider_bi_angle_Scroll);
            // 
            // slider_bi_distance
            // 
            this.slider_bi_distance.Location = new System.Drawing.Point(68, 109);
            this.slider_bi_distance.Maximum = 100;
            this.slider_bi_distance.Minimum = 10;
            this.slider_bi_distance.Name = "slider_bi_distance";
            this.slider_bi_distance.Size = new System.Drawing.Size(104, 45);
            this.slider_bi_distance.TabIndex = 40;
            this.slider_bi_distance.TickStyle = System.Windows.Forms.TickStyle.None;
            this.slider_bi_distance.Value = 80;
            this.slider_bi_distance.Scroll += new System.EventHandler(this.slider_bi_distance_Scroll);
            // 
            // label_bi_distance
            // 
            this.label_bi_distance.AutoSize = true;
            this.label_bi_distance.Location = new System.Drawing.Point(20, 115);
            this.label_bi_distance.Name = "label_bi_distance";
            this.label_bi_distance.Size = new System.Drawing.Size(49, 13);
            this.label_bi_distance.TabIndex = 1;
            this.label_bi_distance.Text = "Distance";
            // 
            // rb_bi_disable
            // 
            this.rb_bi_disable.AutoSize = true;
            this.rb_bi_disable.Location = new System.Drawing.Point(6, 17);
            this.rb_bi_disable.Name = "rb_bi_disable";
            this.rb_bi_disable.Size = new System.Drawing.Size(127, 17);
            this.rb_bi_disable.TabIndex = 2;
            this.rb_bi_disable.TabStop = true;
            this.rb_bi_disable.Text = "No not indicate clicks";
            this.rb_bi_disable.UseVisualStyleBackColor = true;
            this.rb_bi_disable.CheckedChanged += new System.EventHandler(this.rb_bi_disable_CheckedChanged);
            // 
            // rb_bi_icon
            // 
            this.rb_bi_icon.AutoSize = true;
            this.rb_bi_icon.Location = new System.Drawing.Point(6, 40);
            this.rb_bi_icon.Name = "rb_bi_icon";
            this.rb_bi_icon.Size = new System.Drawing.Size(76, 17);
            this.rb_bi_icon.TabIndex = 43;
            this.rb_bi_icon.TabStop = true;
            this.rb_bi_icon.Text = "Show Icon";
            this.rb_bi_icon.UseVisualStyleBackColor = true;
            this.rb_bi_icon.CheckedChanged += new System.EventHandler(this.rb_bi_icon_CheckedChanged);
            // 
            // slider_bi_size
            // 
            this.slider_bi_size.Location = new System.Drawing.Point(68, 70);
            this.slider_bi_size.Maximum = 70;
            this.slider_bi_size.Minimum = 20;
            this.slider_bi_size.Name = "slider_bi_size";
            this.slider_bi_size.Size = new System.Drawing.Size(104, 45);
            this.slider_bi_size.TabIndex = 45;
            this.slider_bi_size.TickStyle = System.Windows.Forms.TickStyle.None;
            this.slider_bi_size.Value = 70;
            this.slider_bi_size.Scroll += new System.EventHandler(this.slider_bi_size_Scroll);
            // 
            // label_bi_size
            // 
            this.label_bi_size.AutoSize = true;
            this.label_bi_size.Location = new System.Drawing.Point(20, 76);
            this.label_bi_size.Name = "label_bi_size";
            this.label_bi_size.Size = new System.Drawing.Size(27, 13);
            this.label_bi_size.TabIndex = 44;
            this.label_bi_size.Text = "Size";
            // 
            // cb_backspace
            // 
            this.cb_backspace.AutoSize = true;
            this.cb_backspace.Location = new System.Drawing.Point(6, 216);
            this.cb_backspace.Name = "cb_backspace";
            this.cb_backspace.Size = new System.Drawing.Size(153, 17);
            this.cb_backspace.TabIndex = 45;
            this.cb_backspace.Text = "Backspace can delete text";
            this.cb_backspace.UseVisualStyleBackColor = true;
            this.cb_backspace.CheckedChanged += new System.EventHandler(this.cb_backspace_CheckedChanged);
            // 
            // cb_bi_history
            // 
            this.cb_bi_history.AutoSize = true;
            this.cb_bi_history.Enabled = false;
            this.cb_bi_history.Location = new System.Drawing.Point(6, 183);
            this.cb_bi_history.Name = "cb_bi_history";
            this.cb_bi_history.Size = new System.Drawing.Size(96, 17);
            this.cb_bi_history.TabIndex = 46;
            this.cb_bi_history.Text = "Add To History";
            this.cb_bi_history.UseVisualStyleBackColor = true;
            this.cb_bi_history.CheckedChanged += new System.EventHandler(this.cb_bi_history_CheckedChanged);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(737, 357);
            this.Controls.Add(this.gb_buttonindicator);
            this.Controls.Add(this.gb_mouse);
            this.Controls.Add(this.bn_reset_all);
            this.Controls.Add(this.bn_reset_position);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gb_background);
            this.Controls.Add(this.gb_text);
            this.Controls.Add(this.gb_style);
            this.Controls.Add(this.gb_textdir);
            this.Controls.Add(this.groupBox_text_alignment);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Settings";
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Settings_FormClosing);
            this.Load += new System.EventHandler(this.Settings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.slider_opacity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.slider_fontsize)).EndInit();
            this.groupBox_text_alignment.ResumeLayout(false);
            this.groupBox_text_alignment.PerformLayout();
            this.gb_textdir.ResumeLayout(false);
            this.gb_textdir.PerformLayout();
            this.gb_style.ResumeLayout(false);
            this.gb_style.PerformLayout();
            this.gb_text.ResumeLayout(false);
            this.gb_text.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_historycount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_verticalDistance)).EndInit();
            this.gb_background.ResumeLayout(false);
            this.gb_background.PerformLayout();
            this.gb_mouse.ResumeLayout(false);
            this.gb_mouse.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.slider_ci_opacity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.slider_ci_size)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.slider_history_timeout)).EndInit();
            this.gb_buttonindicator.ResumeLayout(false);
            this.gb_buttonindicator.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.slider_bi_angle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.slider_bi_distance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.slider_bi_size)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_backcolor;
        private System.Windows.Forms.Button button_textcolor;
        private System.Windows.Forms.TrackBar slider_opacity;
        private System.Windows.Forms.Label label_opacity;
        private System.Windows.Forms.Label label_fontsize;
        private System.Windows.Forms.TrackBar slider_fontsize;
        private System.Windows.Forms.RadioButton rb_align_left;
        private System.Windows.Forms.RadioButton rb_align_center;
        private System.Windows.Forms.RadioButton rb_align_right;
        private System.Windows.Forms.GroupBox groupBox_text_alignment;
        private System.Windows.Forms.GroupBox gb_textdir;
        private System.Windows.Forms.RadioButton rb_td_up;
        private System.Windows.Forms.RadioButton rb_td_down;
        private System.Windows.Forms.GroupBox gb_style;
        private System.Windows.Forms.RadioButton rb_style_slide;
        private System.Windows.Forms.RadioButton rb_style_noani;
        private System.Windows.Forms.GroupBox gb_text;
        private System.Windows.Forms.Button button_textfont;
        private System.Windows.Forms.GroupBox gb_background;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.ColorDialog picker_textcolor;
        private System.Windows.Forms.ColorDialog picker_backcolor;
        private System.Windows.Forms.FontDialog fontDialog_text;
        private System.Windows.Forms.Label label_historycount;
        private System.Windows.Forms.Label label_verticalDistance;
        private System.Windows.Forms.NumericUpDown nud_historycount;
        private System.Windows.Forms.NumericUpDown nud_verticalDistance;
        private System.Windows.Forms.Button bn_reset_position;
        private System.Windows.Forms.Button bn_reset_all;
        private System.Windows.Forms.GroupBox gb_mouse;
        private System.Windows.Forms.CheckBox cb_cursorindicator;
        private System.Windows.Forms.Button button_ci_color;
        private System.Windows.Forms.Label label_ci_size;
        private System.Windows.Forms.TrackBar slider_ci_size;
        private System.Windows.Forms.TrackBar slider_ci_opacity;
        private System.Windows.Forms.Label label_ci_opacity;
        private System.Windows.Forms.ColorDialog picker_ci_color;
        private System.Windows.Forms.Label label_timeout_display;
        private System.Windows.Forms.CheckBox cb_enableHistoryTimeout;
        private System.Windows.Forms.TrackBar slider_history_timeout;
        private System.Windows.Forms.Label label_history_timeout;
        private System.Windows.Forms.GroupBox gb_buttonindicator;
        private System.Windows.Forms.Label label_bi_size;
        private System.Windows.Forms.RadioButton rb_bi_icon;
        private System.Windows.Forms.RadioButton rb_bi_disable;
        private System.Windows.Forms.Label label_bi_angle;
        private System.Windows.Forms.TrackBar slider_bi_angle;
        private System.Windows.Forms.TrackBar slider_bi_distance;
        private System.Windows.Forms.Label label_bi_distance;
        private System.Windows.Forms.TrackBar slider_bi_size;
        private System.Windows.Forms.CheckBox cb_backspace;
        private System.Windows.Forms.CheckBox cb_bi_history;
    }
}