// File: SmdManager.cs
using System;
using System.Windows.Forms;
using static vs_h.model;
using Sunny.UI;

namespace vs_h
{
    public class SmdManager
    {
        private readonly UITreeView _treeView;

        public SmdManager(UITreeView treeView)
        {
            _treeView = treeView;
        }

        // 🌟 LOGIC THÊM SMD
        public void AddSMDToPov(POV targetPov, TreeNode povNode)
        {
            int newId = targetPov.SMDs.Count;
            string smdName = $"SMD_{newId:D3}";

            SMD newSmd = new SMD
            {
                Name = smdName,
                ID = newId,
                IsEnabled = true,
                Algorithm = "",
                ROI = new model.ROI { X = 0.0, Y = 0.0, Width = 0.0, Height = 0.0 }
            };

            targetPov.SMDs.Add(newSmd);

            TreeNode smdNode = new TreeNode(newSmd.Name) { Tag = newSmd };
            povNode.Nodes.Add(smdNode);
            povNode.Expand();

            _treeView.SelectedNode = smdNode;
            MessageBox.Show($"Đã thêm SMD mới: {smdName} vào POV: {targetPov.Name}.");
        }
    }
}