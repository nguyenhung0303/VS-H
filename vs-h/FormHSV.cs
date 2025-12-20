using HalconDotNet;
using System;
using System.Drawing;
using System.Windows.Forms;
using static vs_h.model;

namespace vs_h
{
    public partial class FormHSV : Form
    {
        // =========================
        // IMAGE
        // =========================
        private string _imagePath;
        private HImage _img;

        // =========================
        // ROI DRAWING
        // =========================
        private bool _isDrawingRoi = false;
        private double _startRow, _startCol;
        private double _endRow, _endCol;
        private HRegion _roiRegion = null;

        public SMD TargetSmd { get; set; }

        public string InputImagePath { get; set; }
        public ROI InputRoi { get; set; }

        public FormHSV()
        {
            InitializeComponent();

            // Slider HSV
            tbSMin.Minimum = 0;
            tbSMin.Maximum = 255;
            tbSMin.Value = 100;

            tbSMax.Minimum = 0;
            tbSMax.Maximum = 255;
            tbSMax.Value = 255;

            tbHMin.Minimum = 0;
            tbHMin.Maximum = 255;
            tbHMin.Value = 20;

            tbHMax.Minimum = 0;
            tbHMax.Maximum = 255;
            tbHMax.Value = 50;

            tbSMin.Scroll += (_, __) => RunHSV();
            tbSMax.Scroll += (_, __) => RunHSV();
            tbHMin.Scroll += (_, __) => RunHSV();
            tbHMax.Scroll += (_, __) => RunHSV();

            // Mouse ROI
            hWindowControl1.HMouseDown += HWindowControl1_HMouseDown;
            hWindowControl1.HMouseMove += HWindowControl1_HMouseMove;
            hWindowControl1.HMouseUp += HWindowControl1_HMouseUp;
        }

        // =========================
        // LOAD IMAGE
        // =========================
        private void btnImgTest_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.tif";
                if (ofd.ShowDialog() != DialogResult.OK) return;

                _imagePath = ofd.FileName;

                _img?.Dispose();
                _img = new HImage();
                _img.ReadImage(_imagePath);

