using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace vs_h
{
    public sealed class RunLogSaver
    {
        private readonly string _root;

        public RunLogSaver(string rootFolder)
        {
            _root = rootFolder;
        }

        public string SaveHsvImageOnly(DateTime now, string qrText, bool isOk, Bitmap image)
        {
            if (image == null) return null;

            string dayFolder = Path.Combine(_root, now.ToString("yyyyMMdd"));
            string okNgFolder = Path.Combine(dayFolder, isOk ? "OK" : "NG");
            Directory.CreateDirectory(okNgFolder);

            string safeName = SanitizeFileName(string.IsNullOrWhiteSpace(qrText) ? "NO_QR" : qrText);

            // tránh ghi đè nếu test nhiều lần cùng QR
            string fileName = safeName + "_" + now.ToString("HHmmss_fff") + ".png";
            string fullPath = Path.Combine(okNgFolder, fileName);

            // lưu PNG
            image.Save(fullPath, ImageFormat.Png);
            return fullPath;
        }

        private static string SanitizeFileName(string name)
        {
            var invalid = Path.GetInvalidFileNameChars();
            string s = new string(name.Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray());
            s = s.Trim();
            if (s.Length == 0) s = "NO_NAME";
            // (tuỳ chọn) hạn chế độ dài file name
            if (s.Length > 120) s = s.Substring(0, 120);
            return s;
        }
    }
}
