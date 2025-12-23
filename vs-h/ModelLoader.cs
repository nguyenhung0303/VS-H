// File: ModelLoader.cs
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using System;
using static vs_h.model;
using Sunny.UI;

namespace vs_h
{
    public class ModelLoader
    {
        private UITreeView _treeView;
        private string _modelsFolderPath;

        public ModelLoader(UITreeView treeView)
        {
            _treeView = treeView;
            string projectRoot = Path.Combine(Application.StartupPath, @"..\..");
            _modelsFolderPath = Path.GetFullPath(Path.Combine(projectRoot, "Models"));
        }

        public void LoadSpecificModelToTreeView(string filePath)
        {
            try
            {
                string jsonString = File.ReadAllText(filePath);
                Model model = Newtonsoft.Json.JsonConvert.DeserializeObject<Model>(jsonString);

                if (model != null)
                {
                    TreeNode modelNode = new TreeNode(model.Name) { Name = model.Name, Tag = model };

                    if (model.POVs != null)
                    {
                        foreach (POV pov in model.POVs)
                        {
                            TreeNode povNode = new TreeNode(pov.Name) { Tag = pov };

                            if (pov.SMDs != null)
                            {
                                foreach (SMD smd in pov.SMDs)
                                {
                                    TreeNode smdNode = new TreeNode(smd.Name) { Tag = smd };
                                    povNode.Nodes.Add(smdNode);
                                }
                            }
                            modelNode.Nodes.Add(povNode);
                        }
                    }
                    _treeView.Nodes.Add(modelNode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc file '{Path.GetFileName(filePath)}': {ex.Message}", "Lỗi Tải Model", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadAllModelsToTreeView()
        {
            _treeView.Nodes.Clear();
            if (!Directory.Exists(_modelsFolderPath)) return;
            string[] modelFiles = Directory.GetFiles(_modelsFolderPath, "*.json");
            foreach (string filePath in modelFiles)
            {
                LoadSpecificModelToTreeView(filePath);
            }
        }
    }
}