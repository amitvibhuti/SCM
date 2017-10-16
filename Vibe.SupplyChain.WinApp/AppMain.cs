using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vibe.SupplyChain.Data;
using Vibe.SupplyChain.Win;

namespace Vibe.SupplyChain.WinApp
{
    public partial class AppMain : Form
    {
        DataManager _manager;
        ISupplyChainForm _selectedForm;
        public AppMain()
        {
            InitializeComponent();
            try
            {
                _selectedForm = new FrmTreeView(_manager);
                _manager = new DataManager();
                this.Text = this.Text + " " + Application.ProductVersion;
                LoadForm(_selectedForm);
                LoadKPIForm();
                LoadMyViews();
            }
            catch(Exception exc)
            {
                Console.WriteLine(exc.Message + "\r\n" + exc.InnerException ?? 
                    exc.InnerException.Message + "\r\n" + exc.InnerException.StackTrace);
                MessageBox.Show(exc.Message);
            }
        }
        void LoadMyViews()
        {
            myViewsToolStripMenuItem.DropDownItems.Clear();
            TableViewCollection tvc = new TableViewCollection(ConfigurationManager.AppSettings["ViewSource"]);
            foreach(TableViewMetadata tvm in tvc.MetadataList)
            {
                ToolStripItem tsi = new ToolStripMenuItem(tvm.ViewName);
                tsi.Click+=(sender, e)=>{
                    _selectedForm = new FrmTableView(_manager, tvm);
                    LoadForm(_selectedForm);
                };
                myViewsToolStripMenuItem.DropDownItems.Add(tsi);
            }
        }
        void LoadKPIForm()
        {
            KPIForm form = new KPIForm(_manager, new List<SCKPI>() {
                new SCKPI() {
                    Name ="Net Profit",
                    ValueText ="Root.NetProfit",
                    ViewType = KPIViewType.DigiMeter,
                    UnitText ="Root.Currency" },
                new SCKPI() {
                    Name ="Growth",
                    ValueText ="Root.GrowthPercentage",
                    ViewType = KPIViewType.DigiMeter,
                    UnitText ="%" },
                new SCKPI() {
                    Name ="Annual Growth",
                    ValueText ="Root.AnnualGrowthPercentage",
                    ViewType = KPIViewType.DigiMeter,
                    UnitText ="%" }
            });
            ((Form)form).TopLevel = false;
            ((Form)form).AutoScroll = true;
            ((Form)form).FormBorderStyle = FormBorderStyle.None;
            ((Form)form).Dock = DockStyle.Fill;
            pnlKPI.Controls.Clear();
            pnlKPI.Controls.Add((Form)form);
            ((Form)form).Show();
        }
        void LoadForm(ISupplyChainForm form)
        {
            ((Form)_selectedForm).TopLevel = false;
            ((Form)_selectedForm).AutoScroll = true;
            ((Form)_selectedForm).FormBorderStyle = FormBorderStyle.None;
            ((Form)_selectedForm).Dock = DockStyle.Fill;
            pnlBody.Controls.Clear();
            pnlBody.Controls.Add((Form)_selectedForm);
            ((Form)_selectedForm).Show();
        }
        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_manager != null)
                _manager.Data.Reset();
            else
                _manager = DataManager.Reset();
            _selectedForm.Reload();
        }

        private void showJSONDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.IO.File.WriteAllText("temp.txt", _manager.Data.Root.Serialize());
            System.Diagnostics.Process.Start("temp.txt");
        }

        private void treeViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _selectedForm = new FrmTreeView(_manager);
            LoadForm(_selectedForm);
        }

        private void uploadJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                string jsonString = File.ReadAllText(openFileDialog1.FileName);
                _manager.Import(jsonString);
                _selectedForm.Reload();
            }
        }
        private void ordersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string TableMetadata = "{\"DataNode\":\"Root.Customers.Orders\"," +
                                "\"ViewName\":\"Open orders\"," +
                                "\"Columns\":[" +
                                    "{\"HeaderText\":\"ID\", \"Value\":\"DisplayID\"}, " +
                                    "{\"HeaderText\":\"Customer\", \"Value\":\"Parent.Name\"}, " +
                                    "{\"HeaderText\":\"Date\", \"Value\":\"Date\"}, " +
                                    "{\"HeaderText\":\"Price\", \"Value\":\"Price\"}, " +
                                    "{\"HeaderText\":\"Payment\", \"Value\":\"PaymentRecieved\"}, " +
                                    "{\"HeaderText\":\"Status\", \"Value\":\"PaymentCleared\"} " +
                                "]," +
                                "\"Filters\":[" +
                                    "{\"FilterNode\":\"PaymentCleared\", \"FilterOperand\":0, \"FilterValue\":\"false\"} " +
                                "]" +
                             "}";
            _selectedForm = new FrmTableView(_manager, TableMetadata);
            LoadForm(_selectedForm);
        }

        private void inventoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string TableMetadata = "{\"DataNode\":\"Root.Inventories\"," +
                                "\"ViewName\":\"Inventory View\"," + 
                                "\"Columns\":[" +
                                    "{\"HeaderText\":\"ID\", \"Value\":\"DisplayID\"}, " +
                                    "{\"HeaderText\":\"Part\", \"Value\":\"PartId\"} " +
                                "]" +                                
                             "}";
            _selectedForm = new FrmTableView(_manager, TableMetadata);
            LoadForm(_selectedForm);
        }

        private void createViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _selectedForm = new FrmCreateView(_manager);
            LoadForm(_selectedForm);
        }

        private void manageViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _selectedForm = new FrmViews(_manager);
            LoadForm(_selectedForm);
        }

        private void learnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not available with this version.");
        }

        private void contactUsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("You are currently using an evaluation software. Please write to us to get a licensed sofware. \r\n vibhuti.amit@gmail.com");
        }
    }
}