                DrawView(false);
            }
        }

        // =========================
        // HSV PROCESS
        // =========================
        private void RunHSV()
        {
            if (_img == null) return;

            DrawView(false); // vẽ ảnh + ROI trước

            double area;
            HObject mask = CreateHsvMask(out area);

            if (mask != null)
            {
                var win = hWindowControl1.HalconWindow;
                win.SetColor("green");
                win.SetDraw("fill");
                win.DispObj(mask);
            }
        }

        // =========================
        // DRAW VIEW
        // =========================
        private void DrawView(bool drawTempRoi)
        {
            if (_img == null) return;

            var win = hWindowControl1.HalconWindow;
            win.ClearWindow();

            HTuple w, h;
            HOperatorSet.GetImageSize(_img, out w, out h);
            win.SetPart(0, 0, h.I - 1, w.I - 1);

            win.DispObj(_img);

            // temp ROI
            if (drawTempRoi)
            {
                win.SetColor("cyan");
                win.SetDraw("margin");
                win.DispRectangle1(
                    Math.Min(_startRow, _endRow),
                    Math.Min(_startCol, _endCol),
                    Math.Max(_startRow, _endRow),
                    Math.Max(_startCol, _endCol)
                );
            }

            // final ROI
            if (_roiRegion != null)
            {
                win.SetColor("yellow");
                win.SetDraw("margin");
                win.DispObj(_roiRegion);
            }
        }

        // =========================
        // ROI MOUSE EVENTS
        // =========================
        private void HWindowControl1_HMouseDown(object sender, HMouseEventArgs e)
        {
            if (_img == null) return;
            if (e.Button != MouseButtons.Left) return;

            _isDrawingRoi = true;
            _startRow = e.Y;
            _startCol = e.X;
            _endRow = _startRow;
            _endCol = _startCol;
        }

        private void HWindowControl1_HMouseMove(object sender, HMouseEventArgs e)
        {
            if (!_isDrawingRoi) return;

            _endRow = e.Y;
            _endCol = e.X;
            DrawView(true);
        }

        private void btnCheckTestHSV_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(txtHsvTestScoreMin.Text, out double minScore) ||
        !double.TryParse(txtHsvTestScoreMax.Text, out double maxScore))
            {
                MessageBox.Show("Min / Max không hợp lệ");
                return;
            }

            if (minScore > maxScore)
            {
                MessageBox.Show("Min phải <= Max");
                return;
            }

            // tạo mask + score
            double score;
            HObject mask = CreateHsvMask(out score);

            txtHsvTestScore.Text = score.ToString("0");

            bool isPass = (score >= minScore && score <= maxScore);

            // PASS/FAIL textbox
            if (isPass)
            {
                txtResult.Text = "PASS";
                txtResult.BackColor = Color.LimeGreen;
                txtResult.ForeColor = Color.Black;
            }
            else
            {
                txtResult.Text = "FAIL";
                txtResult.BackColor = Color.Red;
                txtResult.ForeColor = Color.White;
            }

            // ✅ Vẽ: PASS -> ROI xanh, FAIL -> ROI đỏ, luôn hiện mask
            DrawCheckResult(isPass, mask);
        }


        private void HWindowControl1_HMouseUp(object sender, HMouseEventArgs e)
        {
            if (!_isDrawingRoi) return;

            _isDrawingRoi = false;
            _endRow = e.Y;
            _endCol = e.X;

            double row1 = Math.Min(_startRow, _endRow);
            double col1 = Math.Min(_startCol, _endCol);
            double row2 = Math.Max(_startRow, _endRow);
            double col2 = Math.Max(_startCol, _endCol);

            if (row2 - row1 < 5 || col2 - col1 < 5)
            {
                _roiRegion?.Dispose();
                _roiRegion = null;
                DrawView(false);
                return;
            }

            _roiRegion?.Dispose();
            _roiRegion = new HRegion();
            _roiRegion.GenRectangle1(row1, col1, row2, col2);

            DrawView(false);
            RunHSV();
        }

        private void FormHSV_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputImagePath))
                return;

            // Load ảnh
            _imagePath = InputImagePath;

            _img?.Dispose();
            _img = new HImage();
            _img.ReadImage(_imagePath);

            // Nếu có ROI thì dựng ROI
            if (InputRoi != null && InputRoi.Width > 0 && InputRoi.Height > 0)
            {
                _roiRegion?.Dispose();
                _roiRegion = new HRegion();
                _roiRegion.GenRectangle1(
                    InputRoi.Y,
                    InputRoi.X,
                    InputRoi.Y + InputRoi.Height,
                    InputRoi.X + InputRoi.Width
                );
            }

            // --- 2) ✅ ĐỔ HSV ĐÃ LƯU LÊN UI ---
            if (TargetSmd != null && TargetSmd.Algorithm == "HSV" && TargetSmd.HSV != null)
            {
                // clamp để tránh vượt min/max của trackbar
                tbHMin.Value = Clamp(tbHMin, TargetSmd.HSV.HMin);
                tbHMax.Value = Clamp(tbHMax, TargetSmd.HSV.HMax);
                tbSMin.Value = Clamp(tbSMin, TargetSmd.HSV.SMin);
                tbSMax.Value = Clamp(tbSMax, TargetSmd.HSV.SMax);

                txtHsvTestScoreMin.Text = TargetSmd.HSV.ScoreMin.ToString();
                txtHsvTestScoreMax.Text = TargetSmd.HSV.ScoreMax.ToString();
            }

            DrawView(false);
            RunHSV();
        }

        private void btnSaveHsv_Click(object sender, EventArgs e)
        {
            if (TargetSmd == null)
            {
                MessageBox.Show("Không có SMD để lưu");
                return;
            }

            // Lấy giá trị từ slider
            int hMin = tbHMin.Value;
            int hMax = tbHMax.Value;
            int sMin = tbSMin.Value;
            int sMax = tbSMax.Value;

            if (hMin > hMax) (hMin, hMax) = (hMax, hMin);
            if (sMin > sMax) (sMin, sMax) = (sMax, sMin);

            // Lấy score từ textbox (hoặc tính sẵn)
            if (!double.TryParse(txtHsvTestScoreMin.Text, out double scoreMin) ||
                !double.TryParse(txtHsvTestScoreMax.Text, out double scoreMax))
            {
                MessageBox.Show("Score Min / Max không hợp lệ");
                return;
            }

            // ✅ GHI VÀO SMD
            TargetSmd.Algorithm = "HSV";

            TargetSmd.HSV.HMin = hMin;
            TargetSmd.HSV.HMax = hMax;

            TargetSmd.HSV.SMin = sMin;
            TargetSmd.HSV.SMax = sMax;

            TargetSmd.HSV.ScoreMin = scoreMin;
            TargetSmd.HSV.ScoreMax = scoreMax;

            // Thông báo + đóng form
            MessageBox.Show("Đã lưu HSV vào SMD");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private HObject CreateHsvMask(out double area)
        {
            area = 0;

            if (_img == null || _roiRegion == null)
                return null;

            int sMin = tbSMin.Value;
            int sMax = tbSMax.Value;
            int hMin = tbHMin.Value;
            int hMax = tbHMax.Value;

            if (sMin > sMax) (sMin, sMax) = (sMax, sMin);
            if (hMin > hMax) (hMin, hMax) = (hMax, hMin);

            // Reduce theo ROI
            HImage imgRoi = _img.ReduceDomain(_roiRegion);

            // RGB -> HSV
            HObject r, g, b;
            HOperatorSet.Decompose3(imgRoi, out r, out g, out b);

            HObject h, s, v;
            HOperatorSet.TransFromRgb(r, g, b, out h, out s, out v, "hsv");

            // Threshold Saturation
            HObject regSat;
            HOperatorSet.Threshold(s, out regSat, sMin, sMax);

            // Reduce Hue theo Saturation
            HObject hSat;
            HOperatorSet.ReduceDomain(h, regSat, out hSat);

            // Threshold Hue
            HObject regColor;
            HOperatorSet.Threshold(hSat, out regColor, hMin, hMax);

            // Lấy blob lớn nhất
            HObject conn, sel;
            HOperatorSet.Connection(regColor, out conn);
            HOperatorSet.SelectShapeStd(conn, out sel, "max_area", 0);

            // Tính area
            HTuple num;
            HOperatorSet.CountObj(sel, out num);

            if (num.I > 0)
            {
                HTuple a, r0, c0;
                HOperatorSet.AreaCenter(sel, out a, out r0, out c0);
                area = a[0].D;
            }

            return sel; // TRẢ VỀ MASK HSV
        }

        private void hWindowControl1_HMouseMove_1(object sender, HMouseEventArgs e)
        {

        }

        private void DrawCheckResult(bool isPass, HObject mask)
        {
            // vẽ ảnh + ROI trước
            DrawView(false);

            var win = hWindowControl1.HalconWindow;

            // 1) Vẽ MASK (luôn vẽ)
            if (mask != null)   
            {
                win.SetColor("green");     // mask luôn 1 màu cho dễ nhìn (bạn đổi tùy ý)
                win.SetDraw("fill");       // đậm
                win.DispObj(mask);
            }

            // 2) Vẽ ROI viền theo PASS/FAIL (đè lên trên)
            if (_roiRegion != null)
            {
                win.SetColor(isPass ? "green" : "red");
                win.SetDraw("margin");
                win.SetLineWidth(4);       // làm viền ROI dày cho rõ (2–6 tùy bạn)
                win.DispObj(_roiRegion);
                win.SetLineWidth(1);       // reset lại (tránh ảnh hưởng chỗ khác)
            }
        }
        private int Clamp(TrackBar tb, int value)
        {
            if (value < tb.Minimum) return tb.Minimum;
            if (value > tb.Maximum) return tb.Maximum;
            return value;
        }



    }
}
