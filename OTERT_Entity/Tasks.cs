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
    
    public partial class Tasks
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tasks()
        {
            this.Files = new HashSet<Files>();
            this.TasksLine = new HashSet<TasksLine>();
        }
    
        public int ID { get; set; }
        public Nullable<int> OrderID { get; set; }
        public string RegNo { get; set; }
        public System.DateTime OrderDate { get; set; }
        public int CustomerID { get; set; }
        public Nullable<int> RequestedPositionID { get; set; }
        public int JobID { get; set; }
        public Nullable<System.DateTime> DateTimeStartOrder { get; set; }
        public Nullable<System.DateTime> DateTimeEndOrder { get; set; }
        public int DateTimeDurationOrder { get; set; }
        public Nullable<System.DateTime> DateTimeStartActual { get; set; }
        public Nullable<System.DateTime> DateTimeEndActual { get; set; }
        public Nullable<int> DateTimeDurationActual { get; set; }
        public Nullable<decimal> CostCalculated { get; set; }
        public Nullable<bool> InstallationCharges { get; set; }
        public Nullable<bool> MonthlyCharges { get; set; }
        public Nullable<decimal> CallCharges { get; set; }
        public string TelephoneNumber { get; set; }
        public Nullable<decimal> TechnicalSupport { get; set; }
        public Nullable<decimal> AddedCharges { get; set; }
        public Nullable<decimal> CostActual { get; set; }
        public Nullable<System.DateTime> PaymentDateOrder { get; set; }
        public Nullable<System.DateTime> PaymentDateCalculated { get; set; }
        public Nullable<System.DateTime> PaymentDateActual { get; set; }
        public Nullable<bool> IsForHelpers { get; set; }
        public Nullable<bool> IsLocked { get; set; }
        public Nullable<bool> IsCanceled { get; set; }
        public Nullable<decimal> CancelPrice { get; set; }
        public string Comments { get; set; }
        public string InvoceComments { get; set; }
        public Nullable<int> SateliteID { get; set; }
        public Nullable<int> DistancesID { get; set; }
        public Nullable<bool> Internet { get; set; }
        public Nullable<bool> MSN { get; set; }
        public Nullable<int> LineTypeID { get; set; }
        public System.DateTime DateStamp { get; set; }
        public string EnteredByUser { get; set; }
    
        public virtual Customers Customers { get; set; }
        public virtual Jobs Jobs { get; set; }
        public virtual Orders Orders { get; set; }
        public virtual RequestedPositions RequestedPositions { get; set; }
        public virtual Satelites Satelites { get; set; }
        public virtual Distances Distances { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Files> Files { get; set; }
        public virtual LineTypes LineTypes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TasksLine> TasksLine { get; set; }
    }
}
