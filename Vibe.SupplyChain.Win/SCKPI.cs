using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vibe.SupplyChain.Win
{
    public enum KPIViewType{ DigiMeter }
    public class SCKPI
    {
        public KPIViewType ViewType { get; set; }
        public string Name { get; set; }
        public string ValueText { get; set; }
        public string UnitText { get; set; }
    }
}
