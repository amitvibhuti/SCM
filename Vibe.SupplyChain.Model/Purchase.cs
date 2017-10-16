using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Vibe.SupplyChain.Model
{
    [DataContract]
    public class Purchase : Entity
    {
        [DataMember]
        [EntityObjectAttribute(Mandate = Mandate.Required, Accessibility = Accessibility.Edit)]
        public DateTime Date { get; set; }
        [DataMember]
        [EntityObjectAttribute(SelectOptions = "Root.Parts", SelectText = "Name", DisplayLabel = "Part")]
        public int PartId { get; set; }
        [DataMember]
        [EntityObjectAttribute(Mandate = Mandate.Required, Accessibility = Accessibility.Edit)]
        public double Quantity { get; set; }
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Price", Mandate = Mandate.Required, Accessibility = Accessibility.Edit)]
        public double CostPrice { get; set; }

        public Purchase(Entity parent) : base(parent)
        {
            Date = DateTime.Now;
        }
        public override string ToString()
        {
            return base.ToString() + " " + PartId + " [" +Date.ToShortDateString() + "] (" + Quantity + " @ Rate=" + Root.Currency + " " + CostPrice + ")";
        }
    }
}
