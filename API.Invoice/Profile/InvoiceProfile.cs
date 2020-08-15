using API.Invoice.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Invoice.Profile
{
    public class InvoiceProfile : AutoMapper.Profile
    {
        public InvoiceProfile()
        {
            CreateMap<invoice, Models.InvoiceViewModel>()
                .ForMember(inv => inv.InvoiceStatusId, map => map.MapFrom(c => c.invoice_status_id))
                .ForMember(inv => inv.CustomerId, map => map.MapFrom(c => c.customer_id))
                .ForMember(inv => inv.ContractorId, map => map.MapFrom(c => c.contractor_id))                
                .ForMember(inv => inv.InvoiceItems, map => map.MapFrom(c => c.invoiceitems)).ReverseMap();
            CreateMap<contractor, Models.Contractor>();
            CreateMap<customer, Models.Customer>();
            CreateMap<invoicestatu, Models.InvoiceStatus>();
            CreateMap<invoiceitem, Models.InvoiceItemViewModel>()
                .ForMember(x => x.InvoiceFiles,map=> map.MapFrom(x=>x.invoicefiles))
                .ForMember(x => x.BillingItem, map => map.MapFrom(x => x.billingitem)).ReverseMap();
            CreateMap<billingitem, Models.BillingItem>();
            CreateMap<invoicefile, Models.InvoiceFile>();
        }
    }
}