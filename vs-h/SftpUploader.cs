using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Renci.SshNet;

namespace vs_h
{
    public class SftpUploader
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _user;
        private readonly string _pass;

        public SftpUploader(model.LogServerConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            _host = config.HostName ?? "";
            _user = config.UserName ?? "";
            _pass = config.PassWork ?? "";

            if (!int.TryParse(config.PortNumber, out _port))
                _port = 4422; // default
        }

        /// <summary>
        /// Upload ảnh lên server với cấu trúc: Hung_AT/VS_H/LOG_IMG/{date}/{OK|NG}/{SN}_{timestamp}/{fileName}
        /// </summary>
        /// <param name="image">Bitmap cần upload</param>
        /// <param name="qrSerialNumber">Serial number từ QR code</param>
        /// <param name="cameraSN">Serial number của camera</param>
        /// <param name="isPass">true = OK folder, false = NG folder</param>
        /// <param name="testTimestamp">Timestamp của lần test (HHmmss), nếu null sẽ tạo mới</param>
        /// <returns>Remote path nếu thành công, null nếu thất bại</returns>
        public string UploadImage(Bitmap image, string qrSerialNumber, string cameraSN, bool isPass, string testTimestamp = null)
        {
            if (image == null || string.IsNullOrWhiteSpace(qrSerialNumber))
                return null;

            try
            {
                // Tạo tên file: {cameraSN}.png
                string fileName = $"{cameraSN}.png";

                // Sử dụng timestamp được truyền vào, hoặc tạo mới nếu null
                string time = testTimestamp ?? DateTime.Now.ToString("HHmmss");

                // Tạo đường dẫn remote: Hung_AT/VS_H/LOG_IMG/20231226/OK/ABC123456789_143022/CAM001.png
                string date = DateTime.Now.ToString("yyyyMMdd");
                string okNg = isPass ? "OK" : "NG";
                string folderWithTime = $"{qrSerialNumber}_{time}";
                string remotePath = $"Hung_AT/VS_H/LOG_IMG/{date}/{okNg}/{folderWithTime}/{fileName}";

                // Upload
                return UploadBitmapToSftp(image, remotePath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SFTP] Upload failed: {ex.Message}");
                return null;
            }
        }

        private string UploadBitmapToSftp(Bitmap bitmap, string remotePath)
        {
            if (string.IsNullOrWhiteSpace(_host) || string.IsNullOrWhiteSpace(_user))
            {
                System.Diagnostics.Debug.WriteLine("[SFTP] Missing host or username");
                return null;
            }

            try
            {
                using (var client = new SftpClient(_host, _port, _user, _pass))
                {
                    client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(10);
                    client.Connect();

                    if (!client.IsConnected)
                    {
                        System.Diagnostics.Debug.WriteLine("[SFTP] Connection failed");
                        return null;
                    }

                    // Tạo các folder cha nếu chưa tồn tại
                    CreateRemoteDirectoryRecursive(client, Path.GetDirectoryName(remotePath).Replace("\\", "/"));

                    // Convert Bitmap sang MemoryStream
                    using (var ms = new MemoryStream())
                    {
                        bitmap.Save(ms, ImageFormat.Png);
                        ms.Position = 0;

                        // Upload
                        client.UploadFile(ms, remotePath, true); // true = overwrite nếu đã tồn tại
                    }

                    client.Disconnect();

                    System.Diagnostics.Debug.WriteLine($"[SFTP] Uploaded: {remotePath}");
                    return remotePath;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SFTP] Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Tạo folder recursive trên SFTP server (giống mkdir -p)
        /// </summary>
        private void CreateRemoteDirectoryRecursive(SftpClient client, string remotePath)
        {
            if (string.IsNullOrWhiteSpace(remotePath) || remotePath == "/")
                return;

            remotePath = remotePath.Replace("\\", "/");

            // Tách các phần của path
            string[] parts = remotePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string current = "";

            foreach (string part in parts)
            {
                current += "/" + part;

                try
                {
                    // Kiểm tra folder có tồn tại không
                    if (!client.Exists(current))
                    {
                        client.CreateDirectory(current);
                        System.Diagnostics.Debug.WriteLine($"[SFTP] Created directory: {current}");
                    }
                }
                catch
                {
                    // Folder đã tồn tại hoặc lỗi khác, bỏ qua
                }
            }
        }

        /// <summary>
        /// Test connection đến SFTP server
        /// </summary>
        public bool TestConnection(out string error)
        {
            error = null;

            if (string.IsNullOrWhiteSpace(_host))
            {
                error = "Host name is empty";
                return false;
            }

            if (string.IsNullOrWhiteSpace(_user))
            {
                error = "User name is empty";
                return false;
            }

            try
            {
                using (var client = new SftpClient(_host, _port, _user, _pass))
                {
                    client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(10);
                    client.Connect();

                    if (!client.IsConnected)
                    {
                        error = "Connection failed";
                        return false;
                    }

                    client.Disconnect();
                    return true;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}