using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Vibe.SupplyChain.Win
{
    [DataContract]
    public class TableViewMetadata
    {
        [DataMember]
        public string ViewName { get; set; }
        [DataMember]
        public string DataNode { get; set; }
        [DataMember]
        public List<EntityFilter> Filters { get; set; }
        [DataMember]
        public List<TableViewColumn> Columns { get; set; }
        public TableViewMetadata()
        {
            Filters = new List<EntityFilter>();
            Columns = new List<TableViewColumn>();
        }
        public override string ToString()
        {
            return ViewName;
        }
        public string Serialize()
        {
            DataContractJsonSerializer js = new DataContractJsonSerializer(this.GetType());
            MemoryStream msObj = new MemoryStream();
            js.WriteObject(msObj, this);
            msObj.Position = 0;
            StreamReader sr = new StreamReader(msObj);
            return sr.ReadToEnd();
        }
        public static TableViewMetadata Deserialize(string json)
        {
            try
            {
                using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
                {
                    DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(TableViewMetadata));
                    return (TableViewMetadata)deserializer.ReadObject(ms);
                }
            }
            catch (Exception exc)
            {
                throw new Exception("Invalid Json content. " + exc.Message);
            }
        }
    }
    [DataContract]
    public class TableViewColumn
    {
        [DataMember]
        public string HeaderText { get; set; }
        [DataMember]
        public string Value { get; set; }
    }

    [DataContract]
    public class TableViewCollection
    {
        string _path;
        [DataMember]
        public List<TableViewMetadata> MetadataList { get; set; }
        public TableViewCollection(string path)
        {
            _path = path;
            Load();
        }
        void Load()
        {
            string jsoncontent = null;
            if (!File.Exists(_path))
                File.Create(_path);
            else
                jsoncontent = File.ReadAllText(_path);
            TableViewCollection col = Deserialize(jsoncontent);
            MetadataList = col == null ? new List<TableViewMetadata>() : Deserialize(jsoncontent).MetadataList;
        }
        public static TableViewCollection Deserialize(string json)
        {
            try
            {
                if (String.IsNullOrEmpty(json))
                    return null;
                using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
                {
                    DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(TableViewCollection));
                    return (TableViewCollection)deserializer.ReadObject(ms);
                }
            }
            catch (Exception exc)
            {
                throw new Exception("TableViewCollection: Invalid Json content. " + exc.Message);
            }
        }
        string Serialize()
        {
            DataContractJsonSerializer js = new DataContractJsonSerializer(this.GetType());
            MemoryStream msObj = new MemoryStream();
            js.WriteObject(msObj, this);
            msObj.Position = 0;
            StreamReader sr = new StreamReader(msObj);
            return sr.ReadToEnd();
        }
        public void Remove(string viewName)
        {
            TableViewMetadata md = MetadataList.Find(m => m.ViewName == viewName);
            if (md != null)
            {
                MetadataList.Remove(md);
                Save();
            }
        }
        public void Add(TableViewMetadata metadata)
        {
            if (String.IsNullOrEmpty(metadata.ViewName))
                throw new Exception("Name is required.");

            if (String.IsNullOrEmpty(metadata.DataNode))
                throw new Exception("DataNode is required.");
            TableViewMetadata md = MetadataList.Find(m => m.ViewName == metadata.ViewName);
            if (md != null)
            {
                MetadataList.Remove(md);
            }
            MetadataList.Add(metadata);
            Save();
        }
        void Save(string json)
        {
            File.WriteAllText(_path, json);
        }
        public void Save()
        {
            Save(this.Serialize());
        }
    }
}
