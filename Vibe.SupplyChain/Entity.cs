using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Vibe.SupplyChain
{
    [DataContract]
    public abstract class Entity
    {
        DateTime _createdOn = new DateTime(2017, 08, 01);
        [DataMember]
        [EntityObjectAttribute(Accessibility = Accessibility.NoView)]
        public DateTime CreatedOn { get {
                _createdOn = _createdOn == DateTime.MinValue || _createdOn == DateTime.MaxValue
                    ? new DateTime(2017, 08, 01)
                    : _createdOn;
                return _createdOn;
            }
            set { _createdOn = value == DateTime.MinValue || value == DateTime.MaxValue
                    ? new DateTime(2017, 08, 01)
                    : value; } }
        DateTime _modOn = new DateTime(2017, 08, 01);
        [DataMember]
        [EntityObjectAttribute(Accessibility = Accessibility.NoView)]
        public DateTime ModifiedOn {
            get {
                _modOn = _modOn == DateTime.MinValue || _modOn == DateTime.MaxValue
                   ? new DateTime(2017, 08, 01)
                   : _modOn;
                return _modOn;
            }
            set
            {
                _modOn = value == DateTime.MinValue || value == DateTime.MaxValue
                      ? new DateTime(2017, 08, 01)
                      : value;
            }
        }
        [EntityObjectAttribute(Accessibility = Accessibility.NoView)]
        public Entity Parent { get; set; }
        [EntityObjectAttribute(Accessibility = Accessibility.NoView)]
        public string EntityType { get { return this.GetType().Name; } }
        [EntityObjectAttribute(Accessibility = Accessibility.NoView)]
        public string Hierarchy { get {
                string retValue = this.EntityType;
                Entity parent = this.Parent;
                while (parent != null)
                {
                    retValue = parent.EntityType + " >> " + retValue;
                    parent = parent.Parent;
                }
                return retValue;
            }
        }
        [EntityObjectAttribute(Accessibility = Accessibility.NoView)]
        public string DisplayID
        {
            get
            {
                string retValue = this.ID.ToString();
                Entity parent = this.Parent;
                while (parent != null)
                {
                    retValue = parent.ID + "" + retValue;
                    parent = parent.Parent;
                }
                return retValue;
            }
        }
        public Entity(Entity parent)
        {
            Parent = parent;
            CreatedOn = DateTime.Now;
        }
        [DataMember]
        [EntityObjectAttribute(Accessibility = Accessibility.Auto, Sequence = 1, Width = 60)]
        public int ID { get; set; }
        public string Serialize()
        {
            Console.WriteLine("Entity Serializing");
            DataContractJsonSerializer js = new DataContractJsonSerializer(this.GetType());
            MemoryStream msObj = new MemoryStream();
            js.WriteObject(msObj, this);
            msObj.Position = 0;
            StreamReader sr = new StreamReader(msObj);
            return sr.ReadToEnd();
        }
        public RootEntity Root
        {
            get
            {
                Entity ent = this;
                while(ent.Parent!=null)
                {
                    ent = ent.Parent;
                }
                return (RootEntity)ent;
            }
        }
        public ParsedEntityValue ParseEntityNavigation(string hierarchytext)
        {
            return ParseEntityNavigation(this, hierarchytext);
        }
        public static ParsedEntityValue ParseEntityNavigation(Entity startNode, string hierarchytext)
        {
            ParsedEntityValue pev = new ParsedEntityValue();
            if (!String.IsNullOrEmpty(hierarchytext))
            {
                string[] paths = hierarchytext.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                Entity targetNode = startNode;
                foreach (string path in paths)
                {
                    if (targetNode == null)
                        throw new Exception("Invalid navigation path.");
                    switch (path.ToLower())
                    {
                        case "root":
                            targetNode = targetNode.Root;
                            break;
                        case "parent":
                            targetNode = targetNode.Parent;
                            break;
                        default:
                            pev.Entity = targetNode;
                            pev.PropertyName = path;
                            if (targetNode.GetType().GetProperty(path) == null)
                                break;
                                //throw new Exception(path + " is not found in " + targetNode.GetType().Name);
                            pev.Value = targetNode.GetType().GetProperty(path).GetValue(targetNode, null);
                            EntityObjectAttribute attr = Entity.GetEntityAttribute(targetNode.GetType(), path);
                            pev.IsSumColumn = attr.FooterMode == FooterMode.Sum;
                            return pev;
                    }
                }
            }
            return null;
        }
        public static T Deserialize<T>(string json) where T:Entity
        {
            Console.WriteLine("Entity Deserializing");
            try
            {
                using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
                {
                    DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(T));
                    return (T)deserializer.ReadObject(ms);
                }
            }
            catch(Exception exc)
            {
                throw new Exception("Invalid Json content. " + exc.Message);
            }
        }
        public static Entity Deserialize(Type type, string json)
        {
            Console.WriteLine("Entity Deserializing");
            try
            {
                using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
                {
                    DataContractJsonSerializer deserializer = new DataContractJsonSerializer(type);
                    return (Entity)deserializer.ReadObject(ms);
                }
            }
            catch (Exception exc)
            {
                throw new Exception("Invalid Json content.\r\nThis could happen when your model is not in sync with your application version.", exc);
            }
        }
        [EntityObjectAttribute(Sequence = 0, Accessibility = Accessibility.NoView,
            DisplayLabel = "*",
            Width = 23,
            PassCriteria = "NOT value",
            CalculatedValue = CalculatedValue.Image,
            SuccessImage = ConstraintImage.Success, 
            FailImage = ConstraintImage.Fail)]
        public bool HasPropertyWithConstraintViolation
        {
            get
            {
                bool value = !Properties.TrueForAll(p => !p.HasConstraintViolation(this));
                return value;
            }
        }
        [EntityObjectAttribute(Sequence = 0, Accessibility = Accessibility.NoView)]
        public bool HasChildWithConstraintViolation
        {
            get
            {
                return this.List.Any(elist => elist.HasConstraintViolation);
            }
        }
        [EntityObjectAttribute(Sequence = 0, Accessibility = Accessibility.NoView)]
        public bool HasConstraintViolation
        {
            get
            {
                bool retVal = HasPropertyWithConstraintViolation || HasChildWithConstraintViolation;
                return retVal;
            }
        }
        public void ForEachColumn(Action<string, PropertyInfo, EntityObjectAttribute> act)
        {
            IEnumerable<string> attrs = Entity.GetProperties(this.GetType());
            foreach (string attr in attrs)
            {
                PropertyInfo pInfo = this.GetType().GetProperty(attr);
                EntityObjectAttribute eprop = GetEntityAttribute(attr);
                act(attr, pInfo, eprop);
            }
        }
        public EntityObjectAttribute GetEntityAttribute(string attr)
        {
            return Entity.GetEntityAttribute(this.GetType(), attr);
        }
        public static EntityObjectAttribute GetEntityAttribute(Type type, string attr)
        {
            PropertyInfo pInfo = type.GetProperty(attr);
            if (pInfo == null)
                throw new Exception(attr + " is not defined.");
            return Entity.GetEntityAttribute(pInfo);
        }
        public static EntityObjectAttribute GetEntityAttribute(PropertyInfo pInfo)
        {
            object[] attrs = pInfo.GetCustomAttributes(true);
            foreach (object attr in attrs)
            {
                EntityObjectAttribute eprop = attr as EntityObjectAttribute;
                if (eprop != null)
                {
                    return eprop;
                }
            }
            return null;
        }
        public object GetValue(string attr)
        {
            PropertyInfo pInfo = this.GetType().GetProperty(attr);
            return pInfo.GetValue(this);
        }
        public List<EntityProperty> Properties
        {
            get
            {
                List<EntityProperty> retValue = new List<EntityProperty>();
                IEnumerable<string> attrs = GetProperties(this.GetType());
                foreach(string attr in attrs)
                {
                    if (attr.Contains("ConstraintViolation"))
                        continue;
                    PropertyInfo pInfo = this.GetType().GetProperty(attr);
                    retValue.Add(new EntityProperty(this, pInfo) { Name = pInfo.Name, Value = pInfo.GetValue(this) });
                }
                return retValue;
            }
        }
        bool ParseCriteriaText(string criteria)
        {
            DataTable dt = new DataTable();
            return (bool)dt.Compute(criteria, "");
        }
        /// <summary>
        /// Evaluates criteria
        /// </summary>
        /// <param name="attrName"></param>
        /// <returns></returns>
        public bool EvaluatePassCriteria(string attrName)
        {
            EntityObjectAttribute eoattr = GetEntityAttribute(attrName);
            if (eoattr == null || eoattr.Accessibility == Accessibility.NoView || String.IsNullOrEmpty(eoattr.PassCriteria))
                return true;
            return EvaluateCriteria(eoattr.PassCriteria, attrName);
        }
        public bool EvaluateCriteria(string criteria, string attrName)
        {
            PropertyInfo pInfo = this.GetType().GetProperty(attrName);
            string val = pInfo.GetValue(this, null).ToString();
            criteria = criteria.Replace("value", val.ToString());
            this.ForEachColumn((attrc, pInfoc, epropc) =>
            {
                string partText = "";
                if (criteria.Contains("[" + attrc + "]") || criteria.Contains("[" + attrc + "."))
                {
                    //object pval = ;

                    int sindex = criteria.IndexOf("[" + attrc) + 1;
                    int eindex = criteria.IndexOf("]", sindex);
                    if (eindex == -1)
                        throw new Exception("Invalid criteria text.");
                    partText = criteria.Substring(sindex, eindex - sindex);
                    object pval = ParsePropertyText(partText); // ?? pval
                    criteria = criteria.Replace("[" + partText + "]", pval.ToString());
                }
            });
            return ParseCriteriaText(criteria);
        }        
        /// <summary>
        /// When PassCriteria is true then SuccessColor else FailureColor
        /// </summary>
        /// <param name="attrName"></param>
        /// <returns></returns>
        public Color EvaluateCriteriaColor(string attrName)
        {
            EntityObjectAttribute eoattr = GetEntityAttribute(attrName);
            var value = this.EvaluatePassCriteria(attrName);
            return value ? GetColor(eoattr.SuccessColor) : GetColor(eoattr.FailColor);
        }
        static Color GetColor(BGColor color)
        {
            switch (color)
            {
                case BGColor.Green:
                    return Color.LightGreen;
                case BGColor.Red:
                    return Color.MistyRose;
                case BGColor.Yellow:
                    return Color.LightYellow;
                default:
                case BGColor.None:
                    return Color.White;
            }
        }
        /// <summary>
        /// When PassCriteria is true then SuccessMesssage else FailureMessage
        /// </summary>
        /// <param name="attrName"></param>
        /// <returns></returns>
        public string EvaluateCriteriaMessage(string attrName)
        {
            EntityObjectAttribute eoattr = GetEntityAttribute(attrName);
            if (eoattr.SuccessMessage == null || eoattr.FailMessage == null)
            {
                PropertyInfo pInfo = this.GetType().GetProperty(attrName);
                return pInfo.GetValue(this, null).ToString();
            }
            else
            {
                var value = this.EvaluatePassCriteria(attrName);
                return value ? eoattr.SuccessMessage : eoattr.FailMessage;
            }
        }
        public static IEnumerable<string> GetViewableProperties(Type eType)
        {
            IEnumerable<string> retValue = GetProperties(eType);
            return retValue.Where(r => IsViewable(eType, r));
        }
        public static bool IsViewable(Type eType, string r)
        {
            EntityObjectAttribute eo = GetEntityAttribute(eType, r);
            if (eo == null) return false;
            return eo.Accessibility == Accessibility.Auto || eo.Accessibility == Accessibility.Edit;
        }
        public static IEnumerable<string> GetProperties(Type eType)
        {
            var retValue = new List<Tuple<int, string>>()
                .Select(t => new { Sequence = t.Item1, Name = t.Item2 }).ToList();
            PropertyInfo[] pInfos = eType.GetProperties();
            foreach (PropertyInfo pInfo in pInfos)
            {
                if (typeof(IEnumerable<Entity>).IsAssignableFrom(pInfo.PropertyType)
                    || typeof(EntityProperty).IsAssignableFrom(pInfo.PropertyType)
                    || typeof(EntityList).IsAssignableFrom(pInfo.PropertyType)
                    || typeof(List<EntityList>).IsAssignableFrom(pInfo.PropertyType)
                    || typeof(List<EntityProperty>).IsAssignableFrom(pInfo.PropertyType))
                    continue;
                EntityObjectAttribute eoattr = GetEntityAttribute(pInfo);
                retValue.Add(new { Sequence = eoattr == null ? 999 : eoattr.Sequence, Name = pInfo.Name });
            }
            retValue.Sort((x, y) => x.Sequence.CompareTo(y.Sequence));
            return retValue.Select(t=>t.Name);
        }
        public static List<EntityKeyType> GetParents(Entity rootEntity, Type type)
        {
            List<EntityKeyType> retValue = GetChildTypes("Root", rootEntity.GetType(), 0);
            List<EntityKeyType> immediateParent = retValue
                .Where(t => !t.Type.Equals(type) && Entity.GetListItemType(t.Type).Any(tp => tp.Type == type)).ToList();
            return immediateParent.Union(immediateParent.SelectMany(t => GetParents(rootEntity, t.Type))).ToList();
        }
        public List<EntityList> List
        {
            get
            {
                List<EntityList> retValue = new List<EntityList>();
                PropertyInfo[] pInfos = this.GetType().GetProperties();
                foreach (PropertyInfo pInfo in pInfos)
                {
                    if (!typeof(IEnumerable<Entity>).IsAssignableFrom(pInfo.PropertyType)
                        || typeof(EntityProperty).IsAssignableFrom(pInfo.PropertyType)
                        || typeof(EntityList).IsAssignableFrom(pInfo.PropertyType)
                        || typeof(List<EntityProperty>).IsAssignableFrom(pInfo.PropertyType)
                        || typeof(List<EntityList>).IsAssignableFrom(pInfo.PropertyType))
                        continue;
                    //EntityProperty eprop = GetEntityProperty(pInfo);
                    EntityList list = new EntityList(this, pInfo) { Name = pInfo.Name };
                    IEnumerable<Entity> elist = pInfo.GetValue(this, null) as IEnumerable<Entity>;
                    if (elist == null)
                        continue;
                    list.AddRange(elist);
                    if (elist.Count() > 0)
                    {
                        foreach(PropertyInfo pi in elist.First().GetType().GetProperties())
                        {
                            EntityObjectAttribute eprop = GetEntityAttribute(pi);
                            if (eprop != null && eprop.FooterMode == FooterMode.Sum)
                            {
                                list.SumColumns += "," + pi.Name;
                            }
                        }
                    }
                    retValue.Add(list);
                }
                return retValue;
            }
        }
        public List<EntityKeyType> GetChildTypes()
        {
            return GetChildTypes("Root", this.GetType(), 0);
        }
        public static List<EntityKeyType> GetChildTypes(string prefix, Type type, int depth)
        {
            List<EntityKeyType> types = new List<EntityKeyType>();
            types.Add(new EntityKeyType(prefix, type) { DepthFromRoot = depth });
            List<EntityKeyType> lsttypes = Entity.GetListItemType(type);
            foreach(EntityKeyType lsttype in lsttypes)
            {
                List<EntityKeyType> chld = GetChildTypes(prefix + "." + lsttype.Key, lsttype.Type, depth + 1);
                types = types.Union(chld).ToList();
            }
            return types;
        }
        public static List<EntityKeyType> GetListItemType(Type eType)
        {
            List<EntityKeyType> retValue = new List<EntityKeyType>();
            PropertyInfo[] pInfos = eType.GetProperties();
            foreach (PropertyInfo pInfo in pInfos)
            {
                if (!typeof(IEnumerable<Entity>).IsAssignableFrom(pInfo.PropertyType)
                    || typeof(EntityProperty).IsAssignableFrom(pInfo.PropertyType)
                    || typeof(EntityList).IsAssignableFrom(pInfo.PropertyType)
                    || typeof(List<EntityProperty>).IsAssignableFrom(pInfo.PropertyType)
                    || typeof(List<EntityList>).IsAssignableFrom(pInfo.PropertyType))
                    continue;
                retValue.Add(new EntityKeyType(pInfo.Name, pInfo.PropertyType.GetGenericArguments()[0]));
            }
            return retValue;
        }
        public override string ToString()
        {
            return ID.ToString();
        }
        public object ParseText(string text)
        {
            bool retValue = false;
            if (Boolean.TryParse(text, out retValue))
                return retValue;

            DateTime date = DateTime.MinValue;
            if (DateTime.TryParse(text, out date))
                return date;

            int num = 0;
            if (Int32.TryParse(text, out num))
                return num;

            double dbl = 0;
            if (Double.TryParse(text, out dbl))
                return dbl;

            object val = ParsePropertyText(text);
            if (val != null)
                return val;

            val = this.ParseEntityNavigation(text);
            if (val != null)
                return val;

            return text;
        }
        public object ParsePropertyText(string partText)
        {
            object pval = null;
            string[] parts = partText.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1)
            {
                pval = this;
                for (int i = 0; i < parts.Length; i++)
                {
                    if (typeof(Entity).IsAssignableFrom(pval.GetType()))
                    {
                        PropertyInfo pInfoN = pval.GetType().GetProperty(parts[i]);
                        pval = pInfoN.GetValue(pval, null);

                    }
                    else if (i < parts.Length - 1)
                        throw new Exception("Invalid criteria text.");
                }
            }
            else
            {
                var pi = this.GetType().GetProperty(partText);
                if (pi != null)
                    pval = pi.GetValue(this, null);
            }
            return pval;
        }
        public bool Validate(EntityFilter filter)
        {
            object valL = ParseText(filter.FilterNode);
            object valR = ParseText(filter.FilterValue);
            if (valL.GetType().IsEnum)
                valL = valL.ToString();
            IComparable valLC = (IComparable)valL;
            IComparable valRC = (IComparable)valR;
            bool retvalue = false;
            switch(filter.FilterOperand)
            {
                case Operand.Equal:
                    retvalue = valL.Equals(valR);
                    break;
                case Operand.GreaterThan:
                    retvalue = valLC != null && valLC.CompareTo(valRC) > 0;
                    break;
                case Operand.LessThan:
                    retvalue = valRC != null && valRC.CompareTo(valLC) > 0;
                    break;
            }
            return retvalue;
        }
    }

    [DataContract]
    public enum Operand { Equal, GreaterThan, LessThan }

    [DataContract]
    public class EntityFilter
    {
        [DataMember]
        public string FilterNode { get; set; }
        [DataMember]
        public Operand FilterOperand { get; set; }
        [DataMember]
        public string FilterValue { get; set; }
    }

    public class ParsedEntityValue
    {
        public object Value { get; set; }
        public Entity Entity { get; set; }
        public string PropertyName { get; set; }
        public bool IsSumColumn { get; set; }
    }
    public class EntityKeyType
    {
        public int DepthFromRoot { get; set; }
        public string Key { get; set; }
        public Type Type { get; set; }
        public string Display { get { return Type.Name; } }
        public EntityKeyType(string key, Type type)
        {
            Key = key;
            Type = type;
        }
    }
}
