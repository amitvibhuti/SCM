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
    public partial class KPIForm : Form, ISupplyChainForm
    {
        DataManager _manager;
        List<SCKPI> kpiList;
        public KPIForm(DataManager manager, List<SCKPI> kpis)
        {
            InitializeComponent();
            _manager = manager == null ? new DataManager() : manager;
            kpiList = kpis;
            Reload();
        }
        public void Reload()
        {
            tblKPIs.Controls.Clear();
            tblKPIs.ColumnCount = kpiList.Count;
            int i = 0;
            foreach(SCKPI kpi in kpiList)
            {
                tblKPIs.Controls.Add((UserControl)LoadForm(kpi), i, 0);
                i++;
            }
        }
        IKPIForm LoadForm(SCKPI kpi)
        {
            IKPIForm form = null;
            kpi.ValueText = _manager.Data.Root.ParseEntityNavigation(kpi.ValueText).Value.ToString();
            kpi.UnitText = kpi.UnitText.ToLower().StartsWith("root.")
                ? _manager.Data.Root.ParseText(kpi.UnitText).ToString()
                : kpi.UnitText;
            switch (kpi.ViewType)
            {
                case KPIViewType.DigiMeter:
                    form = new DigitalMeter(kpi);
                        break;
            }

            ((UserControl)form).AutoScroll = false;
            ((UserControl)form).Padding = new Padding(2);
            //((UserControl)form).Dock = DockStyle.Fill;
            ((UserControl)form).Show();
            return form;
        }
    }
}
