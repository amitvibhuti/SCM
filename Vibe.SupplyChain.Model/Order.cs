using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Vibe.SupplyChain.Model
{
    [DataContract]
    public class Order : Vibe.SupplyChain.Entity
    {
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Invoice details")]
        public List<PartOrder> PartOrders { get; set; }
        [DataMember]
        [EntityObjectAttribute(Mandate = Mandate.Required, Accessibility = Accessibility.Edit)]
        public DateTime Date { get; set; }
        [EntityObjectAttribute(DisplayLabel = "Price", Accessibility = Accessibility.Auto, DisplayPrefix = "Root.Currency", FooterMode = FooterMode.Sum)]
        public double Price { get {
                return PartOrders.Sum(po => po.Price);
            }
        }
        [EntityObjectAttribute(DisplayLabel = "GST", Accessibility = Accessibility.Auto, DisplayPrefix = "Root.Currency", FooterMode = FooterMode.Sum)]
        public double Tax { get {
                return PartOrders.Sum(po => po.Tax);
            }
        }
        [EntityObjectAttribute(DisplayLabel = "Gross total", Accessibility = Accessibility.Auto, DisplayPrefix = "Root.Currency", FooterMode = FooterMode.Sum)]
        public double Gross
        {
            get
            {
                return Tax + Price;
            }
        }
        [EntityObjectAttribute(DisplayLabel = "Payment", Accessibility = Accessibility.Auto, DisplayPrefix = "Root.Currency", FooterMode = FooterMode.Sum)]
        public double PaymentRecieved
        {
            get
            {
                Customer cst = (Customer)this.Parent;

                double prevPurchase = cst.Orders.Where(o => o.Date < this.Date).Sum(o => o.Price);
                double prevExcess = cst.TotalPaymentAmount - prevPurchase;
                if (prevExcess <= 0)
                {
                    return 0;
                }
                else if (prevExcess >= this.Price)
                {
                    return this.Price;
                }
                else
                    return prevExcess;
            }
        }
        [EntityObjectAttribute(DisplayLabel = "Status", 
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
                return Price <= PaymentRecieved;
            }
        }
        public Order(Entity parent) : base(parent)
        {
            PartOrders = new List<PartOrder>();
            Date = DateTime.Now;
        }
        public override string ToString()
        {
            int partcnt = PartOrders == null ? 0 : PartOrders.Count;
            return ID.ToString() + ": " + Date.ToShortDateString() + "[" + partcnt + "]";
        }
    }
}
