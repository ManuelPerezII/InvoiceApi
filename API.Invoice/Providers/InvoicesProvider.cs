using API.Invoice.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using API.Invoice.DB;
using AutoMapper;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Configuration;

namespace API.Invoice.Providers
{
    public class InvoicesProvider : IInvoicesProvider
    {
        private readonly ZubairEntities dbContext;
        private readonly IMapper mapper2;
        private readonly ILogger<InvoicesProvider> logger;
        public InvoicesProvider(ZubairEntities dbContext, ILogger<InvoicesProvider> logger,IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper2 = mapper;
        }

        public Task<(bool IsSuccess, string ErrorMessage)> CreateInvoice()
        {
            throw new NotImplementedException();
        }

        public Task<(bool IsSuccess, string ErrorMessage)> DeleteInvoice(int InvoiceID)
        {
            throw new NotImplementedException();
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.InvoiceViewModel> Invoices, string ErrorMessage)> GetInvoicesAsync()
        {
            try
            {                
                var invoices = await dbContext.invoices.Include("contractor").Include("customer").Include("invoicestatu")
                               .Include("invoiceitems").Include("invoiceitems.billingitem").ToListAsync();

                if (invoices != null && invoices.Any())
                {
                    var config = new MapperConfiguration(cfg => {
                        

                        cfg.CreateMap<invoice, Models.InvoiceViewModel>()
                        .ForMember(inv=>inv.InvoiceStatusId,map=> map.MapFrom(c=> c.invoice_status_id))
                        .ForMember(inv => inv.CustomerId, map => map.MapFrom(c => c.customer_id))
                        .ForMember(inv => inv.ContractorId, map => map.MapFrom(c => c.contractor_id))
                        .ForMember(inv => inv.InvoiceItems, map => map.MapFrom(c => c.invoiceitems)).ReverseMap();                        
                        cfg.CreateMap<contractor, Models.Contractor>();
                        cfg.CreateMap<customer, Models.Customer>();
                        cfg.CreateMap<invoicestatu, Models.InvoiceStatus>();
                        //cfg.CreateMap<billingitem, Models.BillingItem>();
                        cfg.CreateMap<invoiceitem, Models.InvoiceItemViewModel>()
                        .ForMember(x => x.BillingItem, map => map.MapFrom(x => x.billingitem)).ReverseMap();
                        //.ForMember(x => x.BillingItem, o => o.Ignore());
                        cfg.CreateMap<billingitem, Models.BillingItem>();
                        //.ForMember(x => x.BillingItem, map => map.MapFrom(x => x.billingitem)).ReverseMap();
                        //cfg.CreateMap<billingitem, Models.BillingItem>();
                        //.ForAllMembers(o=> o.Ignore());
                        //cfg.CreateMap<billingitem, Models.BillingItem>();
                        //cfg.CreateMap<billingitem, Models.InvoiceItemViewModel>()
                        //.ForMember(x=> x.BillingItem,map=>map.MapFrom(x=>x));                        

                    });
                        
                    config.AssertConfigurationIsValid();
                    IMapper mapper = config.CreateMapper();
                    
                    //var result = mapper.Map<IEnumerable<Models.InvoiceViewModel>>(invoices);
                    var result = mapper.Map< IEnumerable<invoice>, IEnumerable<Models.InvoiceViewModel>>(invoices);
                    
                    return (true, result, null);
                }
                return (false, null, "Not Found");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }

        public Task<(bool IsSuccess, string ErrorMessage)> UpdateInvoice()
        {
            throw new NotImplementedException();
        }
    }
}