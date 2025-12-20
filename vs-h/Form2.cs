using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vs_h
{
    public partial class NewModelForm : Form
    {
        public NewModelForm()
        {
            InitializeComponent();
        }
        public string ModelName { get; private set; }

  

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtModelName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên model !");
                return;
            }

            ModelName = txtModelName.Text.Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
       
    }
}
