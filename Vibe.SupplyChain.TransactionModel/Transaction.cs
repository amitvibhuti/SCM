using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Vibe.SupplyChain.TransactionModel
{
    public enum TransactionType { Buy, Sell}
    [DataContract]
    public class Transaction : DatedEntity
    {
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Transaction Type", 
            Sequence = 2,
            SelectOptions = "Enum|Vibe.SupplyChain.TransactionModel.TransactionType, Vibe.SupplyChain.TransactionModel", 
            Accessibility = Accessibility.Edit)]
        public TransactionType TransactionType { get; set; }
        
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Invoice details")]
        public List<PartTransaction> PartTransactions { get; set; }
        [EntityObjectAttribute(DisplayLabel = "Price", 
            Sequence = 5,
            Accessibility = Accessibility.Auto, DisplayPrefix = "Root.Currency", FooterMode = FooterMode.Sum)]
        public double Price
        {
            get
            {
                return PartTransactions.Sum(po => po.Price);
            }
        }
        [EntityObjectAttribute(DisplayLabel = "GST", 
            Sequence = 6,
            Accessibility = Accessibility.Auto, DisplayPrefix = "Root.Currency", FooterMode = FooterMode.Sum)]
        public double Tax
        {
            get
            {
                return PartTransactions.Sum(po => po.Tax);
            }
        }
        [EntityObjectAttribute(DisplayLabel = "Gross total", 
            Sequence = 7,
            Accessibility = Accessibility.Auto, DisplayPrefix = "Root.Currency", FooterMode = FooterMode.Sum)]
        public double Gross
        {
            get
            {
                return Tax + Price;
            }
        }
        [EntityObjectAttribute(DisplayLabel = "Payment", 
            Sequence = 9,
            Accessibility = Accessibility.Auto, DisplayPrefix = "Root.Currency", FooterMode = FooterMode.Sum)]
        public double PaymentProcessed
        {
            get
            {
                BusinessPartner cst = (BusinessPartner)this.Parent;
                double prevTransact = cst.Transactions.Where(o => o.Date < this.Date).Sum(o => o.Gross);
                double prevExcess = cst.InitialCredit + (cst.TotalPaymentReceived - cst.TotalPaymentSent) - prevTransact;
                if (prevExcess <= 0)
                {
                    return 0;
                }
                else if (prevExcess >= this.Gross)
                {
                    return this.Gross;
                }
                else
                    return prevExcess;
            }
        }
        [EntityObjectAttribute(DisplayLabel = "Status",
            Sequence = 10,
            CalculatedValue = CalculatedValue.Text,
            PassCriteria = "value",
            SuccessMessage = "Clear",
            SuccessColor = BGColor.Green,
            FailMessage = "Pending",
            FailColor = BGColor.Red,
            Accessibility = Accessibility.Auto)]
        public bool PaymentCleared
        {
            get
            {
                return Gross <= PaymentProcessed;
            }
        }
        [EntityObjectAttribute(DisplayLabel = "Inventory Cost", 
            Sequence = 4,
            Accessibility = Accessibility.Auto, DisplayPrefix = "Root.Currency")]
        public double InventoryCost
        {
            get
            {
                return this.PartTransactions.Sum(pt => pt.InventoryCost);
            }
        }
        [EntityObjectAttribute(DisplayLabel = "Profit", 
            Sequence = 8,
            Accessibility = Accessibility.Auto)]
        public double NetProfit
        {
            get
            {
                return this.PartTransactions.Sum(pt => pt.NetProfit);
            }
        }
        public Transaction(Entity parent) : base(parent)
        {
            PartTransactions = new List<PartTransaction>();
        }
        public override string ToString()
        {
            int partcnt = PartTransactions == null ? 0 : PartTransactions.Count;
            return ID.ToString() + ": " + Date.ToShortDateString() + "[" + partcnt + "]";
        }
    }
}
