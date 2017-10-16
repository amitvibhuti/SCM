using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vibe.SupplyChain
{
    public class EnumerationEntity: NamedEntity
    {
        Enum _en { get; set; }
        public EnumerationEntity() : base(null)
        {
        }
        public static IEnumerable<EnumerationEntity> Parse(string enType)
        {
            Type enumType=Type.GetType(enType);

            if (!enumType.IsEnum)
                throw new ArgumentException("Not a enum type.");

            return Parse(enumType);
        }
        public static IEnumerable<EnumerationEntity> Parse(Type enumType)
        {
            var values = Enum.GetValues(enumType);
            List<EnumerationEntity> retValue = new List<EnumerationEntity>();
            foreach (object o in values)
            {
                retValue.Add(new EnumerationEntity()
                {
                    ID = Convert.ToInt32(o),
                    Name = Enum.GetName(enumType, o)
                });
            }
            return retValue;
        }

    }
}
