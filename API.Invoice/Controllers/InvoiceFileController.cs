using API.Invoice.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace API.Invoice.Controllers
{

    [RoutePrefix("api/invoicefile")]
    public class InvoiceFileController : ApiController
    {

        private readonly IInvoiceFilesProvider _invoiceFilesProvider;

        public InvoiceFileController(IInvoiceFilesProvider invoiceFilesProvider)
        {
            this._invoiceFilesProvider = invoiceFilesProvider;
        }


        [HttpGet]
        public async Task<IHttpActionResult> GetInvoiceFilesByCustomerId(Guid customerId)
        {
            var (isSuccess,invoiceFiles, errorMessage) = await _invoiceFilesProvider.GetInvoiceFilesByCustomerId(customerId);
            if (isSuccess)
            {
                return Json(invoiceFiles);
            }
            return BadRequest(errorMessage);
        }



    }
}
