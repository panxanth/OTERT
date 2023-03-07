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
        public System.DateTime OrderDate { get; set; }
        public int CustomerID { get; set; }
        public Nullable<int> RequestedPositionID { get; set; }
        public Nullable<System.DateTime> DateTimeStartOrder { get; set; }
        public Nullable<System.DateTime> DateTimeEndOrder { get; set; }
        public int DateTimeDurationOrder { get; set; }
        public Nullable<System.DateTime> DateTimeStartActual { get; set; }
        public Nullable<System.DateTime> DateTimeEndActual { get; set; }
        public Nullable<int> DateTimeDurationActual { get; set; }
        public Nullable<decimal> CostCalculated { get; set; }
        public string TelephoneNumber { get; set; }
        public Nullable<decimal> TechnicalSupport { get; set; }
        public Nullable<decimal> AddedCharges { get; set; }
        public Nullable<decimal> CostActual { get; set; }
        public Nullable<System.DateTime> PaymentDateOrder { get; set; }
        public Nullable<System.DateTime> PaymentDateCalculated { get; set; }
        public Nullable<System.DateTime> PaymentDateActual { get; set; }
        public Nullable<bool> IsLocked { get; set; }
        public Nullable<bool> IsCanceled { get; set; }
        public Nullable<decimal> CancelPrice { get; set; }
        public string Comments { get; set; }
        public string InvoceComments { get; set; }
        public Nullable<bool> MSN1 { get; set; }
        public Nullable<bool> MSN2 { get; set; }
        public Nullable<int> LineTypeID { get; set; }
        public System.DateTime DateStamp { get; set; }
        public string EnteredByUser { get; set; }
        public Nullable<decimal> InstallationCost { get; set; }
        public Nullable<decimal> MonthlyCharges { get; set; }
        public Nullable<decimal> MSNCost { get; set; }
        public Nullable<decimal> InvoiceCost { get; set; }
    
        public virtual Customers Customers { get; set; }
        public virtual LineTypes LineTypes { get; set; }
        public virtual OrdersPTSGR2 OrdersPTSGR2 { get; set; }
        public virtual RequestedPositions RequestedPositions { get; set; }
    }
}
