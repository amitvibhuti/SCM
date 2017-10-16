using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vibe.SupplyChain.Data;
using System.Dynamic;

namespace Vibe.SupplyChain.Win
{
    public partial class SCDataGridView : UserControl
    {
        DataManager _manager;
        IEnumerable<Entity> elist = null;
        public SCDataGridView(DataManager manager, string metadata)
            :this(manager, TableViewMetadata.Deserialize(metadata))
        { }
        public SCDataGridView(DataManager manager, TableViewMetadata tvmetadata)
        {
            InitializeComponent();
            _manager = manager == null ? new DataManager() : manager;

            toolStripLabel.Text = tvmetadata.ViewName;

            string[] nodeparts = tvmetadata.DataNode.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (nodeparts.Length == 0 || nodeparts[0].ToLower() != "root")
                throw new Exception("Datanode must start with Root/");

            IEnumerable<EntityList> list = _manager.Data.Root.List;
            foreach (string nodepart in nodeparts)
            {
                if (nodepart.ToLower() == "root")
                {
                    list = _manager.Data.Root.List;
                }
                else if (elist == null)
                {
                    elist = list.FirstOrDefault(el => el.Name == nodepart);
                }
                else
                {
                    elist = elist.SelectMany(o => o.List
                        .FirstOrDefault(op => op.Name == nodepart));
                }
            }
            if (tvmetadata.Filters != null && tvmetadata.Filters.Count > 0)
                elist = elist.Where(e => tvmetadata.Filters.TrueForAll(flt => e.Validate(flt)));

            if (tvmetadata.Columns == null || tvmetadata.Columns.Count == 0)
            {
                // TODO
            }
            else
            {
                if (elist == null)
                    return;
                var dynatv = tvmetadata.Columns.Select(c =>
                {
                    dynamic x = new ExpandoObject();
                    x.HeaderText = c.HeaderText;
                    x.Value = c.Value;
                    return x;
                });

                var listData = elist.Select(e => {
                    dynamic y = new ExpandoObject();
                    var temp = y as IDictionary<string, object>;
                    foreach (dynamic eo in dynatv)
                    {
                        temp.Add(eo.Value, new SCDataMap() { HeaderText = eo.HeaderText, Entity = e, MapText = eo.Value }); // e.ParseEntityHierarchy(eo.Value)
                    }
                    return y;
                });

                try
                {
                    dgvTableView.BindData(listData.ToDataTable());
                }
                catch(Exception exc)
                {
                    MessageBox.Show(exc.Message + ". It could be possible that your view is not in sync with your model.");
                }
            }
        }
        private void toolStripExport_Click(object sender, EventArgs e)
        {
            string fileName = "Custom";
            string displayName = "Custom Report";
            if (elist != null && elist.Count() > 0)
            {
                fileName = elist.ElementAt(0).EntityType;
                displayName = "List of " + elist.ElementAt(0).EntityType;
            }
            SCUtility.ExportToPDF(_manager, dgvTableView, fileName, displayName);
        }
    }
}
