using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Vibe.SupplyChain.Model
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
        [EntityObjectAttribute(Accessibility = Accessibility.Auto, DisplayPrefix = "Root.Currency")]
        public double CurrentRate
        {
            get
            {
                PartRate pr = GetRate(DateTime.Now);
                return pr == null ? 0 : pr.Rate;
            }
        }
        public PartRate GetRate(DateTime dt)
        {
            if (RateHistory.Count == 0)
                return null;
            IEnumerable<PartRate> prates = RateHistory.Where(rh => rh.StartDate < dt);
            if (prates.Count() == 0)
                prates = RateHistory.Where(rh => rh.StartDate == RateHistory.Min(r => r.StartDate)); 
            DateTime runningdate = prates.Max(rh => rh.StartDate);
            return RateHistory.Find(rh => rh.StartDate == runningdate);
        }
        public override string ToString()
        {
            return base.ToString() + "[" + Root.Currency + " " + CurrentRate + "]";
        }
    }
}
