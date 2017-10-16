using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Vibe.SupplyChain.TransactionModel
{
    [DataContract]
    public class PartTransaction : Vibe.SupplyChain.Entity
    {
        [DataMember]
        [EntityObjectAttribute(SelectOptions = "Root.Parts", 
            Sequence = 2,
            SelectText = "Name", 
            DisplayLabel = "Part")]
        public int PartId { get; set; }
        public Part Part
        {
            get
            {
                return ((Company)Root).Parts.Find(p => p.ID == PartId);
            }
        }
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Details of Inventory wise parts transaction")]
        public List<InventoryPartTransaction> InventoryPartTransactions { get; set; }
        [EntityObjectAttribute(Mandate = Mandate.Required, 
            Sequence = 5,
            Accessibility = Accessibility.Auto)]
        public double Quantity { get {
                return InventoryPartTransactions.Sum(pt => pt.Quantity);
            }
        }
        [EntityObjectAttribute(DisplayLabel = "Rate", 
            Sequence = 4,
            Accessibility = Accessibility.Auto, DisplayPrefix = "Root.Currency")]
        public double Rate
        {
            get
            {
                if (Part == null)
                    return 0;
                PartRate prate = Part.GetTransactionRate(((Transaction)this.Parent).Date);
                if (prate == null)
                    return 0;
                return TransactionType == TransactionType.Sell ? prate.SellRate : prate.BuyRate;
            }
        }
        [EntityObjectAttribute(Accessibility = Accessibility.Auto, 
            Sequence = 7,
            FooterMode = FooterMode.Sum, DisplayPrefix = "Root.Currency")]
        public double Price {
            get
            {
                return InventoryPartTransactions.Sum(pt => pt.Price);
            }
        }
        [EntityObjectAttribute(DisplayLabel = "GST", 
            Sequence = 8,
            Accessibility = Accessibility.Auto, DisplayPrefix = "Root.Currency", FooterMode = FooterMode.Sum)]
        public double Tax
        {
            get
            {
                return InventoryPartTransactions.Sum(pt => pt.Tax);
            }
        }
        public Transaction Transaction { get { return (Transaction)Parent; } }
        [EntityObjectAttribute(DisplayLabel = "Transaction Type", 
            Sequence = 3,
            Accessibility = Accessibility.Auto, DisplayPrefix = "Root.Currency")]
        public TransactionType TransactionType
        {
            get
            {
                return Transaction.TransactionType;
            }
        }
        [EntityObjectAttribute(DisplayLabel = "Gross", 
            Sequence = 9,
            Accessibility = Accessibility.Auto, DisplayPrefix = "Root.Currency", FooterMode = FooterMode.Sum)]
        public double Gross
        {
            get
            {
                return InventoryPartTransactions.Sum(pt => pt.Gross);
            }
        }
        [EntityObjectAttribute(DisplayLabel = "Inventory Cost", 
            Sequence = 6,
            Accessibility = Accessibility.Auto, DisplayPrefix = "Root.Currency")]
        public double InventoryCost
        {
            get
            {
                return InventoryCostRate * Quantity;
            }
        }
        public double InventoryCostRate { get
            {
                if (Part == null)
                    return 0;
                return Part.GetCostRate(((Transaction)Parent).Date);
            }
        }
        [EntityObjectAttribute(DisplayLabel = "Profit", 
            Sequence = 10,
            Accessibility = Accessibility.Auto)]
        public double NetProfit
        {
            get
            {
                return TransactionType == TransactionType.Buy ? 0 : (Rate - InventoryCostRate) * Quantity;
            }
        }
        public PartTransaction(Entity parent) : base(parent)
        {
            InventoryPartTransactions = new List<InventoryPartTransaction>();
        }
        public override string ToString()
        {
            return base.ToString() + ": " + (Part == null ? "" : Part.Name) + " [" + Quantity + "]";
        }
    }
}
