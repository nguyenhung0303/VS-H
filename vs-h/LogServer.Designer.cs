namespace vs_h
{
    partial class LogServer
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
            this.txtHostName = new Sunny.UI.UITextBox();
            this.txtUserServer = new Sunny.UI.UITextBox();
            this.txtPassServer = new Sunny.UI.UITextBox();
            this.Hostname = new Sunny.UI.UILabel();
            this.uiLabel2 = new Sunny.UI.UILabel();
            this.uiLabel3 = new Sunny.UI.UILabel();
            this.Pass = new Sunny.UI.UILabel();
            this.uiGroupBox1 = new Sunny.UI.UIGroupBox();
            this.btnSaveServer = new Sunny.UI.UIButton();
            this.txtPort = new Sunny.UI.UITextBox();
            this.uiGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtHostName
            // 
            this.txtHostName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtHostName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtHostName.Location = new System.Drawing.Point(31, 76);
            this.txtHostName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtHostName.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtHostName.Name = "txtHostName";
            this.txtHostName.Padding = new System.Windows.Forms.Padding(5);
            this.txtHostName.ShowText = false;
            this.txtHostName.Size = new System.Drawing.Size(329, 29);
            this.txtHostName.TabIndex = 0;
            this.txtHostName.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtHostName.Watermark = "";
            // 
            // txtUserServer
            // 
            this.txtUserServer.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtUserServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtUserServer.Location = new System.Drawing.Point(31, 153);
            this.txtUserServer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtUserServer.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtUserServer.Name = "txtUserServer";
            this.txtUserServer.Padding = new System.Windows.Forms.Padding(5);
            this.txtUserServer.ShowText = false;
            this.txtUserServer.Size = new System.Drawing.Size(253, 29);
            this.txtUserServer.TabIndex = 3;
            this.txtUserServer.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtUserServer.Watermark = "";
            // 
            // txtPassServer
            // 
            this.txtPassServer.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPassServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtPassServer.Location = new System.Drawing.Point(292, 153);
            this.txtPassServer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPassServer.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtPassServer.Name = "txtPassServer";
            this.txtPassServer.Padding = new System.Windows.Forms.Padding(5);
            this.txtPassServer.ShowText = false;
            this.txtPassServer.Size = new System.Drawing.Size(251, 29);
            this.txtPassServer.TabIndex = 3;
            this.txtPassServer.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtPassServer.Watermark = "";
            // 
            // Hostname
            // 
            this.Hostname.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.Hostname.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.Hostname.Location = new System.Drawing.Point(27, 48);
            this.Hostname.Name = "Hostname";
            this.Hostname.Size = new System.Drawing.Size(100, 23);
            this.Hostname.TabIndex = 4;
            this.Hostname.Text = "Host name :";
            // 
            // uiLabel2
            // 
            this.uiLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel2.Location = new System.Drawing.Point(27, 125);
            this.uiLabel2.Name = "uiLabel2";
            this.uiLabel2.Size = new System.Drawing.Size(100, 23);
            this.uiLabel2.TabIndex = 5;
            this.uiLabel2.Text = "User name :";
            // 
            // uiLabel3
            // 
            this.uiLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel3.Location = new System.Drawing.Point(389, 48);
            this.uiLabel3.Name = "uiLabel3";
            this.uiLabel3.Size = new System.Drawing.Size(100, 23);
            this.uiLabel3.TabIndex = 6;
            this.uiLabel3.Text = "Port number :";
            // 
            // Pass
            // 
            this.Pass.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.Pass.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.Pass.Location = new System.Drawing.Point(297, 125);
            this.Pass.Name = "Pass";
            this.Pass.Size = new System.Drawing.Size(100, 23);
            this.Pass.TabIndex = 7;
            this.Pass.Text = "PassWord :";
            // 
            // uiGroupBox1
            // 
            this.uiGroupBox1.Controls.Add(this.txtHostName);
            this.uiGroupBox1.Controls.Add(this.Pass);
            this.uiGroupBox1.Controls.Add(this.txtPort);
            this.uiGroupBox1.Controls.Add(this.uiLabel3);
            this.uiGroupBox1.Controls.Add(this.txtPassServer);
            this.uiGroupBox1.Controls.Add(this.uiLabel2);
            this.uiGroupBox1.Controls.Add(this.txtUserServer);
            this.uiGroupBox1.Controls.Add(this.Hostname);
            this.uiGroupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiGroupBox1.Location = new System.Drawing.Point(34, 51);
            this.uiGroupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiGroupBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiGroupBox1.Name = "uiGroupBox1";
            this.uiGroupBox1.Padding = new System.Windows.Forms.Padding(0, 32, 0, 0);
            this.uiGroupBox1.Size = new System.Drawing.Size(643, 268);
            this.uiGroupBox1.TabIndex = 8;
            this.uiGroupBox1.Text = "Session";
            this.uiGroupBox1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSaveServer
            // 
            this.btnSaveServer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnSaveServer.Location = new System.Drawing.Point(308, 337);
            this.btnSaveServer.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSaveServer.Name = "btnSaveServer";
            this.btnSaveServer.Size = new System.Drawing.Size(100, 35);
            this.btnSaveServer.TabIndex = 9;
            this.btnSaveServer.Text = "Save";
            this.btnSaveServer.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSaveServer.Click += new System.EventHandler(this.btnSaveServer_Click);
            // 
            // txtPort
            // 
            this.txtPort.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPort.Enabled = false;
            this.txtPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtPort.Location = new System.Drawing.Point(393, 76);
            this.txtPort.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPort.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtPort.Name = "txtPort";
            this.txtPort.Padding = new System.Windows.Forms.Padding(5);
            this.txtPort.ShowText = false;
            this.txtPort.Size = new System.Drawing.Size(150, 29);
            this.txtPort.TabIndex = 3;
            this.txtPort.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtPort.Watermark = "";
            // 
            // LogServer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.ClientSize = new System.Drawing.Size(730, 401);
            this.Controls.Add(this.btnSaveServer);
            this.Controls.Add(this.uiGroupBox1);
            this.Name = "LogServer";
            this.Text = "LogServer";
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 730, 401);
   
            this.uiGroupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITextBox txtHostName;
        private Sunny.UI.UITextBox txtUserServer;
        private Sunny.UI.UITextBox txtPassServer;
        private Sunny.UI.UILabel Hostname;
        private Sunny.UI.UILabel uiLabel2;
        private Sunny.UI.UILabel uiLabel3;
        private Sunny.UI.UILabel Pass;
        private Sunny.UI.UIGroupBox uiGroupBox1;
        private Sunny.UI.UIButton btnSaveServer;
        private Sunny.UI.UITextBox txtPort;
    }
}