namespace vs_h
{
    partial class RUN
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
            this.flowLayoutPanelRUN = new System.Windows.Forms.FlowLayoutPanel();
            this.txtResultRUN = new System.Windows.Forms.TextBox();
            this.txtSnRUN = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // flowLayoutPanelRUN
            // 
            this.flowLayoutPanelRUN.AutoScroll = true;
            this.flowLayoutPanelRUN.Location = new System.Drawing.Point(12, 25);
            this.flowLayoutPanelRUN.Name = "flowLayoutPanelRUN";
            this.flowLayoutPanelRUN.Padding = new System.Windows.Forms.Padding(8);
            this.flowLayoutPanelRUN.Size = new System.Drawing.Size(1221, 514);
            this.flowLayoutPanelRUN.TabIndex = 0;
            this.flowLayoutPanelRUN.Paint += new System.Windows.Forms.PaintEventHandler(this.flowLayoutPanelRUN_Paint);
            // 
            // txtResultRUN
            // 
            this.txtResultRUN.Location = new System.Drawing.Point(12, 554);
            this.txtResultRUN.Multiline = true;
            this.txtResultRUN.Name = "txtResultRUN";
            this.txtResultRUN.Size = new System.Drawing.Size(451, 101);
            this.txtResultRUN.TabIndex = 1;
            // 
            // txtSnRUN
            // 
            this.txtSnRUN.Location = new System.Drawing.Point(59, 670);
            this.txtSnRUN.Name = "txtSnRUN";
            this.txtSnRUN.Size = new System.Drawing.Size(317, 20);
            this.txtSnRUN.TabIndex = 2;
            // 
            // RUN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1245, 713);
            this.Controls.Add(this.txtSnRUN);
            this.Controls.Add(this.txtResultRUN);
            this.Controls.Add(this.flowLayoutPanelRUN);
            this.Name = "RUN";
            this.Text = "RUN";
            this.Load += new System.EventHandler(this.RUN_Load_1);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelRUN;
        private System.Windows.Forms.TextBox txtResultRUN;
        private System.Windows.Forms.TextBox txtSnRUN;
    }
}