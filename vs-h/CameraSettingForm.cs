using Cong1;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace vs_h
{
    public partial class CameraSettingForm : Form
    {
        private MindVisionCamera _cam = new MindVisionCamera();

        public CameraSettingForm()
        {
            InitializeComponent();

            // ✅ chặn đóng form -> chỉ Hide
            this.FormClosing += CameraSettingForm_FormClosing;

            // ✅ cập nhật màu nút khi mở form
            UpdateConnectButtonUI();
        }
        public string SelectedSN { get; set; }

        private void UpdateConnectButtonUI()
        {
            if (_cam != null && _cam.IsConnected)
            {
                btnConnect.Text = "CONNECTED";
                btnConnect.BackColor = Color.LimeGreen;
                btnConnect.ForeColor = Color.Black;
            }
            else
            {
                btnConnect.Text = "CONNECT";
                btnConnect.BackColor = SystemColors.Control;
                btnConnect.ForeColor = Color.Black;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string serialNumber = SelectedSN;
            if (string.IsNullOrWhiteSpace(serialNumber))
            {
                MessageBox.Show("Chưa chọn camera SN ở Form1.");
                return;
            }

            // ⚠ panelPreview phải tồn tại (handle đã tạo)
            IntPtr previewHandle = panelPreview.Handle;

            if (_cam.IsConnected)
            {
                // DISCONNECT
                if (_cam.Close() > 0)
                {
                    MessageBox.Show($"Camera {serialNumber} đã ngắt kết nối. Luồng Live đã dừng.", "Thông báo");
                }
            }
            else
            {
                // CONNECT + START LIVE
                if (_cam.Open(serialNumber, previewHandle) > 0)
                {
                    _cam.StartLive();
                    MessageBox.Show($"Camera {serialNumber} đã kết nối thành công. Luồng Live đang chạy.", "Thành công");
                }
                else
                {
                    MessageBox.Show($"Kết nối camera {serialNumber} thất bại.", "Lỗi kết nối");
                }
            }

            // ✅ cập nhật màu nút sau khi toggle
            UpdateConnectButtonUI();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!_cam.IsConnected)
            {
                MessageBox.Show("Camera chưa kết nối.");
                return;
            }

            Bitmap bmp = _cam.GrabFrame(1000);
            if (bmp == null)
            {
                MessageBox.Show("Không lấy được ảnh (GrabFrame = null).");
                return;
            }

            var old = pictureBoxCamera.Image;
            pictureBoxCamera.Image = (Bitmap)bmp.Clone();
            old?.Dispose();

            string dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                "MindVisionShots");
            Directory.CreateDirectory(dir);

            string file = Path.Combine(dir, $"snap_{DateTime.Now:yyyyMMdd_HHmmss_fff}.png");
            bmp.Save(file, ImageFormat.Png);
            bmp.Dispose();

            MessageBox.Show($"Đã lưu ảnh:\n{file}");
        }

        private void CameraSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ✅ không cho đóng form (Dispose)
            e.Cancel = true;
            this.Hide();

            // ❌ KHÔNG Close camera ở đây
        }

        // (tuỳ chọn) nếu bạn muốn mỗi lần Show lại thì update đúng trạng thái
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            UpdateConnectButtonUI();
        }
    }
}
