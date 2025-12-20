using System;
using System.Drawing;
using System.Windows.Forms;
using static vs_h.model;

namespace vs_h
{
    /// <summary>
    /// Quản lý vẽ ROI cho SMD trên PictureBox (SizeMode = Zoom).
    /// - ROI lưu theo tọa độ ảnh gốc (pixel).
    /// - Vẽ bằng PictureBox.Paint để không mất khi repaint.
    /// </summary>
    public class RoiDrawer
    {
        private readonly PictureBox _pb;

        public SMD SelectedSmd { get; set; } = null;

        /// <summary>Cho phép hiện menu/vẽ khi có ảnh.</summary>
        public bool ImageAvailable => _pb.Image != null;

        private bool _drawMode = false;
        private bool _isDrawing = false;

        private Point _startPt;
        private Rectangle _tempRect; // rect đang kéo (tọa độ pictureBox)


        public event Action RoiCommitted;
        public RoiDrawer(PictureBox pictureBox)
        {
            _pb = pictureBox ?? throw new ArgumentNullException(nameof(pictureBox));

            // Khuyến nghị dùng Zoom để map chuẩn
            // _pb.SizeMode = PictureBoxSizeMode.Zoom;

            _pb.Paint += Pb_Paint;
            _pb.MouseDown += Pb_MouseDown;
            _pb.MouseMove += Pb_MouseMove;
            _pb.MouseUp += Pb_MouseUp;
        }

        public void EnableDrawMode()
        {
            if (SelectedSmd == null) return;
            if (_pb.Image == null) return;

            _drawMode = true;
            _isDrawing = false;
            _tempRect = Rectangle.Empty;
        }

        public void DisableDrawMode()
        {
            _drawMode = false;
            _isDrawing = false;
            _tempRect = Rectangle.Empty;
            _pb.Invalidate();
        }

        public void ResetSelectedRoi()
        {
            if (SelectedSmd == null) return;

            SelectedSmd.ROI.X = 0;
            SelectedSmd.ROI.Y = 0;
            SelectedSmd.ROI.Width = 0;
            SelectedSmd.ROI.Height = 0;

            DisableDrawMode();
        }

        private void Pb_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (!_drawMode) return;
            if (SelectedSmd == null || _pb.Image == null) return;

            _isDrawing = true;
            _startPt = e.Location;
            _tempRect = Rectangle.Empty;
        }

        private void Pb_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDrawing) return;

            _tempRect = MakeRect(_startPt, e.Location);
            _pb.Invalidate();
        }

        private void Pb_MouseUp(object sender, MouseEventArgs e)
        {

            if (e.Button != MouseButtons.Left) return;
            if (!_isDrawing) return;

            _isDrawing = false;

            if (_tempRect.Width < 5 || _tempRect.Height < 5)
            {
                _tempRect = Rectangle.Empty;
                _pb.Invalidate();
                return;
            }

            // Convert rect từ pictureBox -> ảnh gốc (pixel)
            RectangleF imgRect = PictureBoxRectToImageRect(_tempRect);
            if (imgRect.Width <= 0 || imgRect.Height <= 0)
            {
                _tempRect = Rectangle.Empty;
                _pb.Invalidate();
                return;
            }

            // Save vào ROI của SMD
            SelectedSmd.ROI.X = imgRect.X;
            SelectedSmd.ROI.Y = imgRect.Y;
            SelectedSmd.ROI.Width = imgRect.Width;
            SelectedSmd.ROI.Height = imgRect.Height;

            RoiCommitted?.Invoke();

            // Vẽ xong thì tắt draw mode
            _drawMode = false;
            _tempRect = Rectangle.Empty;

            _pb.Invalidate();
        }

        private void Pb_Paint(object sender, PaintEventArgs e)
        {
            if (_pb.Image == null) return;

            // Vẽ ROI đã lưu của SMD đang chọn
            if (SelectedSmd != null && SelectedSmd.ROI.Width > 0 && SelectedSmd.ROI.Height > 0)
            {
                RectangleF pbRect = ImageRectToPictureBoxRect(new RectangleF(
                    (float)SelectedSmd.ROI.X,
                    (float)SelectedSmd.ROI.Y,
                    (float)SelectedSmd.ROI.Width,
                    (float)SelectedSmd.ROI.Height
                ));

                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(
                        pen,
                        pbRect.X,
                        pbRect.Y,
                        pbRect.Width,
                        pbRect.Height
                    );
                }
            }

            // Vẽ rect tạm khi đang kéo
            if (_isDrawing && !_tempRect.IsEmpty)
            {
                using (Pen pen = new Pen(Color.Lime, 2))
                {
                    e.Graphics.DrawRectangle(pen, _tempRect);
                }
            }
        }

        private static Rectangle MakeRect(Point a, Point b)
        {
            int x = Math.Min(a.X, b.X);
            int y = Math.Min(a.Y, b.Y);
            int w = Math.Abs(a.X - b.X);
            int h = Math.Abs(a.Y - b.Y);
            return new Rectangle(x, y, w, h);
        }

        // ===== Mapping Zoom =====

        private RectangleF GetImageDisplayRectangle()
        {
            var img = _pb.Image;
            if (img == null) return RectangleF.Empty;

            float pbW = _pb.ClientSize.Width;
            float pbH = _pb.ClientSize.Height;

            float imgW = img.Width;
            float imgH = img.Height;

            float ratio = Math.Min(pbW / imgW, pbH / imgH);
            float drawW = imgW * ratio;
            float drawH = imgH * ratio;

            float x = (pbW - drawW) / 2f;
            float y = (pbH - drawH) / 2f;

            return new RectangleF(x, y, drawW, drawH);
        }

        private RectangleF PictureBoxRectToImageRect(Rectangle pbRect)
        {
            var img = _pb.Image;
            if (img == null) return RectangleF.Empty;

            RectangleF display = GetImageDisplayRectangle();

            RectangleF r = RectangleF.Intersect(pbRect, display);
            if (r.Width <= 0 || r.Height <= 0) return RectangleF.Empty;

            float scaleX = img.Width / display.Width;
            float scaleY = img.Height / display.Height;

            float x = (r.X - display.X) * scaleX;
            float y = (r.Y - display.Y) * scaleY;
            float w = r.Width * scaleX;
            float h = r.Height * scaleY;

            return new RectangleF(x, y, w, h);
        }

        private RectangleF ImageRectToPictureBoxRect(RectangleF imgRect)
        {
            var img = _pb.Image;
            if (img == null) return RectangleF.Empty;

            RectangleF display = GetImageDisplayRectangle();

            float scaleX = display.Width / img.Width;
            float scaleY = display.Height / img.Height;

            float x = display.X + imgRect.X * scaleX;
            float y = display.Y + imgRect.Y * scaleY;
            float w = imgRect.Width * scaleX;
            float h = imgRect.Height * scaleY;

            return new RectangleF(x, y, w, h);
        }
    }
}
