using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using API.Invoice.Interfaces;

namespace API.Invoice.Controllers
{

    [RoutePrefix("api/invoicelog")]

    public class InvoiceLogController : ApiController
    {
        private readonly IInvoiceLogsProvider _invoiceLogsProvider;

        public InvoiceLogController(IInvoiceLogsProvider invoiceLogsProvider)
        {
            this._invoiceLogsProvider = invoiceLogsProvider;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetInvoiceFilesByCustomerId(int invoiceId)
        {
            var (isSuccess, invoiceLogs, errorMessage) = await _invoiceLogsProvider.GetInvoiceLogsByInvoiceId(invoiceId);
            if (isSuccess)
            {
                return Json(invoiceLogs);
            }
            return BadRequest(errorMessage);
        }

    }
}
