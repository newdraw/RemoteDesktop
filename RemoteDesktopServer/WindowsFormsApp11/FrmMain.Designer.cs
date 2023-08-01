namespace WindowsFormsApp11
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.txtIP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.trackDitherLevels = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cboDitherType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.trackKeepLevels = new System.Windows.Forms.TrackBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.picHistogram = new System.Windows.Forms.PictureBox();
            this.chkShowInfo = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackDitherLevels)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackKeepLevels)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picHistogram)).BeginInit();
            this.SuspendLayout();
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(102, 47);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(330, 21);
            this.txtIP.TabIndex = 0;
            this.txtIP.Text = "192.168.123.172";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(69, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "IP";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(456, 45);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "开始投屏";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // trackDitherLevels
            // 
            this.trackDitherLevels.Location = new System.Drawing.Point(137, 137);
            this.trackDitherLevels.Maximum = 64;
            this.trackDitherLevels.Minimum = 1;
            this.trackDitherLevels.Name = "trackDitherLevels";
            this.trackDitherLevels.Size = new System.Drawing.Size(394, 45);
            this.trackDitherLevels.TabIndex = 3;
            this.trackDitherLevels.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackDitherLevels.Value = 7;
            this.trackDitherLevels.Scroll += new System.EventHandler(this.trackDitherLevels_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(69, 141);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "仿色等级";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(69, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "仿色方法";
            // 
            // cboDitherType
            // 
            this.cboDitherType.DisplayMember = "图案";
            this.cboDitherType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDitherType.FormattingEnabled = true;
            this.cboDitherType.Items.AddRange(new object[] {
            "图案",
            "杂色",
            "无"});
            this.cboDitherType.Location = new System.Drawing.Point(142, 101);
            this.cboDitherType.Name = "cboDitherType";
            this.cboDitherType.Size = new System.Drawing.Size(204, 20);
            this.cboDitherType.TabIndex = 7;
            this.cboDitherType.ValueMember = "图案";
            this.cboDitherType.SelectedIndexChanged += new System.EventHandler(this.cboDitherType_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(69, 177);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "保留区间";
            // 
            // trackKeepLevels
            // 
            this.trackKeepLevels.Location = new System.Drawing.Point(137, 173);
            this.trackKeepLevels.Maximum = 100;
            this.trackKeepLevels.Minimum = 10;
            this.trackKeepLevels.Name = "trackKeepLevels";
            this.trackKeepLevels.Size = new System.Drawing.Size(394, 45);
            this.trackKeepLevels.TabIndex = 8;
            this.trackKeepLevels.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackKeepLevels.Value = 90;
            this.trackKeepLevels.Scroll += new System.EventHandler(this.trackKeepLevels_Scroll);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.picHistogram);
            this.groupBox1.Location = new System.Drawing.Point(71, 243);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(460, 237);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "直方图";
            // 
            // picHistogram
            // 
            this.picHistogram.Location = new System.Drawing.Point(7, 21);
            this.picHistogram.Name = "picHistogram";
            this.picHistogram.Size = new System.Drawing.Size(354, 210);
            this.picHistogram.TabIndex = 0;
            this.picHistogram.TabStop = false;
            // 
            // chkShowInfo
            // 
            this.chkShowInfo.AutoSize = true;
            this.chkShowInfo.Location = new System.Drawing.Point(71, 210);
            this.chkShowInfo.Name = "chkShowInfo";
            this.chkShowInfo.Size = new System.Drawing.Size(108, 16);
            this.chkShowInfo.TabIndex = 11;
            this.chkShowInfo.Text = "显示帧数和流量";
            this.chkShowInfo.UseVisualStyleBackColor = true;
            this.chkShowInfo.CheckedChanged += new System.EventHandler(this.chkShowInfo_CheckedChanged);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 506);
            this.Controls.Add(this.chkShowInfo);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.trackKeepLevels);
            this.Controls.Add(this.cboDitherType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.trackDitherLevels);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtIP);
            this.Name = "FrmMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackDitherLevels)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackKeepLevels)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picHistogram)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TrackBar trackDitherLevels;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboDitherType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar trackKeepLevels;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox picHistogram;
        private System.Windows.Forms.CheckBox chkShowInfo;
    }
}

