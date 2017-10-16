using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Vibe.SupplyChain.Data;

namespace Vibe.SupplyChain.Win
{
    public partial class FrmTreeView : Form, ISupplyChainForm
    {
        DataManager _manager;
        ListBoxWriter _eventwriter;
        public FrmTreeView(DataManager manager)
        {
            InitializeComponent();
            _eventwriter = new ListBoxWriter(lstEvent);
            _manager = manager == null ? new DataManager() : manager;
            Console.SetOut(_eventwriter);
            Reload();
            Console.WriteLine("Tree view initialized");
        }
        public void Reload()
        {
            treeViewRoot.Nodes.Clear();
            treeViewRoot.ImageList = imageList1;
            RebindInThread();
        }
        private TreeNode AddNode(TreeNode node, string key, string text)
        {
            if (treeViewRoot.InvokeRequired)
                return (TreeNode)treeViewRoot.Invoke(new Func<TreeNode, string, string, TreeNode>(AddNode), node, key, text);
            else
            {
                if (node == null)
                {
                    TreeNode tn = treeViewRoot.Nodes.Add(key, text);
                    tn.Expand();
                    return tn;
                }
                else
                {
                    return node.Nodes.Add(key, text);
                }
            }
        }
        private void AddImageIndex(TreeNode node, int imgId)
        {
            if (treeViewRoot.InvokeRequired)
                treeViewRoot.Invoke(new Action<TreeNode, int>(AddImageIndex), node, imgId);
            else
            {
                if (node == null)
                    treeViewRoot.ImageIndex = imgId;
                else
                    node.ImageIndex = node.SelectedImageIndex = imgId;
            }
        }
        private void UpdateNodeText(TreeNode node, string text)
        {
            if (treeViewRoot.InvokeRequired)
                treeViewRoot.Invoke(new Action<TreeNode, string>(UpdateNodeText), node, text);
            else
            {
                node.Text = text;
                if (node.Parent == null)
                    node.Expand();
            }
        }
        public void RebindInThread()
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += (sender, e) => { RebindAllNodes(); };
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            treeViewRoot.Nodes[0].Expand();
        }

        public void RebindAllNodes()
        {
            TreeNode tn = null;
            if (!treeViewRoot.Nodes.ContainsKey(_manager.Data.Root.ID.ToString()))
            {
                tn = AddNode(null, _manager.Data.Root.ID.ToString(), _manager.Data.Root.ToString());
            }
            else
            {
                tn = treeViewRoot.Nodes.Find(_manager.Data.Root.ID.ToString(), false)[0];
                UpdateNodeText(tn, _manager.Data.Root.ToString());
            }
            tn.Tag = new EntityParam<Entity>() { Node = tn, Entity = _manager.Data.Root };
            bool retVal = true;
            foreach (EntityList elist in _manager.Data.Root.List)
            {
                retVal = retVal & BindNodes(tn, elist, 1);
            }
            AddImageIndex(tn, !retVal ? 1 : 2);
        }
        public bool BindNodes(TreeNode node, EntityList elist, int index)
        {
            if (elist.EntityObjectAttribute != null 
                && elist.EntityObjectAttribute.Accessibility == Accessibility.NoView)
                return false;
            TreeNode tc = null;
            if (!node.Nodes.ContainsKey(elist.Name))
            {
                tc = AddNode(node,elist.Name, elist.Name);
            }
            else
            {
                tc = node.Nodes.Find(elist.Name, false)[0];
                UpdateNodeText(tc, elist.Name);
            }
            tc.Tag = new EntityParam<EntityList> { Node = tc, EntityList = elist, Entity = elist.Entity };
            bool retValue = true;
            if (elist.CanEdit)
            {
                foreach (Entity e in elist)
                {
                    retValue = retValue & BindNode(tc, e, elist, index);
                }
            }
            AddImageIndex(tc, !retValue ? 1 : 2);
            return retValue;
        }
        public bool BindNode(TreeNode node, Entity e, EntityList parentList, int index)
        {
            TreeNode tn = null;
            if (!node.Nodes.ContainsKey(e.ID.ToString()))
            {
                tn = AddNode(node, e.ID.ToString(), e.ToString());
            }
            else
            {
                tn = node.Nodes.Find(e.ID.ToString(), false)[0];
                UpdateNodeText(tn, e.ToString());
            }
            tn.Tag = new EntityParam<Entity>() { Node = tn, Entity = e, EntityList = parentList };
            bool retVal = !e.HasPropertyWithConstraintViolation;
            foreach (EntityList el in e.List)
            {
                retVal = retVal & BindNodes(tn, el, ++index);
            }
            AddImageIndex(tn, !retVal ? 1 : 2);
            return retVal;
        }

