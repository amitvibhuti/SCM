using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Vibe.SupplyChain.TransactionModel
{
    [DataContract]
    public class Inventory : NamedEntity
    {
        public Inventory(Entity parent) : base(parent)
        {}
        [DataMember]
        [EntityObjectAttribute(SelectOptions = "Root.Parts", 
            Sequence = 3,
            SelectText = "Name", DisplayLabel = "Part")]
        public int PartId { get; set; }
        public Part Part { get { return ((Company)Parent).Parts.Find(p => p.ID == PartId); } }
        [DataMember]
        [EntityObjectAttribute(Mandate = Mandate.Required, 
            Sequence = 5,
            Accessibility = Accessibility.Edit)]
        public double InitialQuantity { get; set; }
        [EntityObjectAttribute(DisplayLabel = "Account", Accessibility = Accessibility.NoView)]
        public List<InventoryPartTransaction> InventoryPartTransactions
        {
            get
            {
                return ((Company)Root).Partners
                    .SelectMany(p => p.Transactions
                        .SelectMany(t => t.PartTransactions
                            .SelectMany(pt => pt.InventoryPartTransactions
                                .Where(ipt => ipt.InventoryId == this.ID)))).ToList();
            }
        }
        [EntityObjectAttribute(Mandate = Mandate.Required, 
            Sequence = 6,
            Accessibility = Accessibility.Auto, 
            CalculatedValue = CalculatedValue.Text, 
            PassCriteria = "value >= 0 AND value <= [Capacity]", 
            SuccessColor = BGColor.Green, 
            FailColor = BGColor.Red)]
        public double Quantity
        {
            get
            {
                double trxqty = InventoryPartTransactions.Sum(ipt => (ipt.PartTransaction.TransactionType == TransactionType.Buy ? 1 : -1) * ipt.Quantity);
                return InitialQuantity + trxqty;
            }
        }
        [DataMember]
        [EntityObjectAttribute(Mandate = Mandate.Required, 
            Sequence = 4,
            Accessibility = Accessibility.Edit)]
        public int Capacity { get; set; }
        [EntityObjectAttribute(DisplayLabel = "Elements", 
            ShowChart = "Area,Matterial flow,Date,PostTransactionInventoryQuantity",
            Accessibility = Accessibility.Auto)]
        public List<InventoryElement> Elements
        {
            get
            {
                List<InventoryElement> acc = new List<InventoryElement>();
                acc.Add(new InventoryElement(this));
                acc.AddRange(InventoryPartTransactions.Select(ipt => new InventoryElement(this, ipt)).ToList());
                return acc;
            }
        }
        public double GetQuantity(DateTime date)
        {
            return this.InitialQuantity + GetBoughtQuantityTillDate(date) - GetSoldQuantityTillDate(date);
        }
        public double GetBoughtQuantityTillDate(DateTime date)
        {
            return this.Elements.Where(e => e.Type== InventoryElementType.In && e.Date < date)
                .Sum(e => e.Quantity);
        }
        public double GetBuyPriceTillDate(DateTime date)
        {
            return this.Elements.Where(e =>
                        e.Type==InventoryElementType.In
                        && e.Date <= date)
                .Sum(e => e.Rate * e.Quantity);
        }
        public double GetCostRate(DateTime date)
        {
            return GetBuyPriceTillDate(date) / GetBoughtQuantityTillDate(date);
        }
        public double GetSoldQuantityTillDate(DateTime date)
        {
            return this.Elements.Where(e => e.Type == InventoryElementType.Out && e.Date < date)
                .Sum(e => e.Quantity);
        }
        public override string ToString()
        {
            return base.ToString() + " " + PartId + "(" + Quantity + "/" + Capacity + ")";
        }
    }
}
