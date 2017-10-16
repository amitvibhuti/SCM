using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Vibe.SupplyChain.Model
{
    [DataContract]
    public class Customer : NamedEntity
    {
        List<Order> _orders;
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Detail of Orders")]
        public List<Order> Orders
        {
            get {
                _orders.Sort((o1, o2) => o1.Date.CompareTo(o2.Date));
                return _orders;
            }
            set
            {
                _orders = value;
            }
        }
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Detail of Payments")]
        public List<Payment> Payments { get; set; }
        [DataMember]
        [EntityObjectAttribute(Accessibility = Accessibility.Edit)]
        public string Phone { get; set; }
        [DataMember]
        [EntityObjectAttribute(Accessibility = Accessibility.Edit)]
        public string Email { get; set; }
        [EntityObjectAttribute(DisplayLabel = "Payment", Accessibility = Accessibility.Auto)]
        public double TotalPaymentAmount { get {
                return Payments.Sum(p => p.Amount);
            }
        }
        [EntityObjectAttribute(DisplayLabel ="Purchase", Accessibility = Accessibility.Auto)]
        public double TotalPurchaseAmount
        {
            get
            {
                return Orders.Sum(o => o.Price);
            }
        }
        [EntityObjectAttribute(DisplayLabel = "Liability",
            CalculatedValue = CalculatedValue.Text,
            PassCriteria = "value < 0",
            SuccessColor = BGColor.Green,
            FailColor = BGColor.Red,
            Accessibility = Accessibility.Auto)]
        public double Liability
        {
            get
            {
                return TotalPurchaseAmount - TotalPaymentAmount;
            }
        }
        public Customer(Entity parent) : base(parent)
        {
            Orders = new List<Order>();
            Payments = new List<Payment>();
        }
    }
}
