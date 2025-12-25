// File: Form1.cs

using Cong1;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static vs_h.model;
using Sunny.UI;

namespace vs_h
{
    public partial class Form1 : UIForm
    {
        #region Fields - Khai báo biến và Controls

        private SmdEditControl smdEditor;
        private PovManager povManager;
        private SmdManager smdManager;
        private ImageLoader imageLoader;
        private RoiDrawer roiDrawer;
        private model.SMD _selectedSmd = null;
        private MindVisionCamera _cam = new MindVisionCamera();
        private string _connectedSn = null;

        // Zoom state
        private float _zoom = 1.0f;
        private const float ZOOM_STEP = 1.1f;
        private const float ZOOM_MIN = 0.1f;
        private const float ZOOM_MAX = 10.0f;
        private Size _imgOriginalSize = Size.Empty;
        private Panel _panelImage;

        #endregion

        #region Constructor và Initialization

        public Form1()
        {
            InitializeComponent();

            // 1. KHỞI TẠO SMDEditControl
            smdEditor = new SmdEditControl();
            smdEditor.Dock = DockStyle.Fill;
            panelControl.Controls.Add(smdEditor);
            smdEditor.Hide();

            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

            // Event handlers cho SMDEditControl
            smdEditor.RequestOpenCodeForm += OnRequestOpenCodeForm;
            smdEditor.RequestOpenHsvForm += OnRequestOpenHsvForm;

            // 2. KHỞI TẠO POV MANAGER
            povManager = new PovManager(txtName, txtExposureTime, checkBoxIsEnabled, treeView1);

            // 3. KHỞI TẠO SMD MANAGER
            smdManager = new SmdManager(treeView1);

            // 4. KHỞI TẠO IMAGE LOADER
            imageLoader = new ImageLoader(pictureBox1, this);

            // 5. KHỞI TẠO ROI DRAWER
            roiDrawer = new RoiDrawer(pictureBox1);
            roiDrawer.RoiCommitted += () =>
            {
                smdEditor.UpdateRoiFieldsFromCurrentSmd();
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeTreeView();
            addSMDToolStripMenuItem.Click += addSMDToolStripMenuItem_Click;
            pictureBox1.ContextMenuStrip = contextMenuStrip1;

            LoadCameraListToCombo();

            ModelLoader loader = new ModelLoader(treeView1);
            loader.LoadAllModelsToTreeView();
        }

        private void InitializeTreeView()
        {
            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(16, 16);

            imageList.Images.Add("PASS", CreateColorIcon(Color.LightGreen));
            imageList.Images.Add("FAIL", CreateColorIcon(Color.LightCoral));
            imageList.Images.Add("SKIP", CreateColorIcon(Color.LightGray));

            treeView1.ImageList = imageList;
        }

        private Image CreateColorIcon(Color color)
        {
            Bitmap bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.FillEllipse(new SolidBrush(color), 1, 1, 14, 14);
                g.DrawEllipse(new Pen(Color.Gray, 1), 1, 1, 14, 14);
            }
            return bmp;
        }

        #endregion

        #region Camera Management

        private void LoadCameraListToCombo()
        {
            cbListCamera.Items.Clear();

            var devs = Cong1.MindVisionCamera.GetDeviceList();

            foreach (var d in devs)
            {
                cbListCamera.Items.Add(new CameraItem
                {
                    SN = d.SN,
                    Name = string.IsNullOrWhiteSpace(d.Friendly) ? d.Product : d.Friendly
                });
            }

            if (cbListCamera.Items.Count > 0)
                cbListCamera.SelectedIndex = 0;
        }

        private string GetSelectedSn()
        {
            return (cbListCamera.SelectedItem as CameraItem)?.SN;
        }

        private void SelectCameraBySn(string sn)
        {
            if (string.IsNullOrEmpty(sn)) return;
            for (int i = 0; i < cbListCamera.Items.Count; i++)
            {
                var item = cbListCamera.Items[i] as CameraItem;
                if (item != null && string.Equals(item.SN, sn, StringComparison.OrdinalIgnoreCase))
                {
                    cbListCamera.SelectedIndex = i;
                    return;
                }
            }
        }

