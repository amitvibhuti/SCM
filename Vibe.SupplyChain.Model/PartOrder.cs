using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Vibe.SupplyChain.Model
{
    [DataContract]
    public class PartOrder : Vibe.SupplyChain.Entity
    {
        [DataMember]
        [EntityObjectAttribute(SelectOptions = "Parent.Parent.Parent.Parts", SelectText = "Name", DisplayLabel = "Part")]
        public int PartId { get; set; }
        public Part Part
        {
            get
            {
                return ((Company)this.Parent.Parent.Parent).Parts.Find(p => p.ID == PartId);
            }
        }
        [DataMember]
        [EntityObjectAttribute(Mandate = Mandate.Required, Accessibility = Accessibility.Edit)]
        public double Quantity { get; set; }
        [EntityObjectAttribute(Accessibility = Accessibility.Auto, DisplayPrefix = "Root.Currency")]
        public double Rate
        {
            get
            {
                if (Part == null)
                    return 0;
                PartRate prate = Part.GetRate(((Order)this.Parent).Date);
                if (prate == null)
                    return 0;
                return prate.Rate;
            }
        }
        [EntityObjectAttribute(Accessibility = Accessibility.Auto, FooterMode = FooterMode.Sum, DisplayPrefix = "Root.Currency")]
        public double Price {
            get
            {
                return this.Quantity * this.Rate;
            }
        }
        [EntityObjectAttribute(DisplayLabel = "GST", Accessibility = Accessibility.Auto, DisplayPrefix = "Root.Currency", FooterMode = FooterMode.Sum)]
        public double Tax
        {
            get
            {
                return ((((Company)Root).SGST + ((Company)Root).CGST)/100) * Price;
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
        public PartOrder(Entity parent) : base(parent)
        {
            Quantity = 0;
        }
        public override string ToString()
        {
            return base.ToString() + ": " + (Part == null ? "" : Part.Name) + " [" + Quantity + "]";
        }
    }
}
