namespace vs_h
{
    partial class FormHSV
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
            this.tbSMin = new System.Windows.Forms.TrackBar();
            this.hWindowControl1 = new HalconDotNet.HWindowControl();
            this.tbSMax = new System.Windows.Forms.TrackBar();
            this.tbHMin = new System.Windows.Forms.TrackBar();
            this.tbHMax = new System.Windows.Forms.TrackBar();
            this.btnImgTest = new System.Windows.Forms.Button();
            this.txtHsvTestScore = new System.Windows.Forms.TextBox();
            this.btnCheckTestHSV = new System.Windows.Forms.Button();
            this.txtHsvTestScoreMin = new System.Windows.Forms.TextBox();
            this.txtHsvTestScoreMax = new System.Windows.Forms.TextBox();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSaveHsv = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.tbSMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbHMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbHMax)).BeginInit();
            this.SuspendLayout();
            // 
            // tbSMin
            // 
            this.tbSMin.Location = new System.Drawing.Point(26, 462);
            this.tbSMin.Maximum = 50;
            this.tbSMin.Name = "tbSMin";
            this.tbSMin.Size = new System.Drawing.Size(357, 45);
            this.tbSMin.TabIndex = 0;
            // 
            // hWindowControl1
            // 
            this.hWindowControl1.BackColor = System.Drawing.Color.Black;
            this.hWindowControl1.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl1.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl1.Location = new System.Drawing.Point(56, 12);
            this.hWindowControl1.Name = "hWindowControl1";
            this.hWindowControl1.Size = new System.Drawing.Size(658, 361);
            this.hWindowControl1.TabIndex = 1;
            this.hWindowControl1.WindowSize = new System.Drawing.Size(658, 361);
            this.hWindowControl1.HMouseMove += new HalconDotNet.HMouseEventHandler(this.hWindowControl1_HMouseMove_1);
            // 
            // tbSMax
            // 
            this.tbSMax.Location = new System.Drawing.Point(26, 532);
            this.tbSMax.Maximum = 50;
            this.tbSMax.Name = "tbSMax";
            this.tbSMax.Size = new System.Drawing.Size(357, 45);
            this.tbSMax.TabIndex = 2;
            // 
            // tbHMin
            // 
            this.tbHMin.Location = new System.Drawing.Point(424, 462);
            this.tbHMin.Maximum = 50;
            this.tbHMin.Name = "tbHMin";
            this.tbHMin.Size = new System.Drawing.Size(398, 45);
            this.tbHMin.TabIndex = 3;
            // 
            // tbHMax
            // 
            this.tbHMax.Location = new System.Drawing.Point(424, 532);
            this.tbHMax.Maximum = 50;
            this.tbHMax.Name = "tbHMax";
            this.tbHMax.Size = new System.Drawing.Size(398, 45);
            this.tbHMax.TabIndex = 4;
            // 
            // btnImgTest
            // 
            this.btnImgTest.Location = new System.Drawing.Point(747, 33);
            this.btnImgTest.Name = "btnImgTest";
            this.btnImgTest.Size = new System.Drawing.Size(75, 23);
            this.btnImgTest.TabIndex = 5;
            this.btnImgTest.Text = "button1";
            this.btnImgTest.UseVisualStyleBackColor = true;
            this.btnImgTest.Click += new System.EventHandler(this.btnImgTest_Click);
            // 
            // txtHsvTestScore
            // 
            this.txtHsvTestScore.Location = new System.Drawing.Point(326, 402);
            this.txtHsvTestScore.Name = "txtHsvTestScore";
            this.txtHsvTestScore.Size = new System.Drawing.Size(100, 20);
            this.txtHsvTestScore.TabIndex = 6;
            // 
            // btnCheckTestHSV
            // 
            this.btnCheckTestHSV.Location = new System.Drawing.Point(747, 95);
            this.btnCheckTestHSV.Name = "btnCheckTestHSV";
            this.btnCheckTestHSV.Size = new System.Drawing.Size(75, 23);
            this.btnCheckTestHSV.TabIndex = 7;
            this.btnCheckTestHSV.Text = "button1";
            this.btnCheckTestHSV.UseVisualStyleBackColor = true;
            this.btnCheckTestHSV.Click += new System.EventHandler(this.btnCheckTestHSV_Click);
            // 
            // txtHsvTestScoreMin
            // 
            this.txtHsvTestScoreMin.Location = new System.Drawing.Point(198, 402);
            this.txtHsvTestScoreMin.Name = "txtHsvTestScoreMin";
            this.txtHsvTestScoreMin.Size = new System.Drawing.Size(100, 20);
            this.txtHsvTestScoreMin.TabIndex = 8;
            // 
            // txtHsvTestScoreMax
            // 
            this.txtHsvTestScoreMax.Location = new System.Drawing.Point(451, 402);
            this.txtHsvTestScoreMax.Name = "txtHsvTestScoreMax";
            this.txtHsvTestScoreMax.Size = new System.Drawing.Size(100, 20);
            this.txtHsvTestScoreMax.TabIndex = 9;
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(732, 191);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(100, 58);
            this.txtResult.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 443);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Smin";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(430, 516);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Hmax";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 516);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Smax(full)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(430, 446);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Hmin";
            // 
            // btnSaveHsv
            // 
            this.btnSaveHsv.Location = new System.Drawing.Point(688, 402);
            this.btnSaveHsv.Name = "btnSaveHsv";
            this.btnSaveHsv.Size = new System.Drawing.Size(75, 23);
            this.btnSaveHsv.TabIndex = 15;
            this.btnSaveHsv.Text = "Save";
            this.btnSaveHsv.UseVisualStyleBackColor = true;
            this.btnSaveHsv.Click += new System.EventHandler(this.btnSaveHsv_Click);
            // 
            // FormHSV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 615);
            this.Controls.Add(this.btnSaveHsv);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.txtHsvTestScoreMax);
            this.Controls.Add(this.txtHsvTestScoreMin);
            this.Controls.Add(this.btnCheckTestHSV);
            this.Controls.Add(this.txtHsvTestScore);
            this.Controls.Add(this.btnImgTest);
            this.Controls.Add(this.tbHMax);
            this.Controls.Add(this.tbHMin);
            this.Controls.Add(this.tbSMax);
            this.Controls.Add(this.hWindowControl1);
            this.Controls.Add(this.tbSMin);
            this.Name = "FormHSV";
            this.Text = "FormHSV";
            this.Load += new System.EventHandler(this.FormHSV_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tbSMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbHMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbHMax)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar tbSMin;
        private HalconDotNet.HWindowControl hWindowControl1;
        private System.Windows.Forms.TrackBar tbSMax;
        private System.Windows.Forms.TrackBar tbHMin;
        private System.Windows.Forms.TrackBar tbHMax;
        private System.Windows.Forms.Button btnImgTest;
        private System.Windows.Forms.TextBox txtHsvTestScore;
        private System.Windows.Forms.Button btnCheckTestHSV;
        private System.Windows.Forms.TextBox txtHsvTestScoreMin;
        private System.Windows.Forms.TextBox txtHsvTestScoreMax;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSaveHsv;
    }
}