        private void cbListCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cam.IsConnected)
            {
                try { _cam.StopStream(); } catch { }
                _cam.Close();

                btnConnectCam.Text = "CONNECT";
                btnConnectCam.BackColor = SystemColors.Control;
            }
        }

        private void btnConnectCam_Click(object sender, EventArgs e)
        {
            string sn = GetSelectedSn();

            if (string.IsNullOrEmpty(sn))
            {
                MessageBox.Show("Vui lòng chọn camera trước.");
                return;
            }

            // ĐANG CONNECT → DISCONNECT
            if (_cam.IsConnected)
            {
                _cam.Close();

                btnConnectCam.Text = "CONNECT";
                btnConnectCam.BackColor = SystemColors.Control;
                btnConnectCam.ForeColor = Color.Black;

                MessageBox.Show("Đã ngắt kết nối camera.");
                return;
            }

            // CHƯA CONNECT → CONNECT
            int ret = _cam.Open(sn, IntPtr.Zero);
            if (ret > 0)
            {
                if (_cam.StartStream() <= 0)
                {
                    MessageBox.Show("Open OK nhưng CameraPlay thất bại.");
                    return;
                }

                _connectedSn = sn;

                btnConnectCam.Text = "CONNECTED";
                btnConnectCam.BackColor = Color.LimeGreen;
                btnConnectCam.ForeColor = Color.Black;

                ApplyExposureForSelectedPovIfPossible();

                MessageBox.Show($"Đã kết nối camera: {sn}");
            }
            else
            {
                btnConnectCam.Text = "CONNECT";
                btnConnectCam.BackColor = SystemColors.Control;

                MessageBox.Show($"Kết nối camera {sn} thất bại.");
            }
        }

        private void ApplyExposureForSelectedPovIfPossible()
        {
            if (_cam == null || !_cam.IsConnected) return;

            var node = treeView1.SelectedNode;
            if (node == null) return;

            var pov = node.Tag as POV;
            if (pov == null) return;

            string sn = GetSelectedSn();
            if (!string.IsNullOrEmpty(pov.CameraSN) &&
                !string.Equals(sn, pov.CameraSN, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            _cam.SetExposureTime(pov.ExposureTime);
        }

        private void btnPicture_Click(object sender, EventArgs e)
        {
            if (_cam == null || !_cam.IsConnected)
            {
                MessageBox.Show("Chưa connect camera. Hãy bấm CONNECT trước.");
                return;
            }

            Bitmap bmp = _cam.GrabFrame(1000);
            if (bmp == null)
            {
                MessageBox.Show("Không lấy được ảnh từ camera (GrabFrame = null).");
                return;
            }

            var oldImg = pictureBox1.Image;
            pictureBox1.Image = (Bitmap)bmp.Clone();
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            oldImg?.Dispose();

            string dir = Path.Combine(Application.StartupPath, "Captured");
            Directory.CreateDirectory(dir);
            string filePath = Path.Combine(dir, $"snap_{DateTime.Now:yyyyMMdd_HHmmss_fff}.png");

            bmp.Save(filePath, ImageFormat.Png);
            bmp.Dispose();

            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode != null && selectedNode.Tag is POV povData)
            {
                povData.ImagePath = filePath;
                MessageBox.Show($"Đã chụp ảnh từ cam và gán cho POV: {povData.Name}\n{filePath}");
            }
            else
            {
                MessageBox.Show($"Đã chụp ảnh từ cam.\n{filePath}\n\n(Chọn POV nếu bạn muốn gán ảnh cho POV)");
            }
        }

        #endregion

        #region TreeView Events và Navigation

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView1.SelectedNode = e.Node;

            if (e.Button == MouseButtons.Right)
            {
                TreeNode selectedNode = e.Node;
                object selectedTag = selectedNode.Tag;

                contextMenuModel.Hide();
                contextMenuPOV.Hide();
                contextMenuSMD.Hide();

                if (selectedTag is SMD)
                {
                    contextMenuSMD.Show(treeView1, e.Location);
                }
                else if (selectedTag is POV)
                {
                    contextMenuPOV.Show(treeView1, e.Location);
                }
                else if (selectedTag is Model)
                {
                    contextMenuModel.Show(treeView1, e.Location);
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            object selectedTag = e.Node.Tag;

            if (selectedTag is SMD smdChosen)
            {
                roiDrawer.SelectedSmd = null;
                pictureBox1.Invalidate();
                _selectedSmd = smdChosen;
            }
            else
                _selectedSmd = null;

            povManager.HidePovControls();
            smdEditor.Hide();

            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }

            // SMD
            if (selectedTag is SMD smdData)
            {
                smdEditor.LoadData(smdData);
                smdEditor.Show();
                smdEditor.BringToFront();

                roiDrawer.SelectedSmd = smdData;
                pictureBox1.Invalidate();

                if (e.Node.Parent?.Tag is POV parentPov && !string.IsNullOrEmpty(parentPov.ImagePath))
                {
                    LoadImageToPictureBox(parentPov.ImagePath);
                }
            }
            // POV
            else if (selectedTag is POV povData)
            {
                roiDrawer.SelectedSmd = null;
                pictureBox1.Invalidate();

                povManager.ShowPovControls(povData);

                if (!string.IsNullOrEmpty(povData.CameraSN))
                {
                    SelectCameraBySn(povData.CameraSN);
                }

                LoadImageToPictureBox(povData.ImagePath);
            }

            void LoadImageToPictureBox(string path)
            {
                if (string.IsNullOrWhiteSpace(path)) return;

                try
                {
                    if (!File.Exists(path)) return;

                    pictureBox1.Image?.Dispose();
                    pictureBox1.Image = Image.FromFile(path);
                    ResetZoomFit();
                    _zoom = 0.01f;
                    _imgOriginalSize = pictureBox1.Image.Size;

                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi tải ảnh: {ex.Message}", "Lỗi");
                }
            }

            if (e.Node.Tag is POV)
            {
                btnPicture.Enabled = true;
            }
            else
            {
                btnPicture.Enabled = false;
            }
        }

        #endregion

        #region Image Zoom và Display

        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image == null) return;
            if (_imgOriginalSize == Size.Empty) _imgOriginalSize = pictureBox1.Image.Size;

            float oldZoom = _zoom;

            if (e.Delta > 0) _zoom *= ZOOM_STEP;
            else _zoom /= ZOOM_STEP;

            if (_zoom < ZOOM_MIN) _zoom = ZOOM_MIN;
            if (_zoom > ZOOM_MAX) _zoom = ZOOM_MAX;

            if (Math.Abs(_zoom - oldZoom) < 0.0001f) return;

            Point mouseInPanel = _panelImage.PointToClient(Control.MousePosition);

            int scrollX = -_panelImage.AutoScrollPosition.X;
            int scrollY = -_panelImage.AutoScrollPosition.Y;

            float imgX = (scrollX + mouseInPanel.X) / oldZoom;
            float imgY = (scrollY + mouseInPanel.Y) / oldZoom;

            pictureBox1.Width = (int)(_imgOriginalSize.Width * _zoom);
            pictureBox1.Height = (int)(_imgOriginalSize.Height * _zoom);

            int newScrollX = (int)(imgX * _zoom - mouseInPanel.X);
            int newScrollY = (int)(imgY * _zoom - mouseInPanel.Y);

            if (newScrollX < 0) newScrollX = 0;
            if (newScrollY < 0) newScrollY = 0;

            _panelImage.AutoScrollPosition = new Point(newScrollX, newScrollY);

            pictureBox1.Invalidate();
        }

        private void ResetZoomFit()
        {
            if (pictureBox1.Image == null || _panelImage == null) return;

            _imgOriginalSize = pictureBox1.Image.Size;

            int vw = _panelImage.ClientSize.Width;
            int vh = _panelImage.ClientSize.Height;
            if (vw <= 0 || vh <= 0) return;

            float zx = (float)vw / _imgOriginalSize.Width;
            float zy = (float)vh / _imgOriginalSize.Height;
            _zoom = Math.Min(zx, zy);

            if (_zoom > 1f) _zoom = 1f;
            if (_zoom < ZOOM_MIN) _zoom = ZOOM_MIN;

            pictureBox1.Width = (int)(_imgOriginalSize.Width * _zoom);
            pictureBox1.Height = (int)(_imgOriginalSize.Height * _zoom);

            _panelImage.AutoScrollPosition = new Point(0, 0);
            pictureBox1.Invalidate();
        }

        #endregion

        #region ROI Drawing

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_selectedSmd == null)
            {
                e.Cancel = true;
                return;
            }

            if (pictureBox1.Image == null)
            {
                e.Cancel = true;
                return;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (_selectedSmd != null && pictureBox1.Image != null)
                    contextMenuStrip1.Show(pictureBox1, e.Location);
            }
        }

        private void drawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (roiDrawer.SelectedSmd == null)
            {
                MessageBox.Show("Hãy chọn SMD trước.");
                return;
            }
            if (!roiDrawer.ImageAvailable)
            {
                MessageBox.Show("Chưa có ảnh để vẽ ROI.");
                return;
            }

            roiDrawer.EnableDrawMode();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            roiDrawer.ResetSelectedRoi();
        }

        #endregion

        #region Save và Load Data

        private void btnSave_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode == null) return;

            if (selectedNode.Tag is SMD smdData)
            {
                smdEditor.SaveData();
                selectedNode.Text = smdData.Name;
            }
            else if (selectedNode.Tag is POV povData)
            {
                povManager.SavePovData(povData);
                povData.CameraSN = GetSelectedSn() ?? string.Empty;
                selectedNode.Text = povData.Name;

                ApplyExposureForSelectedPovIfPossible();
            }

            SaveAllModelsToJson();
        }

        private void CommitCurrentEditsToTag()
        {
            TreeNode node = treeView1.SelectedNode;
            if (node == null) return;

            if (node.Tag is SMD)
            {
                smdEditor.SaveData();
                node.Text = ((SMD)node.Tag).Name;
            }
            else if (node.Tag is POV pov)
            {
                povManager.SavePovData(pov);
                pov.CameraSN = GetSelectedSn() ?? string.Empty;
                node.Text = pov.Name;
            }
        }

        private void SaveAllModelsToJson()
        {
            try
            {
                CommitCurrentEditsToTag();

                string projectRoot = Path.Combine(Application.StartupPath, @"..\..");
                string folderPath = Path.Combine(projectRoot, "Models");
                folderPath = Path.GetFullPath(folderPath);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                foreach (TreeNode modelNode in treeView1.Nodes)
                {
                    Model model = new Model { Name = modelNode.Text };

                    foreach (TreeNode povNode in modelNode.Nodes)
                    {
                        if (!(povNode.Tag is POV povTag))
                            continue;

                        POV pov = new POV
                        {
                            Name = povTag.Name,
                            ExposureTime = povTag.ExposureTime,
                            IsEnabled = povTag.IsEnabled,
                            ImagePath = povTag.ImagePath,
                            CameraSN = povTag.CameraSN,
                            SMDs = new List<SMD>()
                        };

                        foreach (TreeNode smdNode in povNode.Nodes)
                        {
                            if (!(smdNode.Tag is SMD smdTag))
                                continue;

                            pov.SMDs.Add(smdTag);

                            Console.WriteLine(
                                $"SAVE: {smdTag.Name} ROI=({smdTag.ROI.X},{smdTag.ROI.Y},{smdTag.ROI.Width},{smdTag.ROI.Height}) ALG={smdTag.Algorithm}"
                            );
                        }

                        model.POVs.Add(pov);
                    }

                    string filePath = Path.Combine(folderPath, model.Name + ".json");
                    string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(model, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(filePath, jsonString);
                }

                MessageBox.Show("Đã lưu tất cả model vào JSON!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu JSON: " + ex.Message);
            }
        }

        #endregion

        #region Model/POV/SMD Management

        private void newModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (NewModelForm form = new NewModelForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    string modelName = form.ModelName;
                    SaveModelAsJson(modelName);

                    ModelLoader loader = new ModelLoader(treeView1);
                    loader.LoadAllModelsToTreeView();
                }
            }
        }

        private void SaveModelAsJson(string modelName)
        {
            Model model = new Model { Name = modelName };

            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(model, Newtonsoft.Json.Formatting.Indented);

            string projectRoot = Path.Combine(Application.StartupPath, @"..\..");
            string folderPath = Path.Combine(projectRoot, "Models");
            folderPath = Path.GetFullPath(folderPath);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, modelName + ".json");
            File.WriteAllText(filePath, jsonString);

            MessageBox.Show($"Model '{modelName}' đã được tạo và lưu tại: {filePath}");
        }

        private void openModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string projectRoot = Path.Combine(Application.StartupPath, @"..\..");
            string folderPath = Path.Combine(projectRoot, "Models");
            folderPath = Path.GetFullPath(folderPath);

            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("Thư mục Models chưa tồn tại!");
                return;
            }

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = folderPath;
                ofd.Filter = "JSON files (*.json)|*.json";
                ofd.Title = "Chọn model để mở";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string filePath = ofd.FileName;
                    string modelName = Path.GetFileNameWithoutExtension(filePath);

                    try
                    {
                        ModelLoader loader = new ModelLoader(treeView1);
                        if (!treeView1.Nodes.ContainsKey(modelName))
                        {
                            loader.LoadSpecificModelToTreeView(filePath);
                            MessageBox.Show($"Đã mở model: {modelName}");
                        }
                        else
                        {
                            MessageBox.Show($"Model '{modelName}' đã được mở rồi.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi mở model: " + ex.Message);
                    }
                }
            }
        }

        private void addPOVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null || treeView1.SelectedNode.Level != 0) return;
            TreeNode modelNode = treeView1.SelectedNode;

            povManager.AddPovToModel(modelNode);
        }

        private void addSMDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode == null) return;

            if (selectedNode.Tag is POV pov)
            {
                smdManager.AddSMDToPov(pov, selectedNode);
            }
        }

        private void deleteSMDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode smdNode = treeView1.SelectedNode;
            if (smdNode == null || !(smdNode.Tag is SMD smd))
            {
                MessageBox.Show("Hãy chọn SMD cần xoá.");
                return;
            }

            TreeNode povNode = smdNode.Parent;
            if (povNode == null || !(povNode.Tag is POV pov))
            {
                MessageBox.Show("Không tìm thấy POV cha.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Bạn chắc chắn muốn xoá SMD '{smd.Name}'?",
                "Xác nhận xoá",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            pov.SMDs?.Remove(smd);
            smdNode.Remove();

            smdEditor.Hide();
            roiDrawer.SelectedSmd = null;
            pictureBox1.Invalidate();

            MessageBox.Show("Đã xoá SMD.");
        }

        #endregion

        #region Test All SMD

        private void btnTestAllSmd_Click(object sender, EventArgs e)
        {
            bool finalPass = true;

            try
            {
                CommitCurrentEditsToTag();

                int total = 0, pass = 0, fail = 0, skip = 0;
                var sb = new System.Text.StringBuilder();

                foreach (TreeNode modelNode in treeView1.Nodes)
                {
                    foreach (TreeNode povNode in modelNode.Nodes)
                    {
                        if (!(povNode.Tag is POV pov)) continue;

                        if (string.IsNullOrWhiteSpace(pov.ImagePath) || !System.IO.File.Exists(pov.ImagePath))
                        {
                            sb.AppendLine($"[SKIP] POV '{pov.Name}': chưa có ảnh / ảnh không tồn tại");

                            foreach (TreeNode smdNode in povNode.Nodes)
                            {
                                if (smdNode.Tag is SMD)
                                {
                                    total++;
                                    skip++;
                                    PaintNodeResult(smdNode, "SKIP");
                                }
                            }
                            continue;
                        }

                        using (var img = new HalconDotNet.HImage(pov.ImagePath))
                        {
                            foreach (TreeNode smdNode in povNode.Nodes)
                            {
                                if (!(smdNode.Tag is SMD smd)) continue;
                                total++;

                                if (!smd.IsEnabled)
                                {
                                    skip++;
                                    PaintNodeResult(smdNode, "SKIP");
                                    sb.AppendLine($"[SKIP] {modelNode.Text}/{pov.Name}/{smd.Name}: IsEnabled=false");
                                    continue;
                                }

                                if (!string.Equals(smd.Algorithm, "HSV", StringComparison.OrdinalIgnoreCase))
                                {
                                    skip++;
                                    PaintNodeResult(smdNode, "SKIP");
                                    sb.AppendLine($"[SKIP] {modelNode.Text}/{pov.Name}/{smd.Name}: Algorithm={smd.Algorithm}");
                                    continue;
                                }

                                if (smd.ROI == null || smd.ROI.Width <= 0 || smd.ROI.Height <= 0)
                                {
                                    fail++;
                                    finalPass = false;
                                    PaintNodeResult(smdNode, "FAIL");
                                    sb.AppendLine($"[FAIL] {modelNode.Text}/{pov.Name}/{smd.Name}: ROI invalid");
                                    continue;
                                }

                                if (smd.HSV == null)
                                {
                                    fail++;
                                    finalPass = false;
                                    PaintNodeResult(smdNode, "FAIL");
                                    sb.AppendLine($"[FAIL] {modelNode.Text}/{pov.Name}/{smd.Name}: HSV null");
                                    continue;
                                }

                                double score = ComputeHsvScoreArea(img, smd);
                                bool isPass = (score >= smd.HSV.ScoreMin && score <= smd.HSV.ScoreMax);

                                if (isPass)
                                {
                                    pass++;
                                    PaintNodeResult(smdNode, "PASS");
                                    sb.AppendLine($"[PASS] {modelNode.Text}/{pov.Name}/{smd.Name}: score={score:0} (min={smd.HSV.ScoreMin:0} max={smd.HSV.ScoreMax:0})");
                                }
                                else
                                {
                                    fail++;
                                    finalPass = false;
                                    PaintNodeResult(smdNode, "FAIL");
                                    sb.AppendLine($"[FAIL] {modelNode.Text}/{pov.Name}/{smd.Name}: score={score:0} (min={smd.HSV.ScoreMin:0} max={smd.HSV.ScoreMax:0})");
                                }
                            }
                        }
                    }
                }

                if (total == 0)
                {
                    txtFinalResult.Text = "NO DATA";
                    txtFinalResult.BackColor = Color.Gray;
                    txtFinalResult.ForeColor = Color.White;
                }
                else
                {
                    UpdateFinalResultTextbox(finalPass);
                }

                MessageBox.Show(
                    $"TEST ALL SMD\nTotal: {total}\nPASS: {pass}\nFAIL: {fail}\nSKIP: {skip}\n\nChi tiết:\n{sb}",
                    "Kết quả Test"
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi test: " + ex.Message);
            }
        }

        private double ComputeHsvScoreArea(HalconDotNet.HImage img, SMD smd)
        {
            using (var roi = new HalconDotNet.HRegion())
            {
                double row1 = smd.ROI.Y;
                double col1 = smd.ROI.X;
                double row2 = smd.ROI.Y + smd.ROI.Height;
                double col2 = smd.ROI.X + smd.ROI.Width;
                roi.GenRectangle1(row1, col1, row2, col2);

                using (var imgRoi = img.ReduceDomain(roi))
                {
                    HalconDotNet.HObject r, g, b;
                    HalconDotNet.HOperatorSet.Decompose3(imgRoi, out r, out g, out b);

                    HalconDotNet.HObject h, s, v;
                    HalconDotNet.HOperatorSet.TransFromRgb(r, g, b, out h, out s, out v, "hsv");

                    int hMin = smd.HSV.HMin;
                    int hMax = smd.HSV.HMax;
                    int sMin = smd.HSV.SMin;
                    int sMax = smd.HSV.SMax;

                    if (hMin > hMax) (hMin, hMax) = (hMax, hMin);
                    if (sMin > sMax) (sMin, sMax) = (sMax, sMin);

                    HalconDotNet.HObject regSat;
                    HalconDotNet.HOperatorSet.Threshold(s, out regSat, sMin, sMax);

                    HalconDotNet.HObject hSat;
                    HalconDotNet.HOperatorSet.ReduceDomain(h, regSat, out hSat);

                    HalconDotNet.HObject regColor;
                    HalconDotNet.HOperatorSet.Threshold(hSat, out regColor, hMin, hMax);

                    HalconDotNet.HObject conn, sel;
                    HalconDotNet.HOperatorSet.Connection(regColor, out conn);
                    HalconDotNet.HOperatorSet.SelectShapeStd(conn, out sel, "max_area", 0);

                    HalconDotNet.HTuple num;
                    HalconDotNet.HOperatorSet.CountObj(sel, out num);
                    if (num.I == 0) return 0;

                    HalconDotNet.HTuple area, rr, cc;
                    HalconDotNet.HOperatorSet.AreaCenter(sel, out area, out rr, out cc);
                    return area[0].D;
                }
            }
        }

        private void PaintNodeResult(TreeNode node, string result)
        {
            switch (result)
            {
                case "PASS":
                    node.ImageKey = "PASS";
                    node.SelectedImageKey = "PASS";
                    break;
                case "FAIL":
                    node.ImageKey = "FAIL";
                    node.SelectedImageKey = "FAIL";
                    break;
                default:
                    node.ImageKey = "SKIP";
                    node.SelectedImageKey = "SKIP";
                    break;
            }

            string originalText = node.Text;
            if (originalText.StartsWith("✓ ") || originalText.StartsWith("✗ ") || originalText.StartsWith("○ "))
            {
                originalText = originalText.Substring(2);
            }
            node.Text = originalText;
        }

        private void UpdateFinalResultTextbox(bool isPass)
        {
            if (isPass)
            {
                txtFinalResult.Text = "PASS";
                txtFinalResult.FillColor = Color.LimeGreen;
                txtFinalResult.ForeColor = Color.Black;
                txtFinalResult.RectColor = Color.LimeGreen;
            }
            else
            {
                txtFinalResult.Text = "FAIL";
                txtFinalResult.FillColor = Color.Red;
                txtFinalResult.ForeColor = Color.White;
                txtFinalResult.RectColor = Color.Red;
            }
        }

        #endregion

        #region Event Handlers - Form và Dialog

        private void OnRequestOpenCodeForm()
        {
            var smd = smdEditor.CurrentSmd;
            if (smd == null)
            {
                MessageBox.Show("Chưa chọn SMD");
                return;
            }

            TreeNode smdNode = treeView1.SelectedNode;
            POV pov = smdNode?.Parent?.Tag as POV;

            if (pov == null || string.IsNullOrWhiteSpace(pov.ImagePath))
            {
                MessageBox.Show("POV chưa có ảnh");
                return;
            }

            FormCODE f = new FormCODE
            {
                InputImagePath = pov.ImagePath,
                InputRoi = smd.ROI,
                TargetSmd = smd
            };

            f.ShowDialog(this);
        }

        private void OnRequestOpenHsvForm()
        {
            var smd = smdEditor.CurrentSmd;
            if (smd == null)
            {
                MessageBox.Show("Chưa chọn SMD");
                return;
            }

            TreeNode smdNode = treeView1.SelectedNode;
            POV pov = smdNode?.Parent?.Tag as POV;

            if (pov == null || string.IsNullOrWhiteSpace(pov.ImagePath))
            {
                MessageBox.Show("POV chưa có ảnh");
                return;
            }

            FormHSV f = new FormHSV
            {
                InputImagePath = pov.ImagePath,
                InputRoi = smd.ROI,
                TargetSmd = smd
            };

            f.ShowDialog(this);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                e.SuppressKeyPress = true;
                SaveAllModelsToJson();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (_cam.IsConnected)
                {
                    _cam.StopStream();
                    _cam.Close();
                }
            }
            catch { }
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                RUN runForm = new RUN();
                runForm.StartPosition = FormStartPosition.CenterParent;
                runForm.Show(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể mở Form RUN: " + ex.Message);
            }
        }

        private void logServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new LogServer();
            f.StartPosition = FormStartPosition.CenterParent;
            f.Show(this);
        }

        #endregion
    }
}