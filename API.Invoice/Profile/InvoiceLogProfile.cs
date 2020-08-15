using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using API.Invoice.DB;

namespace API.Invoice.Profile
{
    public class InvoiceLogProfile : AutoMapper.Profile
    {
        public InvoiceLogProfile()
        {
            CreateMap<invoicelog, Models.InvoiceLog>();
        }
    }
}