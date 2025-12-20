namespace vs_h
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtFinalResult = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnCamera = new System.Windows.Forms.Button();
            this.btnTestAllSmd = new System.Windows.Forms.Button();
            this.btnPicture = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.drawToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuModel = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addPOVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panelControl = new System.Windows.Forms.Panel();
            this.btnConnectCam = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cbListCamera = new System.Windows.Forms.ComboBox();
            this.checkBoxIsEnabled = new System.Windows.Forms.CheckBox();
            this.txtExposureTime = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.contextMenuPOV = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addSMDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuSMD = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteSMDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelImage = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuModel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panelControl.SuspendLayout();
            this.contextMenuPOV.SuspendLayout();
            this.contextMenuSMD.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.LightBlue;
            this.panel1.Controls.Add(this.txtFinalResult);
            this.panel1.Controls.Add(this.menuStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(256, 425);
            this.panel1.TabIndex = 0;
            // 
            // txtFinalResult
            // 
            this.txtFinalResult.Location = new System.Drawing.Point(3, 46);
            this.txtFinalResult.Multiline = true;
            this.txtFinalResult.Name = "txtFinalResult";
            this.txtFinalResult.ReadOnly = true;
            this.txtFinalResult.Size = new System.Drawing.Size(250, 84);
            this.txtFinalResult.TabIndex = 1;
            this.txtFinalResult.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.runToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(256, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newModelToolStripMenuItem,
            this.openModelToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newModelToolStripMenuItem
            // 
            this.newModelToolStripMenuItem.Name = "newModelToolStripMenuItem";
            this.newModelToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.newModelToolStripMenuItem.Text = "New Model";
            this.newModelToolStripMenuItem.Click += new System.EventHandler(this.newModelToolStripMenuItem_Click);
            // 
            // openModelToolStripMenuItem
            // 
            this.openModelToolStripMenuItem.Name = "openModelToolStripMenuItem";
            this.openModelToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.openModelToolStripMenuItem.Text = "Open Model";
            this.openModelToolStripMenuItem.Click += new System.EventHandler(this.openModelToolStripMenuItem_Click);
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.runToolStripMenuItem.Text = "Run";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.runToolStripMenuItem_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.SteelBlue;
            this.panel2.Controls.Add(this.btnCamera);
            this.panel2.Controls.Add(this.btnTestAllSmd);
            this.panel2.Controls.Add(this.btnPicture);
            this.panel2.Controls.Add(this.btnSave);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(446, 86);
            this.panel2.TabIndex = 1;
            // 
            // btnCamera
            // 
            this.btnCamera.Location = new System.Drawing.Point(184, 15);
            this.btnCamera.Name = "btnCamera";
            this.btnCamera.Size = new System.Drawing.Size(75, 23);
            this.btnCamera.TabIndex = 3;
            this.btnCamera.Text = "Camera";
            this.btnCamera.UseVisualStyleBackColor = true;
            this.btnCamera.Click += new System.EventHandler(this.btnCamera_Click);
            // 
            // btnTestAllSmd
            // 
            this.btnTestAllSmd.Location = new System.Drawing.Point(274, 15);
            this.btnTestAllSmd.Name = "btnTestAllSmd";
            this.btnTestAllSmd.Size = new System.Drawing.Size(75, 23);
            this.btnTestAllSmd.TabIndex = 2;
            this.btnTestAllSmd.Text = "test";
            this.btnTestAllSmd.UseVisualStyleBackColor = true;
            this.btnTestAllSmd.Click += new System.EventHandler(this.btnTestAllSmd_Click);
            // 
            // btnPicture
            // 
            this.btnPicture.Location = new System.Drawing.Point(103, 15);
            this.btnPicture.Name = "btnPicture";
            this.btnPicture.Size = new System.Drawing.Size(75, 23);
            this.btnPicture.TabIndex = 1;
            this.btnPicture.Text = "Picture";
            this.btnPicture.UseVisualStyleBackColor = true;
            this.btnPicture.Click += new System.EventHandler(this.btnPicture_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(13, 15);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pictureBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(256, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(446, 425);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.drawToolStripMenuItem,
            this.resetToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(103, 48);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // drawToolStripMenuItem
            // 
            this.drawToolStripMenuItem.Name = "drawToolStripMenuItem";
            this.drawToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.drawToolStripMenuItem.Text = "Draw";
            this.drawToolStripMenuItem.Click += new System.EventHandler(this.drawToolStripMenuItem_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // contextMenuModel
            // 
            this.contextMenuModel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addPOVToolStripMenuItem});
            this.contextMenuModel.Name = "contextMenuModel";
            this.contextMenuModel.Size = new System.Drawing.Size(120, 26);
            // 
            // addPOVToolStripMenuItem
            // 
            this.addPOVToolStripMenuItem.Name = "addPOVToolStripMenuItem";
            this.addPOVToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.addPOVToolStripMenuItem.Text = "Add Pov";
            this.addPOVToolStripMenuItem.Click += new System.EventHandler(this.addPOVToolStripMenuItem_Click);
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.White;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(259, 425);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panelControl);
            this.panel3.Controls.Add(this.treeView1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(702, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(259, 425);
            this.panel3.TabIndex = 2;
            // 
            // panelControl
            // 
            this.panelControl.BackColor = System.Drawing.Color.LightBlue;
            this.panelControl.Controls.Add(this.btnConnectCam);
            this.panelControl.Controls.Add(this.label3);
            this.panelControl.Controls.Add(this.cbListCamera);
            this.panelControl.Controls.Add(this.checkBoxIsEnabled);
            this.panelControl.Controls.Add(this.txtExposureTime);
            this.panelControl.Controls.Add(this.txtName);
            this.panelControl.Controls.Add(this.label2);
            this.panelControl.Controls.Add(this.label1);
            this.panelControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl.Location = new System.Drawing.Point(0, 201);
            this.panelControl.Name = "panelControl";
            this.panelControl.Size = new System.Drawing.Size(259, 224);
            this.panelControl.TabIndex = 1;
            // 
            // btnConnectCam
            // 
            this.btnConnectCam.Location = new System.Drawing.Point(94, 170);
            this.btnConnectCam.Name = "btnConnectCam";
            this.btnConnectCam.Size = new System.Drawing.Size(75, 23);
            this.btnConnectCam.TabIndex = 7;
            this.btnConnectCam.Text = "btnConnectCam";
            this.btnConnectCam.UseVisualStyleBackColor = true;
            this.btnConnectCam.Click += new System.EventHandler(this.btnConnectCam_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Camera";
            // 
            // cbListCamera
            // 
            this.cbListCamera.FormattingEnabled = true;
            this.cbListCamera.Location = new System.Drawing.Point(80, 113);
            this.cbListCamera.Name = "cbListCamera";
            this.cbListCamera.Size = new System.Drawing.Size(121, 21);
            this.cbListCamera.TabIndex = 5;
            this.cbListCamera.SelectedIndexChanged += new System.EventHandler(this.cbListCamera_SelectedIndexChanged);
            // 
            // checkBoxIsEnabled
            // 
            this.checkBoxIsEnabled.AutoSize = true;
            this.checkBoxIsEnabled.Location = new System.Drawing.Point(94, 90);
            this.checkBoxIsEnabled.Name = "checkBoxIsEnabled";
            this.checkBoxIsEnabled.Size = new System.Drawing.Size(73, 17);
            this.checkBoxIsEnabled.TabIndex = 4;
            this.checkBoxIsEnabled.Text = "IsEnabled";
            this.checkBoxIsEnabled.UseVisualStyleBackColor = true;
            // 
            // txtExposureTime
            // 
            this.txtExposureTime.Location = new System.Drawing.Point(94, 63);
            this.txtExposureTime.Name = "txtExposureTime";
            this.txtExposureTime.Size = new System.Drawing.Size(100, 20);
            this.txtExposureTime.TabIndex = 3;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(94, 25);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(100, 20);
            this.txtName.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "ExposureTime";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // contextMenuPOV
            // 
            this.contextMenuPOV.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSMDToolStripMenuItem});
            this.contextMenuPOV.Name = "contextMenuPOV";
            this.contextMenuPOV.Size = new System.Drawing.Size(125, 26);
            // 
            // addSMDToolStripMenuItem
            // 
            this.addSMDToolStripMenuItem.Name = "addSMDToolStripMenuItem";
            this.addSMDToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.addSMDToolStripMenuItem.Text = "Add SMD";
            this.addSMDToolStripMenuItem.Click += new System.EventHandler(this.addSMDToolStripMenuItem_Click);
            // 
            // contextMenuSMD
            // 
            this.contextMenuSMD.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteSMDToolStripMenuItem});
            this.contextMenuSMD.Name = "contextMenuSMD";
            this.contextMenuSMD.Size = new System.Drawing.Size(136, 26);
            // 
            // deleteSMDToolStripMenuItem
            // 
            this.deleteSMDToolStripMenuItem.Name = "deleteSMDToolStripMenuItem";
            this.deleteSMDToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.deleteSMDToolStripMenuItem.Text = "Delete SMD";
            this.deleteSMDToolStripMenuItem.Click += new System.EventHandler(this.deleteSMDToolStripMenuItem_Click);
            // 
            // panelImage
            // 
            this.panelImage.AutoScroll = true;
            this.panelImage.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panelImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelImage.Location = new System.Drawing.Point(256, 0);
            this.panelImage.Name = "panelImage";
            this.panelImage.Size = new System.Drawing.Size(446, 425);
            this.panelImage.TabIndex = 4;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel4.Controls.Add(this.panel2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(256, 339);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(446, 86);
            this.panel4.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(961, 425);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panelImage);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuModel.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panelControl.ResumeLayout(false);
            this.panelControl.PerformLayout();
            this.contextMenuPOV.ResumeLayout(false);
            this.contextMenuSMD.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openModelToolStripMenuItem;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ContextMenuStrip contextMenuModel;
        private System.Windows.Forms.ToolStripMenuItem addPOVToolStripMenuItem;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panelControl;
        private System.Windows.Forms.TextBox txtExposureTime;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxIsEnabled;
        private System.Windows.Forms.ContextMenuStrip contextMenuPOV;
        private System.Windows.Forms.ToolStripMenuItem addSMDToolStripMenuItem;
        private System.Windows.Forms.Button btnPicture;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem drawToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.Button btnTestAllSmd;
        private System.Windows.Forms.TextBox txtFinalResult;
        private System.Windows.Forms.ContextMenuStrip contextMenuSMD;
        private System.Windows.Forms.ToolStripMenuItem deleteSMDToolStripMenuItem;
        private System.Windows.Forms.Button btnCamera;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbListCamera;
        private System.Windows.Forms.Button btnConnectCam;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.Panel panelImage;
        private System.Windows.Forms.Panel panel4;
    }
}

