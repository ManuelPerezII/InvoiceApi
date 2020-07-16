
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
using System.Web.SessionState;

namespace API.Invoice.Controllers
{
    
    [RoutePrefix("api/invoice")]
    public class InvoiceController : ApiController,IRequiresSessionState
    {
        private  IInvoicesProvider invoicesProvider;     
                
        public InvoiceController()
        {            
        }

        public InvoiceController(IInvoicesProvider invoicesProvider)
        {
            this.invoicesProvider = invoicesProvider;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetInvoicesAsync()
        {         
            using (ZubairEntities dbContext = new ZubairEntities())
            {
                invoicesProvider = new InvoicesProvider(dbContext, null, CreateMapper());
                var result = await invoicesProvider.GetInvoicesAsync();
                if (result.IsSuccess)
                {
                    return Json(result.Invoices);
                }
                return NotFound();
            }            
        }

        private IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<InvoiceProfile>();                
            });

            config.AssertConfigurationIsValid();
            IMapper mapper = config.CreateMapper();

            return mapper;
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreateInvoice(Models.Invoice invoice)
        {
            using (ZubairEntities dbContext = new ZubairEntities())
            {
                invoicesProvider = new InvoicesProvider(dbContext, null, null);
                var result = await invoicesProvider.CreateInvoice(invoice);
                if (result.IsSuccess)
                {
                    return Ok();
                }
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateInvoice(Models.Invoice invoice)
        {
            using (ZubairEntities dbContext = new ZubairEntities())
            {
                invoicesProvider = new InvoicesProvider(dbContext, null, null);
                var result = await invoicesProvider.UpdateInvoice(invoice);
                if (result.IsSuccess)
                {
                    return Ok();
                }
                return NotFound();
            }
        }

        [HttpDelete]
        public async Task<IHttpActionResult> DeleteInvoice(int invoiceId)
        {
            using (ZubairEntities dbContext = new ZubairEntities())
            {
                invoicesProvider = new InvoicesProvider(dbContext, null, CreateMapper());
                var result = await invoicesProvider.DeleteInvoice(invoiceId);
                if (result.IsSuccess)
                {
                    return Ok();
                }
                return NotFound();
            }
        }

        #region Commented
        //[HttpPost]
        //public IHttpActionResult Post()
        //{
        //    return Ok("Post");
        //}

        //public IHttpActionResult Put()
        //{            
        //    return Ok("Put");
        //}

        //[HttpPatch]
        //public IHttpActionResult Patch()
        //{
        //    return Ok("Patch");
        //}
        #endregion
    }
}