//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class invoicelog
    {
        public int id { get; set; }
        public Nullable<int> invoice_id { get; set; }
        public Nullable<int> invoicestatusid { get; set; }
        public Nullable<System.DateTime> datecreated { get; set; }
    
        public virtual invoice invoice { get; set; }
        public virtual invoicestatu invoicestatu { get; set; }
    }
}
