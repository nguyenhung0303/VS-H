using Newtonsoft.Json;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static vs_h.model;
namespace vs_h
{
    public partial class LogServer : UIForm
    {
      
        public LogServer(LogServerConfig cfg = null)
        {
            InitializeComponent();
            LoadAnyModelToTextboxes();

        }
        private void LogServer_Load(object sender, EventArgs e)
        {
            LoadAnyModelToTextboxes();
        }

        private void LoadAnyModelToTextboxes()
        {
            string folderPath = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\Models"));
            if (!Directory.Exists(folderPath)) return;

            string first = Directory.GetFiles(folderPath, "*.json").FirstOrDefault();
            if (first == null) return;

            Model model = JsonConvert.DeserializeObject<Model>(File.ReadAllText(first));
            if (model == null) model = new Model();
            if (model.LogServer == null) model.LogServer = new LogServerConfig();

            txtHostName.Text = model.LogServer.HostName ?? "";
            txtUserServer.Text = model.LogServer.UserName ?? "";
            txtPassServer.Text = model.LogServer.PassWork ?? "";
            txtPort.Text = model.LogServer.PortNumber ?? "";
        }




        private void btnSaveServer_Click(object sender, EventArgs e)
        {
            string folderPath = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\Models"));
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("Không tìm thấy thư mục Models.");
                return;
            }

            string host = (txtHostName.Text ?? "").Trim();
            string user = (txtUserServer.Text ?? "").Trim();
            string pass = txtPassServer.Text ?? "";

            string[] files = Directory.GetFiles(folderPath, "*.json");
            if (files.Length == 0)
            {
                MessageBox.Show("Không có file model .json trong Models.");
                return;
            }

            int ok = 0, fail = 0;

            foreach (string file in files)
            {
                try
                {
                    Model model = JsonConvert.DeserializeObject<Model>(File.ReadAllText(file));
                    if (model == null) { fail++; continue; }

                    if (model.LogServer == null) model.LogServer = new LogServerConfig();

                    model.LogServer.HostName = host;
                    model.LogServer.UserName = user;
                    model.LogServer.PassWork = pass;

                    model.LogServer.PortNumber = "4422";  // ✅ luôn 4422
                    model.LogServer.Protocol = "SFTP";  // ✅ luôn SFTP

                    File.WriteAllText(file, JsonConvert.SerializeObject(model, Formatting.Indented));
                    ok++;
                }
                catch
                {
                    fail++;
                }
            }

            MessageBox.Show("Đã lưu LogServer cho tất cả model.\nOK: " + ok + "\nFAIL: " + fail);
        }



    }
}
