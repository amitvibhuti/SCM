using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Vibe.SupplyChain
{
    [DataContract]
    public abstract class NamedEntity : Entity
    {
        [DataMember]
        [EntityObjectAttribute(Mandate = Mandate.Required, Sequence = 2, Accessibility = Accessibility.Edit)]
        public virtual string Name { get; set; }
        public NamedEntity(Entity parent):base(parent)
        { }
        public override string ToString()
        {
            return Name;
        }
    }
}
