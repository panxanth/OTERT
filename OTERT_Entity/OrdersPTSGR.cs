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
    
    public partial class OrdersPTSGR
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrdersPTSGR()
        {
            this.OrdersPTSGR2 = new HashSet<OrdersPTSGR2>();
            this.Files = new HashSet<Files>();
        }
    
        public int ID { get; set; }
        public int EventID { get; set; }
        public Nullable<System.DateTime> DateTimeStart { get; set; }
        public Nullable<System.DateTime> DateTimeEnd { get; set; }
    
        public virtual Events Events { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrdersPTSGR2> OrdersPTSGR2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Files> Files { get; set; }
    }
}
