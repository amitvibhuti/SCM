using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vibe.SupplyChain.Data;

namespace Vibe.SupplyChain.Win
{
    public partial class FrmTableView : Form, ISupplyChainForm
    {
        DataManager _manager;
        public FrmTableView(DataManager manager, TableViewMetadata metadata)
        {
            InitializeComponent();
            _manager = manager == null ? new DataManager() : manager;
            SCDataGridView dgv = new SCDataGridView(_manager, metadata);
            dgv.Dock = DockStyle.Fill;
            this.Controls.Add(dgv);
        }
        public FrmTableView(DataManager manager, string metadata)
        {
            InitializeComponent();
            SCDataGridView dgv = new SCDataGridView(_manager, metadata);
            dgv.Dock = DockStyle.Fill;
            this.Controls.Add(dgv);            
        }
        public void Reload()
        {
        }
    }
}
