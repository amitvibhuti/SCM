using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vibe.SupplyChain.Data
{
    public class DataManager
    {
        public IData Data { get; set; }
        public DataManager():this(false)
        { }
        public DataManager(bool reset)
        {
            try
            {
                string datasource = ConfigurationManager.AppSettings["DataSource"];
                if (String.IsNullOrEmpty(datasource))
                    return;
                string[] dsParts = datasource.Split(new char[] { '|' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (dsParts.Length < 1)
                    throw new Exception("Invalid DataSource");
                string[] dsTypeParts = dsParts[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (dsTypeParts.Length == 1)
                {
                    Type type = Assembly.GetExecutingAssembly().GetType(dsTypeParts[0]);
                    object[] args = dsParts.Length == 1 ? new object[] { } : new object[] { dsParts[1], reset };
                    Data = (IData)Activator.CreateInstance(type, args);
                }
                else
                {
                    // TODO: External assembly
                }
                Data.LinkParent();
            }
            catch (TargetInvocationException exc)
            {
                throw new Exception(exc.InnerException.Message);
            }
        }
        public static DataManager Reset()
        {
            return new DataManager(true);
        }
        public void Import(string json)
        {
            Data.Import(json);
            Data.LinkParent();
        }
    }
}
