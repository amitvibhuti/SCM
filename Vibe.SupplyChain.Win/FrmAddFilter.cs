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
    public partial class FrmAddFilter : Form
    {
        public EntityFilter Filter { get; set; }
        public FrmAddFilter(string property)
        {
            InitializeComponent();
            cmbOperand.DataSource = EnumerationEntity.Parse(typeof(Operand)); //Enum.GetValues(typeof(Operand));
            cmbOperand.ValueMember = "ID";
            cmbOperand.DisplayMember = "Name";
            txtProperty.Text = property;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            Filter = new EntityFilter() { FilterNode = txtProperty.Text, FilterOperand = (Operand)cmbOperand.SelectedValue, FilterValue = txtValue.Text };
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
