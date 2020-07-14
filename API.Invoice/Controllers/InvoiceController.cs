
using API.Invoice.DB;
using API.Invoice.Interfaces;
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
               
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap <invoice, Models.Invoice > ();
            });
            IMapper mapper = config.CreateMapper();

            using (ZubairEntities dbContext = new ZubairEntities())
            {
                invoicesProvider = new InvoicesProvider(dbContext, null, mapper);
                var result = await invoicesProvider.GetInvoicesAsync();
                if (result.IsSuccess)
                {
                    return Ok(result.Invoices);
                }
                return NotFound();
            }            
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