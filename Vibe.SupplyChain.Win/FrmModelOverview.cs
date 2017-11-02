using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vibe.SupplyChain.Data;

namespace Vibe.SupplyChain.Win
{
    public partial class FrmModelOverview : Form, ISupplyChainForm
    {
        DataManager _manager;
        public FrmModelOverview(DataManager manager)
        {
            InitializeComponent();
            _manager = manager == null ? new DataManager() : manager;
            TreeNode rootNode = tvModel.Nodes.Add(_manager.Data.Root.GetType().Name);
            rootNode.Tag = _manager.Data.Root.GetType();
            Add(rootNode, _manager.Data.Root.GetType());
            tvModel.ExpandAll();
            tvModel.SelectedNode = rootNode;
        }
        public void Reload()
        {
        }
        void Add(TreeNode tn, Type eType)
        {
            Console.WriteLine("@" + eType.Name);
            List<Type> types = Entity.GetEntityListTypes(eType);
            foreach(Type t in types)
            {
                TreeNode node = tn.Nodes.Add(t.Name);
                node.Tag = t;
                Add(node, t);
            }
        }

        private void tvModel_AfterSelect(object sender, TreeViewEventArgs e)
        {
            tvModelEntity.Nodes.Clear();
            Type t = (Type)e.Node.Tag;
            TreeNode rootNode = tvModelEntity.Nodes.Add("Parent: " + (e.Node.Parent == null?"--": e.Node.Parent.Text));
            tvModelEntity.Margin = new Padding(20);
            tvModelEntity.ItemHeight = 20;
            rootNode.BackColor = Color.Black;
            rootNode.ForeColor = Color.White;
            TreeNode curNode = rootNode.Nodes.Add(e.Node.Text);
            curNode.BackColor = Color.LightSteelBlue;
            curNode.ForeColor = Color.Black;
            curNode.Nodes.AddRange(Entity.GetProperties(t).Select(p => new TreeNode(p) { BackColor = Color.LightSeaGreen }).ToArray());
            tvModelEntity.ExpandAll();
        }
    }
}