        private void treeViewRoot_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            contextMenuStrip1.Items.Clear();
            if (e.Button == MouseButtons.Right)
            {
                if (e.Node.Tag is EntityParam<Entity>)
                {
                    EntityParam<Entity> entParam = e.Node.Tag as EntityParam<Entity>;
                    foreach(TreeNode tn in e.Node.Nodes)
                    {
                        if(tn.Tag is EntityParam<EntityList>)
                        {
                            EntityParam<EntityList> listParam = tn.Tag as EntityParam<EntityList>;
                            if (listParam.EntityList.CanEdit)
                                AddToContextMenu(listParam.Node, listParam.EntityList, entParam.Entity);
                        }                        
                    }
                    contextMenuStrip1.Items.Add("Delete", Resource.delete, (s, e1) => {
                        Console.WriteLine(entParam.Entity.EntityType + ":" + entParam.Entity.ID + " deleted.");
                        _manager.Data.DeleteEntity(entParam.EntityList, entParam.Entity);
                        TreeNode pNode = e.Node.Parent;
                        e.Node.Remove();
                        RebindInThread();
                        _manager.Propagate();
                    });
                }
                else if (e.Node.Tag is EntityParam<EntityList>)
                {
                    EntityParam<EntityList> eParam = e.Node.Tag as EntityParam<EntityList>;
                    EntityList elist = eParam.EntityList;
                    if (elist.CanEdit)
                        AddToContextMenu(e.Node, elist, elist.Entity);
                }
                contextMenuStrip1.Show(treeViewRoot, e.X, e.Y);
            }
        }
        void AddToContextMenu(TreeNode node, EntityList elist, Entity ent)
        {
            contextMenuStrip1.Items.Add("Add " + elist.ItemType.Name, Resource.add, (s, e1) => {
                Entity enew = elist.CreateItem(ent);
                EntityParam<Entity> neweParam = new EntityParam<Entity>() { EntityList = elist, Entity = enew, Node = node.Nodes.Add(enew.ID.ToString(), enew.ToString()) };
                EditEntity(neweParam);
                RebindInThread();
            });
        }
        private void treeViewRoot_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is EntityParam<Entity>)
            {
                EntityParam<Entity> entParam = e.Node.Tag as EntityParam<Entity>;
                EditEntity(entParam);
            }
            else if (e.Node.Tag is EntityParam<EntityList>)
            {
                EntityParam<EntityList> entParam = e.Node.Tag as EntityParam<EntityList>;
                ShowEntityList(entParam);
            }
        }
        public void ShowEntityList(EntityParam<EntityList> eParam)
        {
            EntityList elist = eParam.EntityList;
            Console.WriteLine("Showing " + elist.Entity.Hierarchy + " >> " + elist.Name);
            splitContainer2.Panel2Collapsed = true;
            splitContainer3.Panel2Collapsed = true;
            pnlChart.Controls.Clear();
            pnlEntity.Tag = elist;
            lblNavigation.Text = elist.Entity.Hierarchy;
            pnlEntity.Controls.Clear();
            btnExportToPDF.Visible = true;
            DataGridView dgv = new DataGridView();
            dgv.BindData(elist.ToDataTable());
            pnlEntity.Controls.Add(dgv);

            if (eParam.EntityList.EntityObjectAttribute == null || String.IsNullOrEmpty(eParam.EntityList.EntityObjectAttribute.ShowChart))
                return;
            splitContainer3.Panel2Collapsed = false;
            pnlChart.Controls.Add(
                new EntityListGraph(_manager, eParam.EntityList)
                {
                    Dock = DockStyle.Fill
                });
        }
        public void EditEntity(EntityParam<Entity> eParam)
        {
            splitContainer2.Panel2Collapsed = false;
            splitContainer3.Panel2Collapsed = true;
            pnlChart.Controls.Clear();
            Entity ent = eParam.Entity;
            Console.WriteLine("Showing " + ent.Hierarchy);
            lblNavigation.Text = ent.Hierarchy;
            pnlEntity.Controls.Clear();
            pnlEntityAuto.Controls.Clear();
            pnlEntity.Tag = ent;
            btnExportToPDF.Visible = false;
            
            TableLayoutPanel tabEdit = new TableLayoutPanel();
            tabEdit.Name = "Edit";
            tabEdit.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tabEdit.Dock = DockStyle.Fill;
            tabEdit.ColumnCount = 2;
            tabEdit.RowCount = Convert.ToInt32(ent.Properties.Count / 2) + 2;
            pnlEntity.Controls.Add(tabEdit);

            TableLayoutPanel tabAuto = new TableLayoutPanel();
            tabAuto.Name = "Auto";
            tabAuto.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tabAuto.Dock = DockStyle.Fill;
            tabAuto.ColumnCount = 2;
            tabAuto.RowCount = Convert.ToInt32(ent.Properties.Count / 2) + 2;
            pnlEntityAuto.Controls.Add(tabAuto);

            int iEdit = 0;
            int iAuto = 0;
            foreach (EntityProperty attr in ent.Properties)
            {
                if (attr.IsViewable)
                {
                    TableLayoutPanel tab = tabAuto;
                    int i = 0;
                    if (attr.IsSelect || attr.IsEditable)
                    {
                        tab = tabEdit;
                        i = iEdit++;
                    }
                    else
                    {
                        i = iAuto++;
                    }

                    Label lbl = new Label() { Text = attr.DisplayText + (attr.IsRequired ? "*" : "") };
                    tab.Controls.Add(lbl, 0, i);

                    PropertyInfo pInfo = ent.GetType().GetProperty(attr.Name);
                    EntityObjectAttribute eprop = Entity.GetEntityAttribute(pInfo);
                    string prefix = "";
                    if (eprop != null && !String.IsNullOrEmpty(eprop.DisplayPrefix))
                    {
                        prefix = ent.ParseEntityNavigation(eprop.DisplayPrefix).Value.ToString();
                        lbl.Text += " (" + prefix + ")";
                    }

                    if (attr.IsSelect)
                    {
                        ComboBox cmb = new ComboBox() { Name = "cmb" + attr.Name, DataSource = attr.SelectOptions, SelectedValue = attr.Value, ValueMember = "ID", DisplayMember = "Name" };
                        tab.Controls.Add(cmb, 1, i);
                        if ((int)attr.Value > 0)
                        {
                            NamedEntity nent = attr.SelectOptions.FirstOrDefault(o => o.ID == (int)attr.Value);
                            string name = nent == null ? "" : attr.SelectOptions.FirstOrDefault(o => o.ID == (int)attr.Value).Name;
                            cmb.SelectedIndex = cmb.FindString(name);
                        }
                    }
                    else if (attr.IsEditable)
                    {
                        TextBox txt = new TextBox() { Name = "txt" + attr.Name, Text = attr.Value == null ? "" : attr.Value.ToString() };
                        if (eprop.Accessibility == Accessibility.EditLarge)
                        {
                            txt.Multiline = true;
                            txt.Height = 50;
                        }
                        txt.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                        if(eprop.MaxLength!=0)
                        {
                            txt.MaxLength = eprop.MaxLength;
                        }
                        tab.Controls.Add(txt, 1, i);
                    }
                    else
                        tab.Controls.Add(new Label() { Text = attr.Value == null ? "" : attr.Value.ToString() }, 1, i);
                }
            }
            Button btn = new Button() { Text = "Save" };
            btn.Tag = eParam.Node.Parent;
            btn.Click += (s, e1) =>
            {
                try
                {
                    foreach (EntityProperty attr in ent.Properties)
                    {
                        if (attr.IsSelect)
                        {
                            Control[] ctrls = tabEdit.Controls.Find("cmb" + attr.Name, false);
                            int idValue = (int)((ComboBox)ctrls[0]).SelectedValue;
                            attr.SetSelectionValue(idValue);
                        }
                        
                        else if (attr.IsEditable)
                        {
                            Control[] ctrls = tabEdit.Controls.Find("txt" + attr.Name, false);
                            string txtValue = ((TextBox)ctrls[0]).Text;
                            if (String.IsNullOrEmpty(txtValue.Trim()) && attr.IsRequired)
                            {
                                MessageBox.Show(attr.Name + " is required.");
                                return;
                            }
                            attr.SetValue(txtValue);
                        }
                    }

                    _manager.Data.Save();
                    ent.OnCreate();
                    if (btn.Tag != null)
                    {
                        RebindInThread();
                        ShowEntityList(new EntityParam<EntityList>() { EntityList = eParam.EntityList, Node = eParam.Node.Parent });
                        _manager.Propagate();
                    }
                    Console.WriteLine(ent.EntityType + ":" + ent.ID + " updated.");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            };
            tabEdit.Controls.Add(btn, 1, ++iEdit);
        }        
        private void btnExportToPDF_Click(object sender, EventArgs e)
        {
            EntityList elist = (EntityList)pnlEntity.Tag;
            if (splitContainer3.Panel2Collapsed)
                SCUtility.ExportToPDF(_manager, (DataGridView)pnlEntity.Controls[0], (elist != null ? elist.Name : "_"), elist.DisplayName);
            else
            {
                ((EntityListGraph)pnlChart.Controls[0]).Chart.SaveImage("chartTemp.jpg", ChartImageFormat.Jpeg);
                SCUtility.ExportToPDF(_manager, (DataGridView)pnlEntity.Controls[0], (elist != null ? elist.Name : "_"), elist.DisplayName, System.Drawing.Image.FromFile("chartTemp.jpg"));
            }
        }
    }
    public class EntityParam<T>
    {
        public TreeNode Node { get; set; }
        public Entity Entity { get; set; }
        public EntityList EntityList { get; set; }
    }
}
