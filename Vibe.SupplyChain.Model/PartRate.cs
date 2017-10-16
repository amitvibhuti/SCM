using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Vibe.SupplyChain.Model
{
    [DataContract]
    public class PartRate : Entity
    {
        [DataMember]
        [EntityObjectAttribute(Mandate = Mandate.Required, Sequence = 4, Accessibility = Accessibility.Edit)]
        public DateTime StartDate { get; set; }
        [DataMember]
        [EntityObjectAttribute(Mandate = Mandate.Required, Sequence = 3, Accessibility = Accessibility.Edit, DisplayPrefix = "Root.Currency")]
        public double Rate { get; set; }
        public PartRate(Entity parent) : base(parent)
        {
            StartDate = DateTime.Now;
            Rate = 0;
        }
        public override string ToString()
        {
            return ID.ToString() + ": " + StartDate.ToShortDateString() + "[" + Root.Currency + " " + Rate + "]";
        }
    }
}
