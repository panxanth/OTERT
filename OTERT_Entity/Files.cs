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
    
    public partial class Files
    {
        public int ID { get; set; }
        public Nullable<int> TaskID { get; set; }
        public Nullable<int> OrderID { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public System.DateTime DateStamp { get; set; }
        public Nullable<int> CustomerID { get; set; }
    
        public virtual Orders Orders { get; set; }
        public virtual Tasks Tasks { get; set; }
        public virtual Customers Customers { get; set; }
    }
}
