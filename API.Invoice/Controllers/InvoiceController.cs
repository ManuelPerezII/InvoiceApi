
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

namespace API.Invoice.Controllers
{
    
    [RoutePrefix("api/invoice")]
    public class InvoiceController : ApiController,IRequiresSessionState
    {
        private  IInvoicesProvider invoicesProvider;     
                
        public InvoiceController(IInvoicesProvider invoicesProvider)
        {
            this.invoicesProvider = invoicesProvider;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetInvoicesAsync()
        {
            var result = await invoicesProvider.GetInvoicesAsync();
            if (result.IsSuccess)
            {
                return Json(result.Invoices);
            }
            return NotFound();            
        }
        
        [Route("CreateInvoice")]        
        [HttpPost]
        public async Task<IHttpActionResult> CreateInvoice()
        {            
            var httpContext = System.Web.HttpContext.Current;
            
            if (httpContext.Request.Files.Count > 0)
            {            
                for (int i = 0; i < httpContext.Request.Files.Count; i++)
                {                   
                    HttpPostedFile httpPostedFile = httpContext.Request.Files[i];
                    if (httpPostedFile != null)
                    {
                        if(httpPostedFile.FileName.ToLower().Contains("data.json"))
                        {
                            var result = await invoicesProvider.CreateInvoice(httpPostedFile);
                            if (result.IsSuccess)
                            {
                                return Ok();
                            }                            
                        }
                        else
                        {                            
                            var fileSavePath = Path.Combine(HostingEnvironment.MapPath(ConfigurationManager.AppSettings["fileUploadFolder"]), httpPostedFile.FileName);                            
                            httpPostedFile.SaveAs(fileSavePath);
                        }                        
                    }
                }
            }                        
            return NotFound();
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateInvoice(Models.Invoice invoice)
        {
            var result = await invoicesProvider.UpdateInvoice(invoice);
            if (result.IsSuccess)
            {
                return Ok();
            }
            return NotFound();
        }

        [HttpDelete]
        public async Task<IHttpActionResult> DeleteInvoice(int invoiceId)
        {
            var result = await invoicesProvider.DeleteInvoice(invoiceId);
            if (result.IsSuccess)
            {
                return Ok();
            }
            return NotFound();
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