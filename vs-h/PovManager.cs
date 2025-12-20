// File: PovManager.cs
using System;
using System.Globalization;
using System.Windows.Forms;
using static vs_h.model;

namespace vs_h
{
    public class PovManager
    {
        private readonly TextBox _txtName;
        private readonly TextBox _txtExposureTime;
        private readonly CheckBox _checkBoxIsEnabled;
        private readonly TreeView _treeView; // Cần thiết cho Add POV

        public PovManager(TextBox nameControl, TextBox exposureControl, CheckBox enabledControl, TreeView treeView)
        {
            _txtName = nameControl;
            _txtExposureTime = exposureControl;
            _checkBoxIsEnabled = enabledControl;
            _treeView = treeView;

            // (Tuỳ chọn) chỉ cho nhập số + dấu . , -
            _txtExposureTime.KeyPress += (s, e) =>
            {
                if (char.IsControl(e.KeyChar)) return;
                if (char.IsDigit(e.KeyChar)) return;
                if (e.KeyChar == '.' || e.KeyChar == ',' || e.KeyChar == '-') return;
                e.Handled = true;
            };
        }

        // 🌟 LOGIC THÊM POV
        public void AddPovToModel(TreeNode modelNode)
        {
            int povCount = modelNode.Nodes.Count;
            string povName = "POV" + (povCount + 1);

            POV newPov = new POV
            {
                Name = povName,
                ExposureTime = 1.0,
                IsEnabled = true
            };

            TreeNode povNode = new TreeNode(newPov.Name) { Tag = newPov };
            modelNode.Nodes.Add(povNode);
            modelNode.Expand();

            _treeView.SelectedNode = povNode;
            MessageBox.Show($"Đã thêm {povName} cho model '{modelNode.Text}'");
        }

        public void ShowPovControls(POV povData)
        {
            _txtName.Text = povData?.Name ?? "";

            // Hiển thị exposure ổn định, không phụ thuộc dấu phẩy/dấu chấm của Windows
            _txtExposureTime.Text = (povData?.ExposureTime ?? 0).ToString(CultureInfo.InvariantCulture);

            _checkBoxIsEnabled.Checked = povData?.IsEnabled ?? false;

            _txtName.Show();
            _txtExposureTime.Show();
            _checkBoxIsEnabled.Show();
        }

        public void HidePovControls()
        {
            _txtName.Hide();
            _txtExposureTime.Hide();
            _checkBoxIsEnabled.Hide();
        }

        public void SavePovData(POV pov)
        {
            if (pov == null) return;

            pov.Name = (_txtName.Text ?? "").Trim();
            pov.IsEnabled = _checkBoxIsEnabled.Checked;

            string raw = (_txtExposureTime.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(raw))
            {
                MessageBox.Show("ExposureTime không được để trống.");
                return;
            }

            // Parse theo cả CurrentCulture và InvariantCulture để ăn cả '.' và ','
            if (double.TryParse(raw, NumberStyles.Float, CultureInfo.CurrentCulture, out double exposure) ||
                double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out exposure))
            {
                if (exposure < 0)
                {
                    MessageBox.Show("ExposureTime phải >= 0");
                    return;
                }

                pov.ExposureTime = exposure;
            }
            else
            {
                MessageBox.Show($"ExposureTime không hợp lệ: '{raw}'\nVí dụ: 10000 hoặc 10000.5");
            }
        }
    }
}
