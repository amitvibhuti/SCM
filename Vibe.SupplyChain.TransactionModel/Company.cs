using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
namespace Vibe.SupplyChain.TransactionModel
{   
    [DataContract]
    public class Company : RootEntity
    {
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Detail of Customers")]
        public List<BusinessPartner> Partners { get; set; }
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Detail of Parts")]
        public List<Part> Parts { get; set; }
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Detail of Inventories")]
        public List<Inventory> Inventories { get; set; }
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Country GST(%)", Accessibility = Accessibility.Edit)]
        public double CGST { get; set; }
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "State GST(%)", Accessibility = Accessibility.Edit)]
        public double SGST { get; set; }
        [EntityObjectAttribute(DisplayLabel = "Profit", 
            Accessibility = Accessibility.Auto,
            IsConstraint = true,
            PassCriteria = "value > 0",
            SuccessColor = BGColor.Green,
            FailColor = BGColor.Red)]
        public double NetProfit { get {
                return Math.Round(Partners.Sum(p => p.NetProfit), 2);
            }
        }
        [EntityObjectAttribute(DisplayLabel = "Growth %",
            Accessibility = Accessibility.Auto,
            IsConstraint = true,
            PassCriteria = "value > 0",
            SuccessColor = BGColor.Green,
            FailColor = BGColor.Red)]
        public double GrowthPercentage
        {
            get
            {
                if (NetInvestment == 0)
                    return 0;
                return Math.Round((NetProfit * 100) / NetInvestment, 2);
            }
        }
        [EntityObjectAttribute(DisplayLabel = "Tracking days", Accessibility = Accessibility.Auto)]
        public int DaysOfTrack { get { return (int)Math.Ceiling(DateTime.Now.Subtract(Root.CreatedOn).TotalDays); } }
        [EntityObjectAttribute(DisplayLabel = "Annual Growth %",
            Accessibility = Accessibility.Auto,
            IsConstraint = true,
            PassCriteria = "value > 0",
            SuccessColor = BGColor.Green,
            FailColor = BGColor.Red)]
        public double AnnualGrowthPercentage
        {
            get
            {
                if (NetInvestment == 0)
                    return 0;
                return Math.Round((Math.Pow((NetProfit + NetInvestment) / NetInvestment, (365 / DaysOfTrack)) - 1) * 100, 2);
            }
        }
        public double NetInvestment
        {
            get
            {
                return Math.Round(Inventories.Sum(i => i.GetBuyPriceTillDate(DateTime.Now)), 2);
            }
        }
        public Company():base(null)
        {
            Partners = new List<BusinessPartner>();
            Parts = new List<Part>();
            Inventories = new List<Inventory>();
        }        
    }
}
