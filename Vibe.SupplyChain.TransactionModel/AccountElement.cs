using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Vibe.SupplyChain.TransactionModel
{
    public enum AccountElementType { Credit, Debit }
    [DataContract]
    public class AccountElement : Entity
    {
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Type",
            SelectOptions = "Enum|Vibe.SupplyChain.TransactionModel.AccountElementType, Vibe.SupplyChain.TransactionModel",
            Accessibility = Accessibility.Edit)]
        public AccountElementType Type { get; internal set; }
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Date", Accessibility = Accessibility.Edit)]
        public DateTime Date { get; internal set; }
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Purpose", Accessibility = Accessibility.Edit)]
        public string Purpose { get; internal set; }
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Amount", 
            Accessibility = Accessibility.Edit, 
            DisplayPrefix = "Root.Currency", 
            FooterMode = FooterMode.Sum)]
        public double Amount { get; internal set; }
        [EntityObjectAttribute(DisplayLabel = "Amount in Account",
           Accessibility = Accessibility.NoView)]
        public double PostTransactionAccountAmount
        {
            get
            {
                return ((BusinessPartner)Parent).Account.Where(e => e.Date <= Date).Sum(e => e.Amount);
            }
        }
        public Transaction Transaction { get; set; }
        public AccountElement(Entity parent, AccountElementType type, DateTime date, string purpose, double amount) : base(parent)
        {
            Type = type;
            Date = date;
            Purpose = purpose;
            Amount = (type == AccountElementType.Credit ? 1 : -1) * amount;
        }
        public AccountElement(Entity parent) : base(parent)
        {
            Type = AccountElementType.Credit;
            Date = ((BusinessPartner)Parent).CreatedOn;
            Purpose = "Account opening";
            Amount = ((BusinessPartner)Parent).InitialCredit;
        }
        public AccountElement(Entity parent, Transaction t) : base(parent)
        {
            Type = (t.TransactionType == TransactionType.Buy ? AccountElementType.Credit : AccountElementType.Debit);
            Date = t.Date;
            Purpose = "Matterial " + (t.TransactionType == TransactionType.Buy ? "baught." : "sold.");
            Amount = (Type == AccountElementType.Credit ? 1 : -1) * t.Gross;
        }
        public AccountElement(Entity parent, Payment p) : base(parent)
        {
            Type = (p.PayemntType == PayemntType.Receive ? AccountElementType.Credit : AccountElementType.Debit);
            Date = p.Date;
            Purpose = "Payment " + (p.PayemntType == PayemntType.Receive ? "received." : "sent.");
            Amount = (Type == AccountElementType.Credit ? 1 : -1) * p.Amount;
        }
    }
}
