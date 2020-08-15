using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using API.Invoice.DB;

namespace API.Invoice.Profile
{
    public class InvoiceFileProfile : AutoMapper.Profile
    {
        public InvoiceFileProfile()
        {
            CreateMap<invoicefile, Models.InvoiceFile>();
        }
    }
}