using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vibe.SupplyChain.Win
{
    public partial class DigitalMeter : UserControl, IKPIForm
    {
        public DigitalMeter(SCKPI kpi)
        {
            InitializeComponent();
            lblHeader.Text = kpi.Name;
            lblValue.Text = kpi.ValueText;
            lblUnit.Text = kpi.UnitText;
        }
    }
}
