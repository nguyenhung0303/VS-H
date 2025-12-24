using System;
using System.Collections.Generic;

namespace vs_h
{
    public class model
    {

        public class LogServerConfig
        {
            public string HostName { get; set; }
            public string PortNumber { get; set; } = "4422";     // default 4422
            public string UserName { get; set; }
            public string PassWork { get; set; }            // ✅ đúng tên bạn yêu cầu
            public string Protocol { get; set; } = "SFTP";  // default SFTP
        }
        // =====================
        // ROI
        // =====================
        public class ROI
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
        }

        // =====================
        // HSV PARAMS
        // =====================
        public class HsvParam
        {
            public int HMin { get; set; }
            public int HMax { get; set; }

            public int SMin { get; set; }
            public int SMax { get; set; }

            public double ScoreMin { get; set; }
            public double ScoreMax { get; set; }
        }

        // =====================
        // SMD
        // =====================
        public class SMD
        {
            public const int ALG_NONE = 0;
            public const int ALG_HSV = 1;

            public string Name { get; set; }
            public bool IsEnabled { get; set; }
            public int ID { get; set; }

            public ROI ROI { get; set; } = new ROI();

            // Thuật toán đang dùng
            public string Algorithm { get; set; } = "NONE";

            // 🔥 HSV được đóng gói gọn
            public HsvParam HSV { get; set; } = new HsvParam();
        }

        // =====================
        // POV
        // =====================
        public class POV
        {
            public string Name { get; set; }
            public double ExposureTime { get; set; }
            public bool IsEnabled { get; set; }

            public string ImagePath { get; set; } = string.Empty;
            public List<SMD> SMDs { get; set; } = new List<SMD>();

            // SN of selected camera for this POV (can be empty)
            public string CameraSN { get; set; } = string.Empty;
        }

        // =====================
        // MODEL
        // =====================
        public class Model
        {
            public string Name { get; set; }
            public List<POV> POVs { get; set; } = new List<POV>();

            public LogServerConfig LogServer { get; set; } = new LogServerConfig();
        }
        public class CameraItem
        {
            public string SN { get; set; }
            public string Name { get; set; }  // FriendlyName hoặc ProductName
            public override string ToString() => $"{Name} ({SN})";
        }
    }
}
