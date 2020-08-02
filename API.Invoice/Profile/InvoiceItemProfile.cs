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
            CreateMap<invoiceitem, Models.InvoiceItemViewModel>()
            .ForMember(x => x.InvoiceFiles, map => map.MapFrom(x => x.invoicefiles))
                .ForMember(x => x.BillingItem, map => map.MapFrom(x => x.billingitem)).ReverseMap();
            CreateMap<billingitem, Models.BillingItem>();
            CreateMap<invoicefile, Models.InvoiceFile>();
        }
    }
}