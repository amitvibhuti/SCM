using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vibe.SupplyChain
{
    public class EntityList : List<Entity>, ITypedList
    {
        public string Name { get; set; }
        public string SumColumns { get; set; }
        public Type ItemType { get { return Property.PropertyType.GetGenericArguments()[0]; } }
        public Entity Entity { get; set; }
        PropertyInfo Property { get; set; }
        public bool CanEdit { get { return Property.GetSetMethod() != null; } }
        public string DisplayName
        {
            get
            {
                if (EntityObjectAttribute != null)
                {
                    return EntityObjectAttribute.DisplayLabel;
                }
                return "";
            }
        }
        public EntityObjectAttribute EntityObjectAttribute
        {
            get
            {
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
        public void ForEachColumn(Action<string, PropertyInfo, EntityObjectAttribute> act)
        {
            IEnumerable<string> attrs = Entity.GetProperties(this.ItemType);
            foreach (string attr in attrs)
            {
                PropertyInfo pInfo = this.ItemType.GetProperty(attr);
                EntityObjectAttribute eprop = Entity.GetEntityAttribute(pInfo);
                act(attr, pInfo, eprop);
            }
        }
        public bool HasConstraintViolation
        {
            get
            {
                bool retVal = this.Any(e => e.HasConstraintViolation);
                return retVal;
            }
        }
        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            PropertyDescriptorCollection pdc;

            if (listAccessors != null && listAccessors.Length > 0)
            {
                pdc = ListBindingHelper.GetListItemProperties(listAccessors[0].PropertyType);
            }
            else
            {
                pdc = TypeDescriptor.GetProperties(ItemType);
            }

            return pdc;
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return ItemType.Name;
        }
        public override string ToString()
        {
            return Name + ": " + base.ToString();
        }
        public EntityList(Entity ent, PropertyInfo pInfo)
        {
            Entity = ent;
            Property = pInfo;
        }
        public Entity CreateItem(Entity parent)
        {
            Entity enew = (Entity)Activator.CreateInstance(this.ItemType, new object[] { parent });
            AddItem(enew);
            return enew;
        }
        public void AddItem(Entity ent)
        {
            int maxid = this.Count == 0 ? 0 : this.Max(e => e.ID);
            ent.ID = maxid + 1;
            this.Add(ent);
            object prop = Property.GetValue(Entity, null);
            Property.PropertyType.GetMethod("Add").Invoke(prop, new[] { ent });
        }
        public void RemoveItem(Entity ent)
        {
            object prop = Property.GetValue(Entity, null);
            Property.PropertyType.GetMethod("Remove").Invoke(prop, new[] { ent });
        }
    }
}
