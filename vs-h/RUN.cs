using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cong1;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO.Ports;
using WinSCP;

namespace vs_h
{
    public partial class RUN : Form
    {
        private class CamInfo
        {
            public MindVisionCamera Cam { get; set; }
            public string SN { get; set; }
            public PictureBox Pb { get; set; }
        }

        private readonly object _fileLogLock = new object();
        private string _logDir;

        private string GetTodayLogPath()
        {
            string date = DateTime.Now.ToString("yyyyMMdd");
            return Path.Combine(_logDir, $"{date}.txt");
        }

        private void AppendLogToFile(string line)
        {
            try
            {
                lock (_fileLogLock)
                {
                    File.AppendAllText(GetTodayLogPath(), line + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch
            {
                // tránh crash nếu lỗi ghi file
            }
        }

        private SerialPort _sp;
        private readonly StringBuilder _rxBuf = new StringBuilder();

        // dùng để chờ phản hồi sau khi gửi
        private readonly object _waitLock = new object();
        private TaskCompletionSource<string> _waitTcs; // sẽ set "PASS"/"FAIL"

        private LogManager _logManager;
        private string _currentQrSerialNumber = null;

        private readonly List<CamInfo> _camInfos = new List<CamInfo>();

        private Dictionary<string, double> _snToExposure = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);



        public RUN()
        {
            InitializeComponent();
            this.Load += RUN_Load;

            this.FormClosing += RUN_FormClosing;
            this.KeyPreview = true;
            this.KeyDown += RUN_KeyDown;

            _logManager = new LogManager();
        }

        

        private Dictionary<string, double> LoadExposureMapFromModels()
        {
            var map = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

            string modelsDir = Path.GetFullPath(Path.Combine(Application.StartupPath, "..", "..", "Models"));
            if (!Directory.Exists(modelsDir)) return map;

            foreach (var file in Directory.GetFiles(modelsDir, "*.json"))
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var m = JsonConvert.DeserializeObject<model.Model>(json);
                    if (m == null || m.POVs == null) continue;

                    foreach (var pov in m.POVs)
                    {
                        if (pov == null) continue;
                        if (string.IsNullOrWhiteSpace(pov.CameraSN)) continue;

                        // Nếu 1 SN xuất hiện nhiều lần ở nhiều model/POV:
                        // ✅ Ở đây mình chọn "giá trị cuối cùng đọc được"
                        map[pov.CameraSN] = pov.ExposureTime;
                    }
                }
                catch { }
            }

            return map;
        }
        private void AppendLog(string text)
        {
            string line = $"{DateTime.Now:HH:mm:ss.fff} {text}";

            // ghi file trước (không phụ thuộc UI thread)
            AppendLogToFile(line);

            // hiển thị lên UI
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.BeginInvoke(new Action(() => AppendLog(text)));
                return;
            }

            richTextBox1.AppendText(line + "\r\n");
            richTextBox1.ScrollToCaret();
        }

        private model.LogServerConfig LoadLogServerConfigFromModels()
        {
            try
            {
                string modelsDir = Path.GetFullPath(Path.Combine(Application.StartupPath, "..", "..", "Models"));
                if (!Directory.Exists(modelsDir)) return null;

                // lấy config từ file json đầu tiên (vì bạn lưu config cho tất cả model)
                var firstFile = Directory.GetFiles(modelsDir, "*.json").FirstOrDefault();
                if (firstFile == null) return null;

                var json = File.ReadAllText(firstFile);
                var m = JsonConvert.DeserializeObject<model.Model>(json);
                if (m == null) return null;

                return m.LogServer; // <-- bạn phải có property này trong model
            }
            catch
            {
                return null;
            }
        }
        private int GetPort(model.LogServerConfig cfg)
        {
            int port = 4422;

            if (cfg == null) return port;

            // nếu PortNumber là string
            if (!string.IsNullOrWhiteSpace(cfg.PortNumber))
            {
                int p;
                if (int.TryParse(cfg.PortNumber.Trim(), out p) && p > 0) port = p;
            }

            return port;
        }

        private void UploadFileSftp(string localPath, string remotePath, model.LogServerConfig cfg)
        {
            var sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Sftp,
                HostName = cfg.HostName,
                PortNumber = GetPort(cfg),
                UserName = cfg.UserName,
                Password = cfg.PassWork,

                GiveUpSecurityAndAcceptAnySshHostKey = true
            };

