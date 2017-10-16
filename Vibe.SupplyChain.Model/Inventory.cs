using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Vibe.SupplyChain.Model
{
    [DataContract]
    public class Inventory : NamedEntity
    {
        public Inventory(Entity parent) : base(parent)
        {}
        [DataMember]
        [EntityObjectAttribute(SelectOptions = "Root.Parts", SelectText = "Name", DisplayLabel = "Part")]
        public int PartId { get; set; }
        [DataMember]
        [EntityObjectAttribute(Mandate = Mandate.Required, Accessibility = Accessibility.Edit)]
        public double InitialQuantity { get; set; }
        [EntityObjectAttribute(Mandate = Mandate.Required, 
            Accessibility = Accessibility.Auto, 
            CalculatedValue = CalculatedValue.Text, 
            PassCriteria = "value >= 0 AND value <= [Capacity]", 
            SuccessColor = BGColor.Green, 
            FailColor = BGColor.Red)]
        public double Quantity
        {
            get
            {
                double soldqty = ((Company)this.Parent).Customers.Sum(c => c.Orders.Sum(o => o.PartOrders.Where(po => po.PartId == PartId).Sum(po => po.Quantity)));
                double purchaseqty = ((Company)this.Parent).Purchases.Where(p => p.PartId == PartId).Sum(p => p.Quantity);
                return InitialQuantity + purchaseqty - soldqty;
            }
        }
        [DataMember]
        [EntityObjectAttribute(Mandate = Mandate.Required, Accessibility = Accessibility.Edit)]
        public int Capacity { get; set; }
        public override string ToString()
        {
            return base.ToString() + " " + PartId + "(" + Quantity + "/" + Capacity + ")";
        }
    }
}
