using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace vs_h
{
    /// <summary>
    /// Class quản lý việc lưu log ảnh theo cấu trúc:
    /// LOG/Ngày/OK hoặc NG/ảnh
    /// </summary>
    public class LogManager
    {
        private readonly string _baseLogPath;

        public LogManager(string baseLogPath = null)
        {
            // Nếu không truyền path, mặc định là thư mục LOG trong StartupPath
            _baseLogPath = baseLogPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LOG");
        }

        /// <summary>
        /// Lưu ảnh vào thư mục LOG với cấu trúc: LOG/Ngày/OK hoặc NG/
        /// </summary>
        /// <param name="image">Ảnh cần lưu</param>
        /// <param name="qrSerialNumber">Serial number từ QR code (dùng làm tên file)</param>
        /// <param name="cameraSN">Serial number của camera</param>
        /// <param name="isPass">True = OK, False = NG</param>
        /// <param name="povName">Tên POV (optional, để phân biệt nếu nhiều POV)</param>
        /// <returns>Đường dẫn file đã lưu, hoặc null nếu thất bại</returns>
        public string SaveImage(Bitmap image, string qrSerialNumber, string cameraSN, bool isPass, string povName = null)
        {
            if (image == null) return null;
            if (string.IsNullOrWhiteSpace(qrSerialNumber)) return null;

            try
            {
                // Tạo cấu trúc thư mục: LOG/yyyyMMdd/OK hoặc NG
                string dateFolder = DateTime.Now.ToString("yyyyMMdd");
                string resultFolder = isPass ? "OK" : "NG";
                string fullPath = Path.Combine(_baseLogPath, dateFolder, resultFolder);

                // Tạo thư mục nếu chưa có
                Directory.CreateDirectory(fullPath);

                // Tạo tên file: QR_SN_Camera_POV_timestamp.png
                string timestamp = DateTime.Now.ToString("HHmmss_fff");
                string fileName = $"{SanitizeFileName(qrSerialNumber)}";

                if (!string.IsNullOrWhiteSpace(cameraSN))
                    fileName += $"_{SanitizeFileName(cameraSN)}";

                if (!string.IsNullOrWhiteSpace(povName))
                    fileName += $"_{SanitizeFileName(povName)}";

                fileName += $"_{timestamp}.png";

                string filePath = Path.Combine(fullPath, fileName);

                // Lưu ảnh
                image.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

                return filePath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LogManager SaveImage Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Lưu nhiều ảnh cùng lúc
        /// </summary>
        public List<string> SaveImages(Dictionary<string, (Bitmap Image, string CameraSN, bool IsPass, string PovName)> imagesToSave, string qrSerialNumber)
        {
            var savedPaths = new List<string>();

            if (imagesToSave == null || imagesToSave.Count == 0) return savedPaths;
            if (string.IsNullOrWhiteSpace(qrSerialNumber)) return savedPaths;

            foreach (var kvp in imagesToSave)
            {
                string path = SaveImage(kvp.Value.Image, qrSerialNumber, kvp.Value.CameraSN, kvp.Value.IsPass, kvp.Value.PovName);
                if (!string.IsNullOrWhiteSpace(path))
                    savedPaths.Add(path);
            }

            return savedPaths;
        }

        /// <summary>
        /// Xóa các file log cũ hơn số ngày chỉ định
        /// </summary>
        /// <param name="daysToKeep">Số ngày giữ lại (mặc định 30 ngày)</param>
        public void CleanOldLogs(int daysToKeep = 30)
        {
            try
            {
                if (!Directory.Exists(_baseLogPath)) return;

                DateTime cutoffDate = DateTime.Now.AddDays(-daysToKeep);

                foreach (var dateFolder in Directory.GetDirectories(_baseLogPath))
                {
                    string folderName = Path.GetFileName(dateFolder);

                    // Kiểm tra xem tên thư mục có phải là ngày không (yyyyMMdd)
                    if (DateTime.TryParseExact(folderName, "yyyyMMdd", null,
                        System.Globalization.DateTimeStyles.None, out DateTime folderDate))
                    {
                        if (folderDate < cutoffDate)
                        {
                            Directory.Delete(dateFolder, true);
                            System.Diagnostics.Debug.WriteLine($"Deleted old log folder: {folderName}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LogManager CleanOldLogs Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy thống kê số lượng ảnh OK/NG theo ngày
        /// </summary>
        public Dictionary<string, (int OK, int NG)> GetStatistics(DateTime? date = null)
        {
            var stats = new Dictionary<string, (int OK, int NG)>();

            try
            {
                if (!Directory.Exists(_baseLogPath)) return stats;

                var dateFolders = Directory.GetDirectories(_baseLogPath);

                foreach (var dateFolder in dateFolders)
                {
                    string folderName = Path.GetFileName(dateFolder);

                    if (date.HasValue)
                    {
                        string targetDate = date.Value.ToString("yyyyMMdd");
                        if (folderName != targetDate) continue;
                    }

                    int okCount = 0;
                    int ngCount = 0;

                    string okPath = Path.Combine(dateFolder, "OK");
                    string ngPath = Path.Combine(dateFolder, "NG");

                    if (Directory.Exists(okPath))
                        okCount = Directory.GetFiles(okPath, "*.png").Length;

                    if (Directory.Exists(ngPath))
                        ngCount = Directory.GetFiles(ngPath, "*.png").Length;

                    stats[folderName] = (okCount, ngCount);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LogManager GetStatistics Error: {ex.Message}");
            }

            return stats;
        }

        /// <summary>
        /// Làm sạch tên file (loại bỏ ký tự không hợp lệ)
        /// </summary>
        private string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return "unnamed";

            var invalidChars = Path.GetInvalidFileNameChars();
            string sanitized = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));

            // Giới hạn độ dài tên file
            if (sanitized.Length > 50)
                sanitized = sanitized.Substring(0, 50);

            return sanitized;
        }

        /// <summary>
        /// Lấy đường dẫn thư mục log hiện tại (theo ngày)
        /// </summary>
        public string GetTodayLogPath(bool isPass)
        {
            string dateFolder = DateTime.Now.ToString("yyyyMMdd");
            string resultFolder = isPass ? "OK" : "NG";
            return Path.Combine(_baseLogPath, dateFolder, resultFolder);
        }
    }
}