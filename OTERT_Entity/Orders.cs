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
    
    public partial class Orders
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Orders()
        {
            this.Tasks = new HashSet<Tasks>();
            this.Files = new HashSet<Files>();
            this.TasksPTSGR = new HashSet<TasksPTSGR>();
            this.OrdersPTSGR = new HashSet<OrdersPTSGR>();
        }
    
        public int ID { get; set; }
        public int OrderTypeID { get; set; }
        public string RegNo { get; set; }
        public int Customer1ID { get; set; }
        public Nullable<int> Customer2ID { get; set; }
        public int EventID { get; set; }
        public Nullable<bool> IsLocked { get; set; }
    
        public virtual OrderTypes OrderTypes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tasks> Tasks { get; set; }
        public virtual Customers Customers1 { get; set; }
        public virtual Customers Customers11 { get; set; }
        public virtual Events Events { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Files> Files { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TasksPTSGR> TasksPTSGR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrdersPTSGR> OrdersPTSGR { get; set; }
    }
}
