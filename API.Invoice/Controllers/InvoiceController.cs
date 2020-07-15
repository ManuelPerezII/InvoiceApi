
using API.Invoice.DB;
using API.Invoice.Interfaces;
using API.Invoice.Profile;
using API.Invoice.Providers;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace API.Invoice.Controllers
{
    
    [RoutePrefix("api/invoice")]
    public class InvoiceController : ApiController
    {
        private  IInvoicesProvider invoicesProvider;
        //private  ZubairEntities dbContext;
                
        public InvoiceController()
        {            
        }

        //public InvoiceController(IInvoicesProvider invoicesProvider)
        //{
        //  //  this.invoicesProvider = invoicesProvider;
        //}

        [HttpGet]
        public async Task<IHttpActionResult> GetInvoicesAsync()
        {         
            using (ZubairEntities dbContext = new ZubairEntities())
            {
                invoicesProvider = new InvoicesProvider(dbContext, null, CreateMapper());
                var result = await invoicesProvider.GetInvoicesAsync();
                if (result.IsSuccess)
                {
                    return Ok(result.Invoices);
                }
                return NotFound();
            }            
        }

        private IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<InvoiceProfile>();
                //cfg.CreateMap<invoice, Models.InvoiceViewModel>()
                //.ForMember(inv => inv.InvoiceStatusId, map => map.MapFrom(c => c.invoice_status_id))
                //.ForMember(inv => inv.CustomerId, map => map.MapFrom(c => c.customer_id))
                //.ForMember(inv => inv.ContractorId, map => map.MapFrom(c => c.contractor_id))
                //.ForMember(inv => inv.InvoiceItems, map => map.MapFrom(c => c.invoiceitems)).ReverseMap();
                //cfg.CreateMap<contractor, Models.Contractor>();
                //cfg.CreateMap<customer, Models.Customer>();
                //cfg.CreateMap<invoicestatu, Models.InvoiceStatus>();
                //cfg.CreateMap<invoiceitem, Models.InvoiceItemViewModel>()
                //.ForMember(x => x.BillingItem, map => map.MapFrom(x => x.billingitem)).ReverseMap();
                //cfg.CreateMap<billingitem, Models.BillingItem>();
            });

            config.AssertConfigurationIsValid();
            IMapper mapper = config.CreateMapper();

            return mapper;
        }
        

        [HttpPost]
        public IHttpActionResult Post()
        {
            return Ok("Post");
        }

        public IHttpActionResult Put()
        {            
            return Ok("Put");
        }

        public IHttpActionResult Patch()
        {
            return Ok("Patch");
        }
        public IHttpActionResult Delete()
        {
            return Ok("Delete");
        }
    }
}