﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace API.Invoice.DB
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ZubairEntities : DbContext
    {
        public ZubairEntities()
            : base("name=ZubairEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<authorizedapp> authorizedapps { get; set; }
        public virtual DbSet<billingitem> billingitems { get; set; }
        public virtual DbSet<contractor> contractors { get; set; }
        public virtual DbSet<customer> customers { get; set; }
        public virtual DbSet<invoice> invoices { get; set; }
        public virtual DbSet<invoicefile> invoicefiles { get; set; }
        public virtual DbSet<invoiceitem> invoiceitems { get; set; }
        public virtual DbSet<invoicelog> invoicelogs { get; set; }
        public virtual DbSet<invoicestatu> invoicestatus { get; set; }
    }
}
