using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Vibe.SupplyChain.TransactionModel
{
    public enum InventoryElementType { In, Out }
    [DataContract]
    public class InventoryElement : Entity
    {
        [EntityObjectAttribute(DisplayLabel = "Type",
            Sequence = 3,
            Accessibility = Accessibility.Auto)]
        public InventoryElementType Type {
            get
            {
                return Transaction == null
                    ? InventoryElementType.In
                    : Transaction.PartTransaction.TransactionType == TransactionType.Buy ? InventoryElementType.In : InventoryElementType.Out;
            }
        }
        [EntityObjectAttribute(DisplayLabel = "Date", 
            Sequence = 2,
            Accessibility = Accessibility.Auto)]
        public DateTime Date {
            get
            {
                 return Transaction == null
                    ? ((Inventory)Parent).CreatedOn
                    : Transaction.PartTransaction.Transaction.Date;
            }
        }
        [EntityObjectAttribute(DisplayLabel = "Rate", 
            Sequence = 4,
            Accessibility = Accessibility.Auto,
            DisplayPrefix = "Root.Currency")]
        public double Rate { get
            {
                return Transaction == null
                 ? ((Inventory)Parent).Part.GetTransactionRate(Date).BuyRate
                 : Transaction.PartTransaction.Rate;
            }
        }
        [EntityObjectAttribute(DisplayLabel = "Quantity",
            Sequence = 5,
            Accessibility = Accessibility.Auto,
            FooterMode = FooterMode.Sum)]
        public double Quantity { get
            {
                return (Type == InventoryElementType.In ? 1 : -1) *
                     (Transaction == null? ((Inventory)Parent).InitialQuantity: Transaction.Quantity);
            }
        }
        [EntityObjectAttribute(DisplayLabel = "Inventory Qty",
            Accessibility = Accessibility.NoView)]
        public double PostTransactionInventoryQuantity
        {
            get
            {
                return ((Inventory)Parent).Elements.Where(e => e.Date <= Date).Sum(e => e.Quantity);
            }
        }
        public InventoryPartTransaction Transaction { get; set; }
        public InventoryElement(Entity parent, InventoryPartTransaction trans) : base(parent)
        {
            Transaction = trans;
        }
        public InventoryElement(Entity parent) : base(parent)
        { }
    }
}
