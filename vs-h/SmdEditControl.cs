using System;
using System.ComponentModel; // Giữ lại các using gốc của UserControl
using System.Globalization;
using System.Windows.Forms;
using static vs_h.model; // Cần thiết để nhận diện kiểu dữ liệu SMD
using System.Diagnostics;

namespace vs_h
{
    // Đảm bảo class này là public để Form1 có thể truy cập
    public partial class SmdEditControl : UserControl
    {
        public event Action RequestOpenHsvForm;

        public event Action RequestOpenCodeForm;

        // 🌟 Lưu trữ đối tượng SMD hiện tại
        public SMD CurrentSmd { get; private set; }

        public SmdEditControl()
        {
            InitializeComponent();

            if (!cmbSMD_Algorithm.Items.Contains("HSV"))
                cmbSMD_Algorithm.Items.Add("HSV");
        }

        // =========================================================
        // 🌟 PHƯƠNG THỨC LOAD DATA (Được gọi từ Form1)
        // =========================================================
        public void LoadData(SMD smd)
        {
            // Gán đối tượng SMD vào thuộc tính nội bộ
            CurrentSmd = smd;

            // --- 1. Đổ dữ liệu SMD ---
            // (Sử dụng tên Controls chính xác của bạn)
            txtSMD_Name.Text = smd.Name.ToString();
            txtSMD_ID.Text = smd.ID.ToString();
            chkSMD_IsEnabled.Checked = smd.IsEnabled;

            // Đổ dữ liệu Algorithm 
            // Cần đảm bảo cmbSMD_Algorithm đã được nạp giá trị
            cmbSMD_Algorithm.Text = smd.Algorithm.ToString();

            // --- 2. Đổ dữ liệu ROI lồng (sử dụng InvariantCulture cho double) ---
            txtROI_X.Text = smd.ROI.X.ToString(CultureInfo.InvariantCulture);
            txtROI_Y.Text = smd.ROI.Y.ToString(CultureInfo.InvariantCulture);
            txtROI_Width.Text = smd.ROI.Width.ToString(CultureInfo.InvariantCulture);
            txtROI_Height.Text = smd.ROI.Height.ToString(CultureInfo.InvariantCulture);
        }

        // =========================================================
        // TÙY CHỌN: HÀM SAVE DATA (Để lưu các thay đổi của người dùng)
        // =========================================================
        public void SaveData()
        {
            if (CurrentSmd == null) return;

            // --- 1. Cập nhật thuộc tính SMD ---
            CurrentSmd.Name = txtSMD_Name.Text;
            CurrentSmd.IsEnabled = chkSMD_IsEnabled.Checked;

            // Cập nhật Algorithm (Parse int)
            CurrentSmd.Algorithm = cmbSMD_Algorithm.Text.Trim();

            // --- 2. Cập nhật ROI lồng (Parse double) ---
            var culture = CultureInfo.InvariantCulture; // Đảm bảo sử dụng dấu chấm thập phân

            if (double.TryParse(txtROI_X.Text, NumberStyles.Any, culture, out double x))
                CurrentSmd.ROI.X = x;

            if (double.TryParse(txtROI_Y.Text, NumberStyles.Any, culture, out double y))
                CurrentSmd.ROI.Y = y;

            if (double.TryParse(txtROI_Width.Text, NumberStyles.Any, culture, out double w))
                CurrentSmd.ROI.Width = w;

            if (double.TryParse(txtROI_Height.Text, NumberStyles.Any, culture, out double h))
                CurrentSmd.ROI.Height = h;


            Debug.WriteLine("--- DỮ LIỆU SMD ĐÃ CẬP NHẬT ---");
            Debug.WriteLine($"Name: {CurrentSmd.Name}, ID: {CurrentSmd.ID}, Enabled: {CurrentSmd.IsEnabled}");
        }

        private void txtSMD_ID_TextChanged(object sender, EventArgs e)
        {

        }

        public void UpdateRoiFieldsFromCurrentSmd()
        {
            if (CurrentSmd == null) return;

            var c = CultureInfo.InvariantCulture;
            txtROI_X.Text = CurrentSmd.ROI.X.ToString(c);
            txtROI_Y.Text = CurrentSmd.ROI.Y.ToString(c);
            txtROI_Width.Text = CurrentSmd.ROI.Width.ToString(c);
            txtROI_Height.Text = CurrentSmd.ROI.Height.ToString(c);
        }

        private void cmbSMD_Algorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentSmd == null) return;
            CurrentSmd.Algorithm = cmbSMD_Algorithm.Text.Trim();
        }

        private void btnSettingAG_Click(object sender, EventArgs e)
        {
            if (CurrentSmd == null)
                return;

            string alg = cmbSMD_Algorithm.Text;

            if (alg == "HSV")
            {
                
                RequestOpenHsvForm?.Invoke();
            }
            else if (alg == "QR")
            {
                RequestOpenCodeForm?.Invoke();   // ✅
            }
            else
            {
                MessageBox.Show("Thuật toán này chưa có Setting");
            }

        }
      
    }
}