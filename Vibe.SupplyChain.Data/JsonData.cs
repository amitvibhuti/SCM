using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vibe.SupplyChain.Data
{
    public class JsonData : SupplyChainData
    {
        string _path;
        public JsonData(string path, bool reset)
        {
            _path = path;
            Load(reset);
        }
        void Load(bool reset)
        {
            string jsoncontent = null;
            if (!File.Exists(_path))
                File.Create(_path);
            else if (reset)
            {
                File.Delete(_path);
                File.Create(_path);
            }
            else
                jsoncontent = File.ReadAllText(_path);
            Root = CreateRoot(jsoncontent);
        }
        RootEntity CreateRoot(string jsoncontent)
        {
            string root = ConfigurationManager.AppSettings["RootEntity"];
            Type rootType = Type.GetType(root);
            if (String.IsNullOrEmpty(jsoncontent))
            {
                RootEntity rEnt = (RootEntity)Activator.CreateInstance(rootType);
                rEnt.Name = "Root";
                return rEnt;
            }
            else
                return (RootEntity)RootEntity.Deserialize(rootType, jsoncontent);
        }
        public override void Import(string json)
        {
            Save(json);
            Root = CreateRoot(json);
        }
        void Save(string json)
        {
            File.WriteAllText(_path, json);
        }
        public override void Reset()
        {
            Root = CreateRoot(null);
            Save();
        }
        public override void Save()
        {
            Save(Root.Serialize());
        }
        public override void DeleteEntity(EntityList elist, Entity ent)
        {
            elist.RemoveItem(ent);
            Save();
        }
        
    }
}
