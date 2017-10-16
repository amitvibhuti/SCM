using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Vibe.SupplyChain.TransactionModel
{
    [DataContract]
    public class Part : NamedEntity
    {
        
        [DataMember]
        public List<PartRate> RateHistory { get; set; }
        public Part(Entity parent) : base(parent)
        {
            RateHistory = new List<PartRate>();
        }
        [EntityObjectAttribute(DisplayLabel = "Buy rate", Accessibility = Accessibility.Auto, DisplayPrefix = "Root.Currency")]
        public double CurrentBuyRate
        {
            get
            {
                PartRate pr = GetTransactionRate(DateTime.Now);
                return pr == null ? 0 : pr.BuyRate;
            }
        }
        [EntityObjectAttribute(DisplayLabel = "Sell rate", Accessibility = Accessibility.Auto, DisplayPrefix = "Root.Currency")]
        public double CurrentSellRate
        {
            get
            {
                PartRate pr = GetTransactionRate(DateTime.Now);
                return pr == null ? 0 : pr.SellRate;
            }
        }
        public PartRate GetTransactionRate(DateTime dt)
        {
            if (RateHistory.Count == 0)
                return null;
            IEnumerable<PartRate> prates = RateHistory.Where(rh => rh.StartDate < dt);
            if (prates.Count() == 0)
                prates = RateHistory.Where(rh => rh.StartDate == RateHistory.Min(r => r.StartDate)); 
            DateTime runningdate = prates.Max(rh => rh.StartDate);
            return RateHistory.Find(rh => rh.StartDate == runningdate);
        }
        public double GetCostRate(DateTime date)
        {
            IEnumerable<Inventory> invs = ((Company)Root).Inventories.Where(i => i.PartId == ID);
            double spentOnDate = invs.Sum(i => i.GetBuyPriceTillDate(date));
            double boughtQty = invs.Sum(i => i.GetBoughtQuantityTillDate(date));
            return spentOnDate / boughtQty;
        }
        public override string ToString()
        {
            return base.ToString() + "[" + Root.Currency + "|B:" + CurrentBuyRate + "|S:" + CurrentSellRate + "]";
        }
    }
}
