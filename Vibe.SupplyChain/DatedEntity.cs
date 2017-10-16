using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Vibe.SupplyChain
{
    [DataContract]
    public abstract class DatedEntity : Entity
    {
        [DataMember]
        [EntityObjectAttribute(Mandate = Mandate.Required, Sequence = 2, Accessibility = Accessibility.Edit)]
        DateTime _date = DateTime.Now;
        [DataMember]
        [EntityObjectAttribute(Mandate = Mandate.Required,
            Sequence = 3,
            Accessibility = Accessibility.Edit)]
        public DateTime Date { get { return _date; } set { _date = value; } }
        public DatedEntity(Entity parent):base(parent)
        { }
        public override string ToString()
        {
            return ID.ToString() + ": " + Date.ToShortDateString();
        }
    }
}
