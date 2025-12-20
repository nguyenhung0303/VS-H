// File: ImageLoader.cs

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace vs_h
{
    public class ImageLoader
    {
        private readonly PictureBox _pictureBox;
        private readonly Form _parentForm;

        public ImageLoader(PictureBox pictureBox, Form parentForm)
        {
            _pictureBox = pictureBox;
            _parentForm = parentForm;
        }
        public string ConvertImageToBase64(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
            {
                return string.Empty;
            }

            try
            {
                // Đọc toàn bộ file ảnh thành mảng bytes
                byte[] imageBytes = File.ReadAllBytes(imagePath);

                // Chuyển mảng bytes thành chuỗi Base64
                string base64String = Convert.ToBase64String(imageBytes);

                return base64String;
            }
            catch { return string.Empty; }
        }
        public string LoadImageFromFile()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                // Bộ lọc file ảnh
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                ofd.Title = "Chọn ảnh để tải (Mô phỏng Chụp ảnh)";

                if (ofd.ShowDialog(_parentForm) == DialogResult.OK)
                {
                    string imagePath = ofd.FileName;
                    try
                    {
                        // 1. Tải và hiển thị ảnh (Sử dụng kỹ thuật sao chép để tránh khóa file gốc)
                        using (var bmpTemp = new Bitmap(imagePath))
                        {
                            // Đặt ảnh mới vào PictureBox
                            _pictureBox.Image = new Bitmap(bmpTemp);
                        }

                        // 2. Cấu hình hiển thị
                        _pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                        MessageBox.Show($"Đã tải và hiển thị ảnh thành công từ: {imagePath}", "Tải ảnh");

                        return imagePath; // Trả về đường dẫn ảnh vừa tải
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi tải hoặc hiển thị ảnh: " + ex.Message, "Lỗi");
                        return string.Empty;
                    }
                }
                return string.Empty;
            }
        }
    }
}