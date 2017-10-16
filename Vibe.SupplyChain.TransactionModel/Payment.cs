using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Vibe.SupplyChain.TransactionModel
{
    public enum PayemntType { Send, Receive }
    [DataContract]
    public class Payment : Entity
    {
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Payment Type",
            SelectOptions = "Enum|Vibe.SupplyChain.TransactionModel.PayemntType, Vibe.SupplyChain.TransactionModel",
            Accessibility = Accessibility.Edit)]
        public PayemntType PayemntType { get; set; }
        [DataMember]
        [EntityObjectAttribute(Mandate = Mandate.Required, Accessibility = Accessibility.Edit)]
        public DateTime Date { get; set; }
        [DataMember]
        [EntityObjectAttribute(Mandate = Mandate.Required, Accessibility = Accessibility.Edit)]
        public double Amount { get; set; }
        public Payment(Entity parent) : base(parent)
        {
            Date = DateTime.Now;
        }
        public override string ToString()
        {
            return base.ToString() + " " + Date.ToShortDateString() + "(" + Amount + ")";
        }
    }
}
