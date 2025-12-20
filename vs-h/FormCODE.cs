using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static vs_h.model;

namespace vs_h
{
    public partial class FormCODE : Form
    {
        public string InputImagePath { get; set; }   // ảnh của POV
        public ROI InputRoi { get; set; }            // ROI của SMD
        public SMD TargetSmd { get; set; }           // SMD đang setting (tuỳ bạn dùng sau)

        private HalconDotNet.HImage _img;

        public FormCODE()
        {
            InitializeComponent();
            this.Load += FormCODE_Load;
        }

        private void FormCODE_Load(object sender, EventArgs e)
        {
            LoadAndDisplay();
        }

        private void LoadAndDisplay()
        {
            if (string.IsNullOrWhiteSpace(InputImagePath) || !System.IO.File.Exists(InputImagePath))
            {
                MessageBox.Show("Chưa có ảnh (InputImagePath).");
                return;
            }

            _img?.Dispose();
            _img = new HalconDotNet.HImage(InputImagePath);

            var win = hWindowControl1.HalconWindow;     // nếu bạn dùng HWindowControl
            win.ClearWindow();

            HalconDotNet.HTuple w, h;
            HalconDotNet.HOperatorSet.GetImageSize(_img, out w, out h);
            win.SetPart(0, 0, h.I - 1, w.I - 1);

            win.DispObj(_img);

            // vẽ ROI nếu có
            if (InputRoi != null && InputRoi.Width > 0 && InputRoi.Height > 0)
            {
                double row1 = InputRoi.Y;
                double col1 = InputRoi.X;
                double row2 = InputRoi.Y + InputRoi.Height;
                double col2 = InputRoi.X + InputRoi.Width;

                win.SetColor("yellow");
                win.SetDraw("margin");
                win.SetLineWidth(3);
                win.DispRectangle1(row1, col1, row2, col2);
                win.SetLineWidth(1);
            }
        }

        private void btnTestCode_Click(object sender, EventArgs e)
        {
            if (_img == null)
            {
                MessageBox.Show("Chưa load ảnh");
                return;
            }
            if (InputRoi == null || InputRoi.Width <= 0 || InputRoi.Height <= 0)
            {
                MessageBox.Show("Chưa có ROI");
                return;
            }

            // ✅ Chọn loại code: "QR Code" hoặc "Aztec Code" hoặc "Data Matrix ECC 200"
            string codeType = "QR Code";

            var r = DataCodeReader.ReadInRoi(_img, InputRoi.X, InputRoi.Y, InputRoi.Width, InputRoi.Height, codeType);

            var win = hWindowControl1.HalconWindow;
            win.ClearWindow();

            // hiển thị ảnh
            HTuple w, h;
            HOperatorSet.GetImageSize(_img, out w, out h);
            win.SetPart(0, 0, h.I - 1, w.I - 1);
            win.DispObj(_img);

            // vẽ ROI
            win.SetColor("yellow");
            win.SetDraw("margin");
            win.SetLineWidth(3);
            win.DispRectangle1(InputRoi.Y, InputRoi.X, InputRoi.Y + InputRoi.Height, InputRoi.X + InputRoi.Width);
            win.SetLineWidth(1);

            if (!r.Found)
            {
                MessageBox.Show(string.IsNullOrEmpty(r.Error)
                    ? $"Không tìm thấy code ({r.TimeMs:0.0} ms)"
                    : $"Lỗi đọc code: {r.Error}");
                return;
            }

            // vẽ contour code
            win.SetColor("green");
            win.SetDraw("margin");
            win.DispObj(r.SymbolXLDs);

            // hiện text
            MessageBox.Show($"FOUND ({r.TimeMs:0.0} ms): {r.DecodedStrings[0]}");

            r.SymbolXLDs?.Dispose();
        }
    }
}
