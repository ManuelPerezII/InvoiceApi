
using API.Invoice.Interfaces;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;
using System.Web.Hosting;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System;
using System.Data.Entity;

namespace API.Invoice.Controllers
{

    [Authorize]
    [RoutePrefix("api/invoice")]
    public class InvoiceController : ApiController,IRequiresSessionState
    {
        private readonly  IInvoicesProvider invoicesProvider;     
                
        public InvoiceController(IInvoicesProvider invoicesProvider)
        {
            this.invoicesProvider = invoicesProvider;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetInvoicesAsync()
        {
            var (IsSuccess, Invoices, ErrorMessage) = await invoicesProvider.GetInvoicesAsync();
            if (IsSuccess)
            {
                return Json(Invoices);
            }
            return BadRequest(ErrorMessage);            
        }
        
        [Route("CreateInvoice")]        
        [HttpPost]
        public async Task<IHttpActionResult> CreateInvoice()
        {            
            var httpContext = System.Web.HttpContext.Current;
            
            if (httpContext.Request.Files.Count > 0)
            {
                var (IsSuccess, ErrorMessage) = await invoicesProvider.CreateInvoice(httpContext.Request.Files);
                if (IsSuccess)
                {
                    return Ok();
                }
                else
                {
                    BadRequest(ErrorMessage);
                }
            }    
            
            return BadRequest();
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateInvoice()
        {
            var httpContext = System.Web.HttpContext.Current;

            if (httpContext.Request.Files.Count > 0)
            {
                var (IsSuccess, ErrorMessage) = await invoicesProvider.UpdateInvoice(httpContext.Request.Files);
                if (IsSuccess)
                {
                    return Ok();
                }
                else
                {
                    BadRequest(ErrorMessage);
                }
            }

            return BadRequest();
        }

        [HttpPut]
        public async Task<IHttpActionResult> UpdateInvoiceStatus(int invoiceID, int statusId)
        {
            var (IsSuccess, ErrorMessage) = await invoicesProvider.UpdateInvoiceStatus(invoiceID, statusId);
            if (IsSuccess)
            {
                return Ok();
            }
            return BadRequest(ErrorMessage);
        }

        [HttpDelete]
        public async Task<IHttpActionResult> DeleteInvoice(int invoiceId)
        {
            var (IsSuccess, ErrorMessage) = await invoicesProvider.DeleteInvoice(invoiceId);
            if (IsSuccess)
            {
                return Ok();
            }
            return BadRequest(ErrorMessage); 
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