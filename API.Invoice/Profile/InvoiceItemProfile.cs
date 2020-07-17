using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using API.Invoice.DB;

namespace API.Invoice.Profile
{
    public class InvoiceItemProfile :AutoMapper.Profile
    {
        public InvoiceItemProfile()
        {
            CreateMap<invoiceitem, Models.InvoiceItem>();
        }
    }
}