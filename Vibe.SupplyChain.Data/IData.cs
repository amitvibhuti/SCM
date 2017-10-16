using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vibe.SupplyChain.Data
{
    public interface IData
    {
        RootEntity Root { get; set; }
        void Import(string json);
        void Save();
        void Reset();
        void LinkParent();
        void DeleteEntity(EntityList elist, Entity ent);
    }
}
