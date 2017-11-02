using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Vibe.SupplyChain.TransactionModel
{
    public enum RequestStatus { Open, Approved, Rejected, Received }
    [DataContract]
    public class DemandRequest : DatedEntity
    {
        [DataMember]
        [EntityObjectAttribute(SelectOptions = "Root.Partners",
            Sequence = 2,
            SelectText = "Name",
            DisplayLabel = "Supplier")]
        public int SupplierID { get; set; }
        public BusinessPartner Supplier
        {
            get
            {
                return ((Company)Root).Partners.Find(p => p.ID == SupplierID);
            }
        }
        [DataMember]
        [EntityObjectAttribute(Mandate = Mandate.Required, Sequence = 2, Accessibility = Accessibility.Edit)]
        public double Quantity { get; set; }
        [DataMember]
        [EntityObjectAttribute(DisplayLabel = "Status",
            SelectOptions = "Enum|Vibe.SupplyChain.TransactionModel.RequestStatus, Vibe.SupplyChain.TransactionModel",
            Accessibility = Accessibility.Edit)]
        public RequestStatus Status { get; set; }
        [EntityObjectAttribute(Mandate = Mandate.Required, Sequence = 4, Accessibility = Accessibility.Auto)]
        public bool IsActive { get { return Status == RequestStatus.Open; } }
        public DemandRequest(Entity parent) : base(parent)
        {
            Date = DateTime.Now;
            Status = RequestStatus.Open;
            Quantity = 0;
        }

        public override string ToString()
        {
            return ID.ToString() + ": " + Date.ToShortDateString() + "|Q:" + Quantity;
        }
        public override void OnUpdated()
        {
            if (Status == RequestStatus.Received)
            {
                Transaction trx = new Transaction(Supplier) { TransactionType = TransactionType.Buy };
                PartTransaction ptrx = new PartTransaction(trx) { PartId = ((Inventory)Parent).PartId };
                InventoryPartTransaction iptrx = new InventoryPartTransaction(ptrx);
                iptrx.Quantity = Quantity;
                ptrx.InventoryPartTransactions.Add(iptrx);
                trx.PartTransactions.Add(ptrx);
                Supplier.Transactions.Add(trx);
                //((Inventory)Parent).Quantity += Quantity;
            }
        }
    }
}
