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
    
    public partial class authorizedapp
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string AppToken { get; set; }
        public string AppSecret { get; set; }
        public Nullable<System.DateTime> TokenExpiration { get; set; }
    }
}
