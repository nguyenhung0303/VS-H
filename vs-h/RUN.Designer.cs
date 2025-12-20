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
            this.panel1 = new System.Windows.Forms.Panel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanelRUN
            // 
            this.flowLayoutPanelRUN.AutoScroll = true;
            this.flowLayoutPanelRUN.BackColor = System.Drawing.Color.PowderBlue;
            this.flowLayoutPanelRUN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelRUN.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanelRUN.Name = "flowLayoutPanelRUN";
            this.flowLayoutPanelRUN.Padding = new System.Windows.Forms.Padding(8);
            this.flowLayoutPanelRUN.Size = new System.Drawing.Size(980, 455);
            this.flowLayoutPanelRUN.TabIndex = 0;
            this.flowLayoutPanelRUN.Paint += new System.Windows.Forms.PaintEventHandler(this.flowLayoutPanelRUN_Paint);
            // 
            // txtResultRUN
            // 
            this.txtResultRUN.Enabled = false;
            this.txtResultRUN.Font = new System.Drawing.Font("Microsoft Sans Serif", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResultRUN.Location = new System.Drawing.Point(15, 29);
            this.txtResultRUN.Multiline = true;
            this.txtResultRUN.Name = "txtResultRUN";
            this.txtResultRUN.Size = new System.Drawing.Size(532, 130);
            this.txtResultRUN.TabIndex = 1;
            this.txtResultRUN.TextChanged += new System.EventHandler(this.txtResultRUN_TextChanged);
            // 
            // txtSnRUN
            // 
            this.txtSnRUN.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSnRUN.Location = new System.Drawing.Point(582, 116);
            this.txtSnRUN.Multiline = true;
            this.txtSnRUN.Name = "txtSnRUN";
            this.txtSnRUN.Size = new System.Drawing.Size(240, 43);
            this.txtSnRUN.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.CadetBlue;
            this.panel1.Controls.Add(this.richTextBox1);
            this.panel1.Controls.Add(this.txtResultRUN);
            this.panel1.Controls.Add(this.txtSnRUN);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 269);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(980, 186);
            this.panel1.TabIndex = 3;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.richTextBox1.Location = new System.Drawing.Point(717, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(263, 186);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "";
            // 
            // RUN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 455);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.flowLayoutPanelRUN);
            this.Name = "RUN";
            this.Text = "RUN";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.RUN_Load_1);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelRUN;
        private System.Windows.Forms.TextBox txtResultRUN;
        private System.Windows.Forms.TextBox txtSnRUN;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}