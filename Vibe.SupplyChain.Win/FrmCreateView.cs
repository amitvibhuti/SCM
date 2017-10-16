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
    public partial class FrmCreateView : Form, ISupplyChainForm
    {
        DataManager _manager;
        BindingList<ListBoxItem> _Columns = new System.ComponentModel.BindingList<ListBoxItem>();
        BindingList<ListBoxItem> _Filters = new System.ComponentModel.BindingList<ListBoxItem>();
        public FrmCreateView(DataManager manager)
        {
            InitializeComponent();
            _manager = manager == null ? new DataManager() : manager;
            Reload();
        }
        public FrmCreateView(DataManager manager, TableViewMetadata metadata)
        {
            InitializeComponent();
            _manager = manager == null ? new DataManager() : manager;
            Reload();
            txtViewName.Text = metadata.ViewName;
            cmbDatNode.SelectedValue = metadata.DataNode;
            foreach(TableViewColumn tvc in metadata.Columns)
            {
                TreeNode tn = tvColumns.GetAllNodes().Find(n => n.Tag.ToString() == tvc.Value);
                _Columns.Add(new ListBoxItem(tn.Text, tn.Tag.ToString()));
                lbColumns.DataSource = _Columns;
            }
            foreach (EntityFilter tvc in metadata.Filters)
            {
                TreeNode tn = tvFilters.GetAllNodes().Find(n => n.Tag.ToString() == tvc.FilterNode);
                ListBoxItem lbi = new ListBoxItem(tn.Text, tn.Tag.ToString());
                lbi.Tag = tvc;
                _Filters.Add(lbi);
                lbFilters.DataSource = _Filters;
            }
            LoadGrid();
        }
        public void Reload()
        {
            List<EntityKeyType> types = _manager.Data.Root.GetChildTypes();
            cmbDatNode.DataSource = types;
            cmbDatNode.DisplayMember = "Display";
            cmbDatNode.ValueMember = "Key";
        }

        private void cmbDatNode_SelectedIndexChanged(object sender, EventArgs e)
        {
            tvColumns.Nodes.Clear();
            tvFilters.Nodes.Clear();
            lbColumns.Items.Clear();
            lbFilters.Items.Clear();
            EntityKeyType keytype = (EntityKeyType)cmbDatNode.SelectedItem; // Order: 2
            tvColumns.Nodes.AddRange(Entity.GetViewableProperties(keytype.Type).Select(p => new TreeNode(p) { Tag = p }).ToArray());
            tvFilters.Nodes.AddRange(Entity.GetViewableProperties(keytype.Type).Select(p => new TreeNode(p) { Tag = p }).ToArray());
            List<EntityKeyType> pList = Entity.GetParents(_manager.Data.Root, keytype.Type);
            foreach (EntityKeyType p in pList)
            {
                string prefix = String.Concat(Enumerable.Repeat("Parent.", keytype.DepthFromRoot - p.DepthFromRoot));
                TreeNode pNode = new TreeNode(p.Type.Name);
                pNode.Tag = p.Key;
                tvColumns.Nodes.Add(pNode);
                pNode.Nodes.AddRange(Entity.GetViewableProperties(p.Type).Select(p2 => new TreeNode(p2) { Tag = prefix + p2 }).ToArray());

                pNode = new TreeNode(p.Type.Name);
                pNode.Tag = p.Key;
                tvFilters.Nodes.Add(pNode);
                pNode.Nodes.AddRange(Entity.GetViewableProperties(p.Type).Select(p2 => new TreeNode(p2) { Tag = prefix + p2 }).ToArray());
            }
            LoadGrid();
        }

        private void btnColumnsMove_Click(object sender, EventArgs e)
        {
            _Columns.Add(new ListBoxItem(tvColumns.SelectedNode.Text, tvColumns.SelectedNode.Tag.ToString()));
            lbColumns.DataSource = _Columns;
            LoadGrid();
        }

        private void btnFiltersMove_Click(object sender, EventArgs e)
        {
            FrmAddFilter addfilter = new FrmAddFilter(tvFilters.SelectedNode.Tag.ToString());
            if (addfilter.ShowDialog() == DialogResult.OK)
            {
                ListBoxItem lbi = new ListBoxItem(tvFilters.SelectedNode.Text, tvFilters.SelectedNode.Tag.ToString());
                lbi.Tag = addfilter.Filter;
                _Filters.Add(lbi);
                lbFilters.DataSource = _Filters;
                LoadGrid();
            }
        }
        void LoadGrid()
        {
            pnlGrid.Controls.Clear();
            SCDataGridView dgv = new SCDataGridView(_manager, Metadata.Serialize());
            dgv.Dock = DockStyle.Fill;
            pnlGrid.Controls.Add(dgv);
            dgv.Show();
        }
        public TableViewMetadata Metadata
        {
            get
            {
                TableViewMetadata tab = new TableViewMetadata();
                tab.ViewName = txtViewName.Text;
                tab.DataNode = ((EntityKeyType)cmbDatNode.SelectedItem).Key;

                if (lbFilters.Items.Count != 0)
                {
                    foreach (object tn in lbFilters.Items)
                    {
                        ListBoxItem item = (ListBoxItem)tn;
                        tab.Filters.Add( (EntityFilter)item.Tag );
                    }
                }
                if (lbColumns.Items.Count == 0)
                {
                    foreach (TreeNode tn in tvColumns.Nodes)
                    {
                        if (tn.Nodes.Count == 0)
                            tab.Columns.Add(new TableViewColumn() { HeaderText = tn.Text, Value = tn.Tag.ToString() });
                    }
                }
                else
                {
                    foreach (object tn in lbColumns.Items)
                    {
                        ListBoxItem item = (ListBoxItem)tn;
                        tab.Columns.Add(new TableViewColumn() { HeaderText = item.Title, Value = item.Path });
                    }
                }
                return tab;
            }
        }
        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadGrid();
        }

        private void toolStripRename_Click(object sender, EventArgs e)
        {
            ListBoxItem item = (ListBoxItem)lbColumns.SelectedItem;
            FrmRename frm = new FrmRename(item.Title);
            if(frm.ShowDialog()==DialogResult.OK)
            {
                item.Title = frm.ParameterNAme;
                LoadGrid();
            }
        }

        private void toolStripDelete_Click(object sender, EventArgs e)
        {
            lbColumns.Items.Remove(lbColumns.SelectedItem);
        }

        private void lbColumns_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            var index = lbColumns.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                lbColumns.SelectedIndex = index;
                contextMenuStrip1.Show(Cursor.Position);
                contextMenuStrip1.Visible = true;
            }
            else
            {
                contextMenuStrip1.Visible = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtViewName.Text))
                MessageBox.Show("View name is required");
            else
            {
                TableViewCollection tvc = new TableViewCollection(ConfigurationManager.AppSettings["ViewSource"]);
                tvc.Add(Metadata);
                MessageBox.Show("View saved");
            }
        }
    }
    public class ListBoxItem : INotifyPropertyChanged
    {
        public string _title;
        public string _path;
        public string Title { get { return _title; } set { _title = value; OnDisplayPropertyChanged(); } }
        public string Path { get { return _path; } set { _path = value; OnDisplayPropertyChanged(); } }
        public object Tag { get; set; }
        public ListBoxItem(string title, string path)
        {
            Title = title;
            Path = path;
        }
        public override string ToString()
        {
            return Title + " [" + Path + "]";
        }
        void OnDisplayPropertyChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("DisplayName"));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
