using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Vibe.SupplyChain.Model
{   
    [DataContract]
    public class Company : RootEntity
    {
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Detail of Customers")]
        public List<Customer> Customers { get; set; }
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Detail of Parts")]
        public List<Part> Parts { get; set; }
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Detail of Inventories")]
        public List<Inventory> Inventories { get; set; }
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Detail of Purchases")]
        public List<Purchase> Purchases { get; set; }
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Country GST(%)", Accessibility = Accessibility.Edit)]
        public double CGST { get; set; }
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "State GST(%)", Accessibility = Accessibility.Edit)]
        public double SGST { get; set; }
        public Company():base(null)
        {
            Customers = new List<Customer>();
            Parts = new List<Part>();
            Inventories = new List<Inventory>();
            Purchases = new List<Purchase>();
        }        
    }
}
