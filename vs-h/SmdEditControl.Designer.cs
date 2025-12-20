namespace vs_h
{
    partial class SmdEditControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkSMD_IsEnabled = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSMD_Algorithm = new System.Windows.Forms.ComboBox();
            this.txtSMD_ID = new System.Windows.Forms.TextBox();
            this.txtROI_Y = new System.Windows.Forms.TextBox();
            this.txtROI_Width = new System.Windows.Forms.TextBox();
            this.txtROI_X = new System.Windows.Forms.TextBox();
            this.txtROI_Height = new System.Windows.Forms.TextBox();
            this.lb2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSMD_Name = new System.Windows.Forms.TextBox();
            this.btnSettingAG = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkSMD_IsEnabled
            // 
            this.chkSMD_IsEnabled.AutoSize = true;
            this.chkSMD_IsEnabled.Location = new System.Drawing.Point(64, 73);
            this.chkSMD_IsEnabled.Name = "chkSMD_IsEnabled";
            this.chkSMD_IsEnabled.Size = new System.Drawing.Size(80, 17);
            this.chkSMD_IsEnabled.TabIndex = 2;
            this.chkSMD_IsEnabled.Text = "checkBox1";
            this.chkSMD_IsEnabled.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "label1";
            // 
            // cmbSMD_Algorithm
            // 
            this.cmbSMD_Algorithm.FormattingEnabled = true;
            this.cmbSMD_Algorithm.Items.AddRange(new object[] {
            "None",
            "QR",
            "HSV"});
            this.cmbSMD_Algorithm.Location = new System.Drawing.Point(64, 148);
            this.cmbSMD_Algorithm.Name = "cmbSMD_Algorithm";
            this.cmbSMD_Algorithm.Size = new System.Drawing.Size(121, 21);
            this.cmbSMD_Algorithm.TabIndex = 4;
            this.cmbSMD_Algorithm.SelectedIndexChanged += new System.EventHandler(this.cmbSMD_Algorithm_SelectedIndexChanged);
            // 
            // txtSMD_ID
            // 
            this.txtSMD_ID.Location = new System.Drawing.Point(64, 47);
            this.txtSMD_ID.Name = "txtSMD_ID";
            this.txtSMD_ID.Size = new System.Drawing.Size(121, 20);
            this.txtSMD_ID.TabIndex = 5;
            this.txtSMD_ID.TextChanged += new System.EventHandler(this.txtSMD_ID_TextChanged);
            // 
            // txtROI_Y
            // 
            this.txtROI_Y.Location = new System.Drawing.Point(150, 96);
            this.txtROI_Y.Name = "txtROI_Y";
            this.txtROI_Y.Size = new System.Drawing.Size(35, 20);
            this.txtROI_Y.TabIndex = 6;
            // 
            // txtROI_Width
            // 
            this.txtROI_Width.Location = new System.Drawing.Point(85, 122);
            this.txtROI_Width.Name = "txtROI_Width";
            this.txtROI_Width.Size = new System.Drawing.Size(35, 20);
            this.txtROI_Width.TabIndex = 7;
            // 
            // txtROI_X
            // 
            this.txtROI_X.Location = new System.Drawing.Point(85, 96);
            this.txtROI_X.Name = "txtROI_X";
            this.txtROI_X.Size = new System.Drawing.Size(35, 20);
            this.txtROI_X.TabIndex = 8;
            // 
            // txtROI_Height
            // 
            this.txtROI_Height.Location = new System.Drawing.Point(150, 122);
            this.txtROI_Height.Name = "txtROI_Height";
            this.txtROI_Height.Size = new System.Drawing.Size(35, 20);
            this.txtROI_Height.TabIndex = 9;
            // 
            // lb2
            // 
            this.lb2.AutoSize = true;
            this.lb2.Location = new System.Drawing.Point(14, 17);
            this.lb2.Name = "lb2";
            this.lb2.Size = new System.Drawing.Size(35, 13);
            this.lb2.TabIndex = 10;
            this.lb2.Text = "label2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 156);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "label3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "label4";
            // 
            // txtSMD_Name
            // 
            this.txtSMD_Name.Location = new System.Drawing.Point(64, 14);
            this.txtSMD_Name.Name = "txtSMD_Name";
            this.txtSMD_Name.Size = new System.Drawing.Size(121, 20);
            this.txtSMD_Name.TabIndex = 13;
            // 
            // btnSettingAG
            // 
            this.btnSettingAG.Location = new System.Drawing.Point(85, 175);
            this.btnSettingAG.Name = "btnSettingAG";
            this.btnSettingAG.Size = new System.Drawing.Size(75, 23);
            this.btnSettingAG.TabIndex = 14;
            this.btnSettingAG.Text = "SettingAG";
            this.btnSettingAG.UseVisualStyleBackColor = true;
            this.btnSettingAG.Click += new System.EventHandler(this.btnSettingAG_Click);
            // 
            // SmdEditControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnSettingAG);
            this.Controls.Add(this.txtSMD_Name);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lb2);
            this.Controls.Add(this.txtROI_Height);
            this.Controls.Add(this.txtROI_X);
            this.Controls.Add(this.txtROI_Width);
            this.Controls.Add(this.txtROI_Y);
            this.Controls.Add(this.txtSMD_ID);
            this.Controls.Add(this.cmbSMD_Algorithm);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkSMD_IsEnabled);
            this.Name = "SmdEditControl";
            this.Size = new System.Drawing.Size(203, 258);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox chkSMD_IsEnabled;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSMD_Algorithm;
        private System.Windows.Forms.TextBox txtSMD_ID;
        private System.Windows.Forms.TextBox txtROI_Y;
        private System.Windows.Forms.TextBox txtROI_Width;
        private System.Windows.Forms.TextBox txtROI_X;
        private System.Windows.Forms.TextBox txtROI_Height;
        private System.Windows.Forms.Label lb2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSMD_Name;
        private System.Windows.Forms.Button btnSettingAG;
    }
}
