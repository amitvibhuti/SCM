using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Vibe.SupplyChain.TransactionModel
{
    [DataContract]
    public class InventoryPartTransaction : Entity
    {
        [DataMember]
        [EntityObjectAttribute(SelectOptions = "Root.Inventories", 
            Sequence = 2,
            SelectEntityFilterValue = "Part.ID", 
            SelectElementFilterValue = "PartId", 
            SelectOperand = Operand.Equal,
            SelectText = "Name", 
            DisplayLabel = "Inventory")]
        public int InventoryId { get; set; }
        public Inventory Inventory
        {
            get
            {
                return ((Company)Root).Inventories.Find(i => i.ID == InventoryId);
            }
        }
        [DataMember]
        [EntityObjectAttribute(Mandate = Mandate.Required, 
            Sequence = 3,
            Accessibility = Accessibility.Edit)]
        public double Quantity { get; set; }
        public PartTransaction PartTransaction
        {
            get
            {
                return (PartTransaction)this.Parent;
            }
        }
        public Part Part
        {
            get
            {
                return PartTransaction.Part;
            }
        }
        [EntityObjectAttribute(Accessibility = Accessibility.Auto, 
            Sequence = 4,
            FooterMode = FooterMode.Sum, DisplayPrefix = "Root.Currency")]
        public double Price
        {
            get
            {
                return this.Quantity * this.PartTransaction.Rate;
            }
        }
        [EntityObjectAttribute(DisplayLabel = "GST", 
            Sequence = 5,
            Accessibility = Accessibility.Auto, DisplayPrefix = "Root.Currency", FooterMode = FooterMode.Sum)]
        public double Tax
        {
            get
            {
                return ((((Company)Root).SGST + ((Company)Root).CGST) / 100) * Price;
            }
        }
        [EntityObjectAttribute(DisplayLabel = "Gross", 
            Sequence = 6,
            Accessibility = Accessibility.Auto, DisplayPrefix = "Root.Currency", FooterMode = FooterMode.Sum)]
        public double Gross
        {
            get
            {
                return Tax + Price;
            }
        }
        public InventoryPartTransaction(Entity parent) : base(parent)
        {
            Quantity = 0;
        }
        public override string ToString()
        {
            return base.ToString() + ": " 
                + (Part == null ? "" : Part.Name) + "|"
                + (Inventory == null ? "" : Inventory.Name) 
                + " [" + Quantity + "]";
        }
    }
}
