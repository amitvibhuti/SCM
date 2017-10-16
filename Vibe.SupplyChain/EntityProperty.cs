using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vibe.SupplyChain
{
    public class EntityProperty
    {
        public string Name { get; set; }
        public object Value { get; set; }
        Entity Entity { get; set; }
        PropertyInfo Property { get; set; }
        public string DisplayText
        {
            get
            {
                string retValue = Name;
                if (EntityObjectAttribute != null)
                {
                    retValue = String.IsNullOrEmpty(EntityObjectAttribute.DisplayLabel) ? Name : EntityObjectAttribute.DisplayLabel;
                }
                return retValue;
            }
        }
        public bool IsViewable
        {
            get
            {
                bool retValue = true;
                if (EntityObjectAttribute != null)
                {
                    retValue = EntityObjectAttribute.Accessibility != Accessibility.NoView;
                }
                return retValue;
            }
        }
        public bool IsEditable { get {
                bool retValue = false;
                if (EntityObjectAttribute != null)
                {
                    retValue = EntityObjectAttribute.Accessibility == Accessibility.Edit ||
                        EntityObjectAttribute.Accessibility == Accessibility.EditLarge;
                }
                return retValue;
            }
        }
        public bool IsRequired
        {
            get
            {
                bool retValue = false;
                if (EntityObjectAttribute != null)
                {
                    retValue = EntityObjectAttribute.Mandate == Mandate.Required;
                }
                return retValue;
            }
        }
        public bool IsSelect
        {
            get
            {
                bool retValue = false;
                if (EntityObjectAttribute != null)
                {
                    retValue = !String.IsNullOrEmpty(EntityObjectAttribute.SelectOptions);
                }
                return retValue;
            }
        }
        public int Sequence
        {
            get
            {
                int retValue = 999;
                if (EntityObjectAttribute != null)
                {
                    retValue = EntityObjectAttribute.Sequence;
                }
                return retValue;
            }
        }
        public List<NamedEntity> SelectOptions
        {
            get
            {
                IEnumerable<NamedEntity> retValue = new List<NamedEntity>();
                if (EntityObjectAttribute != null && IsSelect)
                {
                    if (EntityObjectAttribute.SelectOptions.ToLower().StartsWith("enum|"))
                    {
                        retValue = EnumerationEntity.Parse(EntityObjectAttribute.SelectOptions.Substring(5));
                    }
                    else
                    {
                        retValue = (IEnumerable<NamedEntity>)Entity.ParseEntityNavigation(EntityObjectAttribute.SelectOptions).Value;
                    }
                    if (!String.IsNullOrEmpty(EntityObjectAttribute.SelectEntityFilterValue))
                    {
                        //retValue = retValue.Where(e => Entity.EvaluateCriteria(EntityObjectAttribute.SelectEntityFilterValue, Name));

                        retValue = retValue.Where(e => Entity.Validate(new EntityFilter()
                        {
                            FilterNode = EntityObjectAttribute.SelectEntityFilterValue,
                            FilterValue = e.ParseText(EntityObjectAttribute.SelectElementFilterValue).ToString(),
                            FilterOperand = EntityObjectAttribute.SelectOperand
                        }));
                    }
                }
                return retValue.ToList();
            }
        }
        public bool HasConstraintViolation(Entity ent)
        {
            bool retVal= !ent.EvaluatePassCriteria(this.Name);
            return retVal;
        }
        EntityObjectAttribute EntityObjectAttribute
        {
            get {
                object[] attrs = Property.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    EntityObjectAttribute authAttr = attr as EntityObjectAttribute;
                    if (authAttr != null)
                    {
                        return authAttr;
                    }
                }
                return null;
            }
        }
        public EntityProperty(Entity ent, PropertyInfo pInfo)
        {
            Entity = ent;
            Property = pInfo;
        }
        public void SetSelectionValue(int id)
        {
            Property.SetValue(Entity, id);
        }
        public void SetValue(object value)
        {
            object val = value.ToString();
            try
            {
                switch (Property.PropertyType.Name.ToLower())
                {
                    case "int":
                    case "int16":
                    case "int32":
                        val = Convert.ToInt32(value);
                        break;
                    case "float":
                    case "double":
                        val = Convert.ToDouble(value);
                        break;
                    case "char":
                        val = Convert.ToChar(value);
                        break;
                    case "datetime":
                        if (value.ToString() == "1/1/0001 12:00:00 AM")
                        {
                            value = "1/1/2001 12:00:00 AM";
                        }
                        val = Convert.ToDateTime(value);
                        break;
                    default:
                    case "string":
                        break;
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Invalid input data. Expected " + Property.PropertyType.Name + "\r\n" + ex.Message);
            }
            Entity.ModifiedOn = DateTime.Now;
            Property.SetValue(Entity, val);
        }
        public override string ToString()
        {
            return Name + ": " + base.ToString();
        }
    }
}
