//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OTERT_Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class TasksPTSGR
    {
        public int ID { get; set; }
        public int OrderPTSGR2ID { get; set; }
        public string RegNo { get; set; }
        public string InvoiceProtocol { get; set; }
        public System.DateTime OrderDate { get; set; }
        public int CustomerID { get; set; }
        public Nullable<int> RequestedPositionID { get; set; }
        public string TelephoneNumber { get; set; }
        public string CorrespondentName { get; set; }
        public int PTSRPricelistID { get; set; }
        public int MSNCount { get; set; }
        public string MSN1 { get; set; }
        public string MSN2 { get; set; }
        public Nullable<System.DateTime> DateTimeStartActual { get; set; }
        public Nullable<System.DateTime> DateTimeEndActual { get; set; }
        public Nullable<int> DateTimeDurationActual { get; set; }
        public Nullable<decimal> CallChardes { get; set; }
        public Nullable<decimal> AddedCharges { get; set; }
        public Nullable<decimal> InvoiceCost { get; set; }
        public Nullable<decimal> DailyCost { get; set; }
        public Nullable<decimal> SubscriberFee { get; set; }
        public Nullable<decimal> CostActual { get; set; }
        public Nullable<System.DateTime> PaymentDateActual { get; set; }
        public Nullable<bool> IsLocked { get; set; }
        public Nullable<bool> IsCanceled { get; set; }
        public string Comments { get; set; }
        public System.DateTime DateStamp { get; set; }
        public string EnteredByUser { get; set; }
    
        public virtual Customers Customers { get; set; }
        public virtual OrdersPTSGR2 OrdersPTSGR2 { get; set; }
        public virtual PTSGRPricelist PTSGRPricelist { get; set; }
        public virtual RequestedPositions RequestedPositions { get; set; }
    }
}
