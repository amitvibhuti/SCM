using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Vibe.SupplyChain.TransactionModel
{
    [DataContract]
    public class BusinessPartner : NamedEntity
    {
        [DataMember]
        [EntityObjectAttribute(Sequence = 2, Accessibility = Accessibility.EditLarge)]
        public string Address { get; set; }
        [DataMember]
        [EntityObjectAttribute(Sequence = 3, Accessibility = Accessibility.Edit)]
        public string City { get; set; }
        [DataMember]
        [EntityObjectAttribute(Sequence = 4, Accessibility = Accessibility.Edit)]
        public string Country { get; set; }
        [DataMember]
        [EntityObjectAttribute(Sequence = 5, Accessibility = Accessibility.Edit)]
        public string Zip { get; set; }
        [DataMember]
        [EntityObjectAttribute(Sequence = 6, Accessibility = Accessibility.Edit)]
        public string Phone { get; set; }
        [DataMember]
        [EntityObjectAttribute(Sequence = 7, Accessibility = Accessibility.Edit)]
        public string Email { get; set; }
        [DataMember]
        [EntityObjectAttribute(Sequence = 8, Accessibility = Accessibility.Edit)]
        public double InitialCredit { get; set; }
        [EntityObjectAttribute(Sequence = 9, 
            DisplayLabel = "Received", 
            DisplayPrefix = "Root.Currency", 
            FooterMode = FooterMode.Sum, 
            Accessibility = Accessibility.Auto)]
        public double TotalPaymentReceived
        {
            get
            {
                return Payments.Where(p => p.PayemntType == PayemntType.Receive).Sum(p => p.Amount);
            }
        }
        [EntityObjectAttribute(Sequence = 10, 
            DisplayLabel = "Paid", 
            DisplayPrefix = "Root.Currency", 
            FooterMode = FooterMode.Sum, 
            Accessibility = Accessibility.Auto)]
        public double TotalPaymentSent
        {
            get
            {
                return Payments.Where(p => p.PayemntType == PayemntType.Send).Sum(p => p.Amount);
            }
        }

        [EntityObjectAttribute(Sequence = 11, 
            DisplayLabel = "Buy", 
            DisplayPrefix = "Root.Currency", 
            FooterMode = FooterMode.Sum, 
            Accessibility = Accessibility.Auto)]
        public double TotalBuyTransaction
        {
            get
            {
                return Transactions.Where(t => t.TransactionType == TransactionType.Buy).Sum(t => t.Gross);
            }
        }
        [EntityObjectAttribute(Sequence = 12, 
            DisplayLabel = "Sell", 
            DisplayPrefix = "Root.Currency", 
            FooterMode = FooterMode.Sum, 
            Accessibility = Accessibility.Auto)]
        public double TotalSellTransaction
        {
            get
            {
                return Transactions.Where(t => t.TransactionType == TransactionType.Sell).Sum(t => t.Gross);
            }
        }
        /// <summary>
        /// Payment due on Partner. 
        /// Positive value: Partner should pay to company
        /// Negative value: Company should pay to Partner
        /// </summary>
        [EntityObjectAttribute(Sequence = 13, 
            DisplayLabel = "Liability", 
            DisplayPrefix = "Root.Currency", 
            FooterMode = FooterMode.Sum,
            CalculatedValue = CalculatedValue.Text,
            PassCriteria = "value < 0",
            SuccessColor = BGColor.Green,
            FailColor = BGColor.Red,
            Accessibility = Accessibility.Auto)]
        public double PaymentDue 
        {
            get
            {
                return (TotalPaymentSent - TotalPaymentReceived) + (TotalSellTransaction - TotalBuyTransaction) - InitialCredit;
            }
        }
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Detail of Transactions")]
        public List<Transaction> Transactions { get; set; }
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Detail of Payments")]
        public List<Payment> Payments { get; set; }
        [EntityObjectAttribute(DisplayLabel = "Account", 
            ShowChart = "Area,Orders flow,Date,PostTransactionAccountAmount",
            Accessibility = Accessibility.Auto)]
        public List<AccountElement> Account { get {
                List<AccountElement> acc = new List<AccountElement>();
                acc.Add(new AccountElement(this));
                acc.AddRange(Payments.Select(p => new AccountElement(this, p))
                        .Union<AccountElement>(Transactions.Select(t => new AccountElement(this,t))).ToList());
                return acc;
            }
        }
        [EntityObjectAttribute(DisplayLabel = "Profit", Accessibility = Accessibility.Auto)]
        public double NetProfit { get {
                return Transactions.Sum(t => t.NetProfit );
            }
        }
        public BusinessPartner(Entity parent) : base(parent)
        {
            Transactions = new List<Transaction>();
            Payments = new List<Payment>();
        }

    }
}
