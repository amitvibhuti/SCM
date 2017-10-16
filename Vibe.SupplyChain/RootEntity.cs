using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Vibe.SupplyChain
{
    [DataContract]
    public class RootEntity : NamedEntity
    {
        char _currency = '\u20B9';
        [DataMember]
        [EntityObjectAttribute(Mandate = Mandate.Required, Sequence = 3, Accessibility = Accessibility.Edit, MaxLength = 1)]
        public char Currency
        {
            get
            {
                return _currency;
            }
            set { _currency = value; }
        }
        string _expLoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        [DataMember]
        [EntityObjectAttribute(Sequence = 4, Accessibility = Accessibility.Edit)]
        public string ExportLocation { get { return _expLoc; } set { _expLoc = value; } }
        public RootEntity(Entity parent):base(parent)
        {
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
