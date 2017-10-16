using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vibe.SupplyChain.Win
{
    public partial class FrmRename : Form
    {
        public string ParameterNAme { get; set; }
        public FrmRename(string name)
        {
            InitializeComponent();
            txtName.Text = name;
            ParameterNAme = name;
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrEmpty( txtName.Text))
            {
                label1.Text = "Name is required";
                return;
            }
            ParameterNAme = txtName.Text;
            DialogResult = DialogResult.OK;
        }
    }
}
