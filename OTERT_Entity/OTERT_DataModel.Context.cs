﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class OTERTConnStr : DbContext
    {
        public OTERTConnStr()
            : base("name=OTERTConnStr")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Countries> Countries { get; set; }
        public virtual DbSet<CountryPricelist> CountryPricelist { get; set; }
        public virtual DbSet<Customers> Customers { get; set; }
        public virtual DbSet<CustomerTypes> CustomerTypes { get; set; }
        public virtual DbSet<Distances> Distances { get; set; }
        public virtual DbSet<Events> Events { get; set; }
        public virtual DbSet<Jobs> Jobs { get; set; }
        public virtual DbSet<Languages> Languages { get; set; }
        public virtual DbSet<LineTypes> LineTypes { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<OrderTypes> OrderTypes { get; set; }
        public virtual DbSet<Places> Places { get; set; }
        public virtual DbSet<RequestedPositions> RequestedPositions { get; set; }
        public virtual DbSet<Satelites> Satelites { get; set; }
        public virtual DbSet<Tasks> Tasks { get; set; }
        public virtual DbSet<UIControlsVisibility> UIControlsVisibility { get; set; }
        public virtual DbSet<UserGroups> UserGroups { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Sales> Sales { get; set; }
        public virtual DbSet<SalesFormulas> SalesFormulas { get; set; }
        public virtual DbSet<JobCancelPrices> JobCancelPrices { get; set; }
        public virtual DbSet<JobFormulas> JobFormulas { get; set; }
        public virtual DbSet<JobsMain> JobsMain { get; set; }
        public virtual DbSet<Files> Files { get; set; }
        public virtual DbSet<DocumentReplacemets> DocumentReplacemets { get; set; }
        public virtual DbSet<JobTypes> JobTypes { get; set; }
    }
}