            using (var session = new Session())
            {
                session.Open(sessionOptions);

                // tạo folder remote nếu cần
                EnsureRemoteDirectory(session, Path.GetDirectoryName(remotePath).Replace("\\", "/"));

                var transferOptions = new TransferOptions
                {
                    TransferMode = TransferMode.Binary
                };

                var result = session.PutFiles(localPath, remotePath, false, transferOptions);
                result.Check();
            }
        }

        private void EnsureRemoteDirectory(Session session, string remoteDir)
        {
            if (string.IsNullOrWhiteSpace(remoteDir)) return;
            remoteDir = remoteDir.Replace("\\", "/");
            if (!remoteDir.StartsWith("/")) remoteDir = "/" + remoteDir;
            if (!remoteDir.EndsWith("/")) remoteDir += "/";

            var parts = remoteDir.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string current = "/";
            for (int i = 0; i < parts.Length; i++)
            {
                current += parts[i] + "/";
                if (!session.FileExists(current))
                {
                    session.CreateDirectory(current);
                }
            }
        }

        private async Task UploadTodayLogAsync(string finalResult)
        {
            try
            {
                var cfg = LoadLogServerConfigFromModels();
                if (cfg == null)
                {
                    AppendLog("[UPLOAD] No LogServerConfig in Models");
                    return;
                }

                string localLog = GetTodayLogPath();
                if (!File.Exists(localLog))
                {
                    AppendLog("[UPLOAD] Local log not found: " + localLog);
                    return;
                }

                // remote dir: /logs/yyyy-MM-dd/
                string remoteBaseDir = "/VS_H";
                string remoteDir = remoteBaseDir.TrimEnd('/')
                 + "/LOG_txt/"
                 + DateTime.Now.ToString("yyyy-MM-dd")
                 + "/";
                //remoteDir = remoteDir.TrimEnd('/') + "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/";
                //remoteDir = remoteDir.Replace("\\", "/");
                if (!remoteDir.StartsWith("/")) remoteDir = "/" + remoteDir;
                remoteDir = remoteDir.TrimEnd('/') + "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/";

                string remoteFile = string.Format(
                    "RUN_{0}_{1}_{2}.txt",
                    Environment.MachineName,
                    DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                    string.IsNullOrWhiteSpace(finalResult) ? "NA" : finalResult
                );

                string remotePath = remoteDir + remoteFile;

                // chạy upload trên thread nền để không đơ UI
                await Task.Run(() => UploadFileSftp(localLog, remotePath, cfg));

                AppendLog("[UPLOAD] OK -> " + remotePath);
            }
            catch (Exception ex)
            {
                AppendLog("[UPLOAD-ERR] " + ex.Message);
            }
        }


        private void ApplyExposureForSn(CamInfo ci)
        {
            if (ci == null || ci.Cam == null) return;
            double exp;
            if (_snToExposure.TryGetValue(ci.SN, out exp))
            {
                ci.Cam.SetExposureTime(exp);
                Debug.WriteLine($"RUN: Apply exposure SN={ci.SN} exp={exp}");
            }
        }
        private class RoiDraw
        {
            public Rectangle Rect;
            public bool Pass;
            public string Text;
            public string Algorithm;
        }

        private Bitmap DrawRoisOnBitmap(Bitmap src, List<RoiDraw> rois)
        {
            if (src == null) return null;
            if (rois == null || rois.Count == 0) return (Bitmap)src.Clone();

            Bitmap b = (Bitmap)src.Clone();

            using (Graphics g = Graphics.FromImage(b))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                for (int i = 0; i < rois.Count; i++)
                {
                    RoiDraw r = rois[i];
                    if (r.Rect.Width <= 0 || r.Rect.Height <= 0) continue;

                    using (Pen pen = new Pen(r.Pass ? Color.Lime : Color.Red, 3))
                    {
                        g.DrawRectangle(pen, r.Rect);
                    }

                    // Vẽ tên ROI/SMD (tuỳ chọn)
                    if (!string.IsNullOrEmpty(r.Text))
                    {
                        using (Font font = new Font("Arial", 10, FontStyle.Bold))
                        {
                            SizeF sz = g.MeasureString(r.Text, font);
                            float x = r.Rect.X;
                            float y = r.Rect.Y - sz.Height - 2;
                            if (y < 0) y = r.Rect.Y + 2;

                            RectangleF bg = new RectangleF(x, y, sz.Width + 6, sz.Height + 2);
                            using (Brush bgBrush = new SolidBrush(Color.FromArgb(140, 0, 0, 0)))
                                g.FillRectangle(bgBrush, bg);

                            using (Brush txtBrush = new SolidBrush(r.Pass ? Color.Lime : Color.Red))
                                g.DrawString(r.Text, font, txtBrush, x + 3, y + 1);
                        }
                    }
                }
            }

            return b;
        }

        private void SetPictureBoxImageSafe(PictureBox pb, Bitmap bmp)
        {
            if (pb == null || bmp == null) return;

            if (pb.InvokeRequired)
            {
                pb.Invoke((Action)(() => SetPictureBoxImageSafe(pb, bmp)));
                return;
            }

            Image old = pb.Image;
            pb.Image = bmp;
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            if (old != null) old.Dispose();
        }

        private async void RUN_Load(object sender, EventArgs e)
        {
            _logDir = @"D:\LOG_VS-H\LOG_txt";
            Directory.CreateDirectory(_logDir);

            try
            {
                var devs = MindVisionCamera.GetDeviceList();
                if (devs == null || devs.Count == 0)
                {
                    // No cameras found; nothing to connect
                    return;
                }
                _snToExposure = LoadExposureMapFromModels();
                foreach (var d in devs)
                {
                    var cam = new MindVisionCamera();

                    // Create PictureBox for this camera
                    var pb = new PictureBox();
                    pb.Width = 770;
                    pb.Height = 560;
                    pb.SizeMode = PictureBoxSizeMode.Zoom;
                    pb.BorderStyle = BorderStyle.FixedSingle;
                    pb.Tag = d.SN; // store SN for reference

                    // Add label overlay if desired
                    var panel = new Panel();
                    panel.Padding = new Padding(4);
                    panel.Width = pb.Width + 8;
                    panel.Height = pb.Height + 24;

                    var lbl = new Label();
                    lbl.Text = string.IsNullOrEmpty(d.Friendly) ? d.Product : d.Friendly;
                    lbl.Dock = DockStyle.Top;
                    lbl.Height = 18;

                    panel.Controls.Add(pb);
                    panel.Controls.Add(lbl);
                    lbl.BringToFront();
                    pb.Dock = DockStyle.Bottom;

                    flowLayoutPanelRUN.Controls.Add(panel);

                    // attempt to open and start stream
                    int openRet = await Task.Run(() => cam.Open(d.SN, IntPtr.Zero));
                    if (openRet <= 0)
                    {
                        // failed to open, indicate on label
                        lbl.Text += " (open failed)";
                        continue;
                    }

                    int strRet = await Task.Run(() => cam.StartStream());
                    ApplyExposureForSn(new CamInfo { Cam = cam, SN = d.SN, Pb = pb });
                    if (strRet <= 0)
                    {
                        lbl.Text += " (stream failed)";
                        // keep camera in list though
                    }

                    // store info for later captures
                    _camInfos.Add(new CamInfo { Cam = cam, SN = d.SN, Pb = pb });

                }

                // Optionally notify user (silent if none connected)
                if (_camInfos.Count > 0)
                {
                    // small, non-blocking notification
                    Task.Run(() => { System.Diagnostics.Debug.WriteLine($"RUN: Connected {_camInfos.Count} camera(s)"); });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi kết nối camera tự động: " + ex.Message);
            }
            try
            {
                _sp = new SerialPort("COM7", 9600, Parity.None, 8, StopBits.One);
                _sp.NewLine = "\r\n";
                _sp.DataReceived += Sp_DataReceived;
                _sp.Open();

                AppendLog("[OPEN] COM7 opened");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không mở được COM7: " + ex.Message);
            }

        }
        private void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string chunk = _sp.ReadExisting();
                if (string.IsNullOrEmpty(chunk)) return;

                lock (_rxBuf)
                {
                    _rxBuf.Append(chunk);

                    while (true)
                    {
                        string all = _rxBuf.ToString();
                        int idx = all.IndexOf("\r\n", StringComparison.Ordinal);
                        if (idx < 0) break;

                        string line = all.Substring(0, idx);
                        _rxBuf.Remove(0, idx + 2);

                        AppendLog("[RX] " + line);

                        // nếu line có PASS/FAIL thì báo cho “task đang chờ”
                        if (line.IndexOf("PASS", StringComparison.OrdinalIgnoreCase) >= 0)
                            TryCompleteWait("PASS");
                        else if (line.IndexOf("FAIL", StringComparison.OrdinalIgnoreCase) >= 0)
                            TryCompleteWait("FAIL"); // optional, nếu bạn muốn xử lý FAIL
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLog("[RX-ERR] " + ex.Message);
            }
        }
        private async Task<string> SendAndWaitResultAsync(string sn)
        {
            if (_sp == null || !_sp.IsOpen)
            {
                AppendLog("[ERR] COM7 not open");
                return "NO_COM";
            }
            if (string.IsNullOrWhiteSpace(sn)) return "NO_SN";

            TaskCompletionSource<string> tcs;
            lock (_waitLock)
            {
                _waitTcs?.TrySetCanceled();
                _waitTcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
                tcs = _waitTcs;
            }

            SetResultUI("WAIT", Color.Gold, Color.Black);

            string msg = sn + new string(' ', 13) + "CHECK_CCD+++\r\n";
            _sp.Write(msg);
            AppendLog("[TX] " + msg.Replace("\r", "\\r").Replace("\n", "\\n"));

            var done = await Task.WhenAny(tcs.Task, Task.Delay(1000));

            if (done != tcs.Task)
            {
                SetResultUI("SFC", Color.OrangeRed, Color.White);
                AppendLog("-----> SFC");
                return "SFC";
            }

            string res = await tcs.Task; // PASS/FAIL

            if (string.Equals(res, "PASS", StringComparison.OrdinalIgnoreCase))
            {
                SetResultUI("PASS", Color.LimeGreen, Color.Black);
                return "PASS";
            }
            else
            {
                SetResultUI("FAIL", Color.Red, Color.White);
                return "FAIL";
            }
        }

        private void SetResultUI(string text, Color bg, Color fg)
        {
            if (txtResultRUN.InvokeRequired)
            {
                txtResultRUN.BeginInvoke(new Action(() => SetResultUI(text, bg, fg)));
                return;
            }
            txtResultRUN.Text = text;
            txtResultRUN.BackColor = bg;
            txtResultRUN.ForeColor = fg;
        }

        private void TryCompleteWait(string result)
        {
            lock (_waitLock)
            {
                _waitTcs?.TrySetResult(result);
            }
        }
        private void UpdateRunResultTextbox(int total, int fail, int pass)
        {
            // nếu gọi từ thread khác thì marshal về UI
            if (txtResultRUN.InvokeRequired)
            {
                txtResultRUN.Invoke(new Action(() => UpdateRunResultTextbox(total, fail, pass)));
                return;
            }

            if (total <= 0)
            {
                txtResultRUN.Text = "NO DATA";
                txtResultRUN.BackColor = Color.Gray;
                txtResultRUN.ForeColor = Color.White;
                return;
            }

            bool isPass = (fail == 0);
            if (isPass)
            {
                txtResultRUN.Text = "PASS";
                txtResultRUN.BackColor = Color.LimeGreen;
                txtResultRUN.ForeColor = Color.Black;
            }
            else
            {
                txtResultRUN.Text = "FAIL";
                txtResultRUN.BackColor = Color.Red;
                txtResultRUN.ForeColor = Color.White;
            }
        }

        private void RUN_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var info in _camInfos.ToList())
            {
                try
                {
                    if (info.Cam.IsConnected)
                    {
                        try { info.Cam.StopStream(); } catch { }
                        try { info.Cam.Close(); } catch { }
                    }
                }
                catch { }
            }
            _camInfos.Clear();

            // Dọn dẹp log cũ hơn 30 ngày (optional)
            try
            {
                _logManager?.CleanOldLogs(30);
            }
            catch { }

            try { if (_sp != null && _sp.IsOpen) _sp.Close(); } catch { }
            try { _sp?.Dispose(); } catch { }
        }
        //com

        private async void RUN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Space) return;

            if (_camInfos.Count == 0)
            {
                MessageBox.Show("Không có camera nào kết nối.");
                return;
            }
            SetTxtSnRUN("", append: false);
            // 1) GrabFrame song song
            var tasks = _camInfos.Select(ci => Task.Run(() => ci.Cam.GrabFrame(1000))).ToArray();

            Bitmap[] bitmaps = null;
            try { bitmaps = await Task.WhenAll(tasks); }
            catch { bitmaps = null; }

            // 2) Lưu file cho HALCON + giữ bitmap để hiển thị
            var snToFile = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var snToBitmap = new Dictionary<string, Bitmap>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < _camInfos.Count; i++)
            {
                CamInfo info = _camInfos[i];
                Bitmap bmp = (bitmaps != null && i < bitmaps.Length) ? bitmaps[i] : null;

                if (bmp == null) continue;

                // giữ clone để hiển thị (không dispose clone)
                snToBitmap[info.SN] = (Bitmap)bmp.Clone();

                try
                {
                    string dir = Path.Combine(Application.StartupPath, "Captured");
                    Directory.CreateDirectory(dir);
                    string filePath = Path.Combine(dir, $"run_{info.SN}_{DateTime.Now:yyyyMMdd_HHmmss_fff}.png");
                    bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                    snToFile[info.SN] = filePath;
                }
                catch { }

                bmp.Dispose();
            }


            // 3) Test -> lấy danh sách ROI PASS/FAIL theo SN
            Dictionary<string, List<RoiDraw>> drawMap = PerformChecksForCaptured(snToFile);
            // commm
            //if (txtResultRUN.Text == "PASS")
            //{
            //    // Lấy SN (bạn đang append nhiều dòng "CameraSN: decoded")
            //    // Tạm thời lấy dòng cuối cùng:
            //    string Sn = (txtSnRUN.Lines.LastOrDefault() ?? "").Trim();
            //    if (Sn.Length > 12) Sn = Sn.Substring(0, 12);
            //    MessageBox.Show("SN gửi đi: " + Sn);
            //    if (!string.IsNullOrWhiteSpace(Sn))
            //    {
            //        string msg = Sn + new string(' ', 13) + "CHECK_CCD+++\r\n";
            //        try { _sp?.Write(msg); } catch { }
            //    }
            //}

            // 4) Vẽ ROI lên ảnh và show lên từng PictureBox
            for (int i = 0; i < _camInfos.Count; i++)
            {
                CamInfo info = _camInfos[i];

                Bitmap baseBmp;
                if (!snToBitmap.TryGetValue(info.SN, out baseBmp)) continue;

                List<RoiDraw> rois;
                if (drawMap != null && drawMap.TryGetValue(info.SN, out rois) && rois.Count > 0)
                {
                    Bitmap annotated = DrawRoisOnBitmap(baseBmp, rois);
                    baseBmp.Dispose(); // dispose bản base vì đã có annotated
                    SetPictureBoxImageSafe(info.Pb, annotated);


                }
                else
                {
                    // không có ROI thì vẫn hiển thị ảnh gốc
                    SetPictureBoxImageSafe(info.Pb, baseBmp);
                }
            }
            e.Handled = true;

            string finalComResult = null;

            if (string.Equals(txtResultRUN.Text, "PASS", StringComparison.OrdinalIgnoreCase))
            {
                string Sn = (txtSnRUN.Lines.LastOrDefault() ?? "").Trim();

                int dash = Sn.IndexOf('-');
                if (dash >= 0) Sn = Sn.Substring(0, dash);

                if (Sn.Length > 12) Sn = Sn.Substring(0, 12);

                MessageBox.Show("SN gửi đi: " + Sn);

                finalComResult = await SendAndWaitResultAsync(Sn);   // ✅ có kết quả PASS/FAIL/SFC
            }
            else
            {
                // nếu test hình đã FAIL thì bạn vẫn muốn upload log -> gán "FAIL_IMG"
                finalComResult = "FAIL_IMG";
            }

            // ✅ Upload log sau khi đã có kết quả cuối
            await UploadTodayLogAsync(finalComResult);

            AppendLog("-------------------done-------------------");
            if (!string.IsNullOrWhiteSpace(_currentQrSerialNumber))
            {
                foreach (var info in _camInfos)
                {
                    Bitmap shot = GetPictureBoxBitmapCloneSafe(info.Pb);
                    if (shot == null) continue;

                    try
                    {
                        List<RoiDraw> rois = null; // ✅ luôn có giá trị (null) từ đầu

                        if (drawMap != null)
                            drawMap.TryGetValue(info.SN, out rois);

                        bool hasHsv = HasHsvRoi(rois);
                        bool hasQr = HasQrRoi(rois);

                        bool isPassCam = true;
                        if (rois != null && rois.Count > 0)
                            isPassCam = rois.All(r => r.Pass);

                        // ✅ 1) Có HSV thì lưu OK/NG
                        if (hasHsv)
                            _logManager.SaveImage(shot, _currentQrSerialNumber, info.SN, isPassCam, "RUN");

                        // ✅ 2) Có QR thì lưu vào folder QR
                        if (hasQr)
                            _logManager.SaveQrImage(shot, _currentQrSerialNumber, info.SN, "RUN");
                    }
                    finally
                    {
                        shot.Dispose();
                    }
                }
            }


        }


        private Dictionary<string, List<RoiDraw>> PerformChecksForCaptured(Dictionary<string, string> snToFile)
        {
            var drawMap = new Dictionary<string, List<RoiDraw>>(StringComparer.OrdinalIgnoreCase);

            // Reset QR serial number cho lần test mới
            _currentQrSerialNumber = null;

            // Dictionary để lưu ảnh HSV cần log: Key = CameraSN, Value = (Bitmap, isPass, povName)
            //var hsvImagesToLog = new Dictionary<string, List<(Bitmap Image, bool IsPass, string PovName)>>(StringComparer.OrdinalIgnoreCase);

            try
            {
                string modelsDir = Path.GetFullPath(Path.Combine(Application.StartupPath, "..", "..", "Models"));
                if (!Directory.Exists(modelsDir))
                {
                    MessageBox.Show("Thư mục Models không tồn tại.");
                    return drawMap;
                }

                int total = 0, pass = 0, fail = 0, skip = 0;
                var details = new StringBuilder();

                foreach (var file in Directory.GetFiles(modelsDir, "*.json"))
                {
                    try
                    {
                        var json = File.ReadAllText(file);
                        var model = JsonConvert.DeserializeObject<model.Model>(json);
                        if (model == null) continue;

                        foreach (var pov in model.POVs)
                        {
                            if (pov == null) continue;
                            if (string.IsNullOrWhiteSpace(pov.CameraSN)) continue;
                            if (!snToFile.ContainsKey(pov.CameraSN)) continue;

                            string imgPath = snToFile[pov.CameraSN];
                            if (!File.Exists(imgPath))
                            {
                                details.AppendLine($"[SKIP] {model.Name}/{pov.Name}: ảnh không tồn tại");
                                continue;
                            }

                            if (!drawMap.ContainsKey(pov.CameraSN))
                                drawMap[pov.CameraSN] = new List<RoiDraw>();

                            using (var img = new HalconDotNet.HImage(imgPath))

                            {
                                // Biến để track xem POV này có HSV test nào không
                                bool hasHsvTest = false;
                                bool povAllHsvPass = true; // Giả định ban đầu là PASS

                                foreach (var smd in pov.SMDs)
                                {
                                    total++;

                                    if (smd == null || !smd.IsEnabled)
                                    {
                                        skip++;
                                        details.AppendLine($"[SKIP] {model.Name}/{pov.Name}/{smd?.Name}: IsEnabled=false");
                                        continue;
                                    }

                                    if (smd.ROI == null || smd.ROI.Width <= 0 || smd.ROI.Height <= 0)
                                    {
                                        fail++;
                                        details.AppendLine($"[FAIL] {model.Name}/{pov.Name}/{smd.Name}: ROI invalid");
                                        continue;
                                    }

                                    // ===== HSV =====
                                    if (string.Equals(smd.Algorithm, "HSV", StringComparison.OrdinalIgnoreCase))
                                    {
                                        hasHsvTest = true;

                                        if (smd.HSV == null)
                                        {
                                            fail++;
                                            povAllHsvPass = false;
                                            details.AppendLine($"[FAIL] {model.Name}/{pov.Name}/{smd.Name}: HSV null");
                                            drawMap[pov.CameraSN].Add(new RoiDraw
                                            {
                                                Rect = ToRect(smd.ROI),
                                                Pass = false,
                                                Text = "HSV null"
                                            });
                                            continue;
                                        }

                                        double score = ComputeHsvScoreArea(img, smd);
                                        bool isPass = (score >= smd.HSV.ScoreMin && score <= smd.HSV.ScoreMax);

                                        if (isPass)
                                        {
                                            pass++;
                                            details.AppendLine($"[PASS] {model.Name}/{pov.Name}/{smd.Name}: HSV score={score:0}");
                                        }
                                        else
                                        {
                                            fail++;
                                            povAllHsvPass = false;
                                            details.AppendLine($"[FAIL] {model.Name}/{pov.Name}/{smd.Name}: HSV score={score:0}");
                                        }

                                        drawMap[pov.CameraSN].Add(new RoiDraw
                                        {
                                            Rect = ToRect(smd.ROI),
                                            Pass = isPass,
                                            Text = $"HSV:{score:0}",
                                            Algorithm = "HSV"
                                        });

                                        continue;
                                    }

                                    // ===== QR / CODE =====
                                    if (string.Equals(smd.Algorithm, "QR", StringComparison.OrdinalIgnoreCase) ||
                                        string.Equals(smd.Algorithm, "CODE", StringComparison.OrdinalIgnoreCase))
                                    {
                                        string decoded, err;
                                        bool ok = TryReadQrInRoi(img, smd.ROI, out decoded, out err);

                                        if (ok)
                                        {
                                            pass++;
                                            details.AppendLine($"[PASS] {model.Name}/{pov.Name}/{smd.Name}: QR='{decoded}'");

                                            // Lưu QR serial number (lấy cái đầu tiên)
                                            if (string.IsNullOrWhiteSpace(_currentQrSerialNumber))
                                                _currentQrSerialNumber = decoded;

                                            //SetTxtSnRUN($"{pov.CameraSN}: {decoded}", append: true);
                                            string sn13 = (decoded ?? "").Trim();
                                            if (sn13.Length > 12) sn13 = sn13.Substring(0, 12);

                                            SetTxtSnRUN(sn13, append: true);
                                        }
                                        else
                                        {
                                            fail++;
                                            details.AppendLine($"[FAIL] {model.Name}/{pov.Name}/{smd.Name}: QR fail ({err})");
                                        }

                                        drawMap[pov.CameraSN].Add(new RoiDraw
                                        {
                                            Rect = ToRect(smd.ROI),
                                            Pass = ok,
                                            Text = ok ? $"QR:{decoded}" : "QR:FAIL",
                                            Algorithm = "QR"
                                        });

                                        continue;
                                    }

                                    // ===== thuật toán khác =====
                                    skip++;
                                    details.AppendLine($"[SKIP] {model.Name}/{pov.Name}/{smd.Name}: Algorithm={smd.Algorithm}");
                                }

                                // Sau khi xử lý hết các SMD của POV này
                                // Nếu có HSV test, lưu ảnh vào danh sách để log
                                //if (hasHsvTest)
                                //{
                                //    try
                                //    {
                                //        // Load lại ảnh từ file để lưu
                                //        using (var bmpToLog = new Bitmap(imgPath))
                                //        {
                                //            Bitmap clonedBmp = (Bitmap)bmpToLog.Clone();

                                //            if (!hsvImagesToLog.ContainsKey(pov.CameraSN))
                                //                hsvImagesToLog[pov.CameraSN] = new List<(Bitmap, bool, string)>();

                                //            hsvImagesToLog[pov.CameraSN].Add((clonedBmp, povAllHsvPass, pov.Name));
                                //        }
                                //    }
                                //    catch (Exception ex)
                                //    {
                                //        details.AppendLine($"[WARNING] Không thể load ảnh để log: {ex.Message}");
                                //    }
                                //}
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        details.AppendLine($"Error model file {Path.GetFileName(file)}: {ex.Message}");
                    }
                }

                UpdateRunResultTextbox(total, fail, pass);

                // Lưu ảnh HSV vào LOG sau khi đã có QR serial number
                //if (!string.IsNullOrWhiteSpace(_currentQrSerialNumber) && hsvImagesToLog.Count > 0)
                //{
                //    foreach (var kvp in hsvImagesToLog)
                //    {
                //        string cameraSN = kvp.Key;
                //        foreach (var imgInfo in kvp.Value)
                //        {
                //            try
                //            {
                //                string savedPath = _logManager.SaveImage(
                //                    imgInfo.Image,
                //                    _currentQrSerialNumber,
                //                    cameraSN,
                //                    imgInfo.IsPass,
                //                    imgInfo.PovName
                //                );

                //                if (!string.IsNullOrWhiteSpace(savedPath))
                //                {
                //                    details.AppendLine($"[LOG] Saved: {Path.GetFileName(savedPath)}");
                //                }
                //            }
                //            catch (Exception ex)
                //            {
                //                details.AppendLine($"[LOG ERROR] {ex.Message}");
                //            }
                //            finally
                //            {
                //                // Dispose bitmap sau khi lưu
                //                imgInfo.Image?.Dispose();
                //            }
                //        }
                //    }
                //}
                //else if (hsvImagesToLog.Count > 0)
                //{
                //    // Nếu không có QR code, dispose tất cả bitmap
                //    foreach (var kvp in hsvImagesToLog)
                //    {
                //        foreach (var imgInfo in kvp.Value)
                //        {
                //            imgInfo.Image?.Dispose();
                //        }
                //    }
                //    details.AppendLine("[WARNING] Không lưu LOG vì không đọc được QR code");
                //}

                return drawMap;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi kiểm tra: " + ex.Message);

                // Cleanup nếu có lỗi
                //if (hsvImagesToLog != null)
                //{
                //    foreach (var kvp in hsvImagesToLog)
                //    {
                //        foreach (var imgInfo in kvp.Value)
                //        {
                //            imgInfo.Image?.Dispose();
                //        }
                //    }
                //}

                return drawMap;
            }
        }
        private bool HasHsvRoi(List<RoiDraw> rois) =>
    rois != null && rois.Any(r => string.Equals(r.Algorithm, "HSV", StringComparison.OrdinalIgnoreCase));

        private bool HasQrRoi(List<RoiDraw> rois) =>
            rois != null && rois.Any(r => string.Equals(r.Algorithm, "QR", StringComparison.OrdinalIgnoreCase));

        private void ShowLogStatistics()
        {
            var stats = _logManager.GetStatistics();

            var sb = new StringBuilder();
            sb.AppendLine("=== THỐNG KÊ LOG ===");
            sb.AppendLine();

            foreach (var kvp in stats.OrderByDescending(x => x.Key))
            {
                sb.AppendLine($"Ngày {kvp.Key}:");
                sb.AppendLine($"  OK: {kvp.Value.OK}");
                sb.AppendLine($"  NG: {kvp.Value.NG}");
                sb.AppendLine($"  Tổng: {kvp.Value.OK + kvp.Value.NG}");
                sb.AppendLine();
            }

            MessageBox.Show(sb.ToString(), "Thống kê LOG");
        }


        private double ComputeHsvScoreArea(HalconDotNet.HImage img, model.SMD smd)
        {
            // replicate same logic as in Form1
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

        private void flowLayoutPanelRUN_Paint(object sender, PaintEventArgs e)
        {
            // Designer referenced empty handler - no custom painting required.
        }

        private void RUN_Load_1(object sender, EventArgs e)
        {

        }
        private class RoiOverlay
        {
            public Rectangle Rect;
            public Color Color;
            public string Text; // có thể null
        }

        private static Rectangle ToRect(model.ROI roi)
        {
            // ROI của bạn là double -> convert sang int (tránh lỗi double->int)
            int x = (int)Math.Round(roi.X);
            int y = (int)Math.Round(roi.Y);
            int w = (int)Math.Round(roi.Width);
            int h = (int)Math.Round(roi.Height);

            if (w < 1) w = 1;
            if (h < 1) h = 1;
            return new Rectangle(x, y, w, h);
        }



        private bool TryReadQrInRoi(HalconDotNet.HImage img, model.ROI roi, out string decoded, out string err)
        {
            decoded = null;
            err = null;

            try
            {
                string codeType = "QR Code";
                var r = DataCodeReader.ReadInRoi(img, roi.X, roi.Y, roi.Width, roi.Height, codeType);

                if (r == null)
                {
                    err = "Reader trả về null";
                    return false;
                }

                if (!r.Found)
                {
                    err = string.IsNullOrWhiteSpace(r.Error) ? "Không tìm thấy QR" : r.Error;
                    return false;
                }

                decoded = (r.DecodedStrings != null && r.DecodedStrings.Count() > 0) ? r.DecodedStrings[0] : "";
                r.SymbolXLDs?.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }
        }
        private void SetTxtSnRUN(string text, bool append = false)
        {
            if (txtSnRUN.InvokeRequired)
            {
                txtSnRUN.Invoke(new Action(() => SetTxtSnRUN(text, append)));
                return;
            }

            if (!append) txtSnRUN.Clear();

            if (append && txtSnRUN.TextLength > 0)
                txtSnRUN.AppendText(Environment.NewLine);

            txtSnRUN.AppendText(text ?? "");
        }

        private void txtResultRUN_TextChanged(object sender, EventArgs e)
        {

        }

        //LOG IMG
        private Bitmap GetPictureBoxBitmapCloneSafe(PictureBox pb)
        {
            if (pb == null) return null;

            if (pb.InvokeRequired)
                return (Bitmap)pb.Invoke(new Func<Bitmap>(() => GetPictureBoxBitmapCloneSafe(pb)));

            var bmp = pb.Image as Bitmap;
            if (bmp == null) return null;

            return (Bitmap)bmp.Clone(); // ✅ clone để lưu
        }

    }
}
