using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vibe.SupplyChain.Data;

namespace Vibe.SupplyChain.Win
{
    public partial class FrmViews : Form, ISupplyChainForm
    {
        DataManager _manager;
        TableViewCollection tvc;
        public FrmViews(DataManager manager)
        {
            InitializeComponent();
            _manager = manager == null ? new DataManager() : manager;
            tvc = new TableViewCollection(ConfigurationManager.AppSettings["ViewSource"]);
            Reload();
        }
        public void Reload()
        {
            lstViews.DataSource = tvc.MetadataList;
            if (tvc.MetadataList.Count > 0)
                lstViews.SelectedIndex = 0;
        }

        private void lstViews_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadGrid();
        }
        void LoadGrid()
        {
            pnlGrid.Controls.Clear();
            TableViewMetadata metadata = (TableViewMetadata)lstViews.SelectedItem;
            SCDataGridView dgv = new SCDataGridView(_manager, metadata.Serialize());
            dgv.Dock = DockStyle.Fill;
            pnlGrid.Controls.Add(dgv);
            dgv.Show();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tvc.Remove(((TableViewMetadata)lstViews.SelectedItem).ViewName);
        }

        private void lstViews_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            var index = lstViews.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                lstViews.SelectedIndex = index;
                contextMenuStrip1.Show(Cursor.Position);
                contextMenuStrip1.Visible = true;
            }
            else
            {
                contextMenuStrip1.Visible = false;
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmCreateView createview = new FrmCreateView(_manager, (TableViewMetadata)lstViews.SelectedItem);
            createview.Show();
        }
    }
}
