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
    
    public partial class UIControlsVisibility
    {
        public int ID { get; set; }
        public string UIControlID { get; set; }
        public int OrderTypeID { get; set; }
    
        public virtual OrderTypes OrderTypes { get; set; }
    }
}
