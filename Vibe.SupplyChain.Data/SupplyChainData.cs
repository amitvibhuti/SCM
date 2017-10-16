using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vibe.SupplyChain.Data
{
    public abstract class SupplyChainData : IData
    {
        public RootEntity Root { get; set; }
        public abstract void Save();
        public abstract void Reset();
        public abstract void Import(string json);
        public abstract void DeleteEntity(EntityList elist, Entity ent);

        public void LinkParent()
        { LinkParent(Root); }
        void LinkParent(Entity ent)
        {
            PropertyInfo[] pInfos = ent.GetType().GetProperties().Where(p => p.GetSetMethod() != null).ToArray();
            foreach (PropertyInfo pInfo in pInfos)
            {
                if (!typeof(IEnumerable<Entity>).IsAssignableFrom(pInfo.PropertyType)
                    || typeof(EntityProperty).IsAssignableFrom(pInfo.PropertyType)
                    || typeof(EntityList).IsAssignableFrom(pInfo.PropertyType)
                    || typeof(List<EntityProperty>).IsAssignableFrom(pInfo.PropertyType)
                    || typeof(List<EntityList>).IsAssignableFrom(pInfo.PropertyType))
                    continue;
                IEnumerable<Entity> elist = pInfo.GetValue(ent, null) as IEnumerable<Entity>;
                if (elist == null)
                    continue;
                foreach(Entity e in elist)
                {
                    e.Parent = ent;
                    LinkParent(e);
                }
            }
        }
    }
}
