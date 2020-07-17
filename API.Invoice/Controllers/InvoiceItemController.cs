using API.Invoice.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace API.Invoice.Controllers
{
    [RoutePrefix("api/invoiceitem")]
    public class InvoiceItemController : ApiController
    {
        private IInvoiceItemsProvider invoiceItemsProvider;
        public InvoiceItemController(IInvoiceItemsProvider invoiceItemsProvider)
        {
            this.invoiceItemsProvider = invoiceItemsProvider;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetInvoiceItemsAsync(int id)
        {
            var result = await invoiceItemsProvider.GetInvoiceItems(id);
            if (result.IsSuccess)
            {
                return Json(result.InvoiceItems);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreateInvoiceItem(Models.InvoiceItem invoiceItem)
        {
            var result = await invoiceItemsProvider.CreateInvoiceItem(invoiceItem);
            if (result.IsSuccess)
            {
                return Ok();
            }
            return NotFound();

        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateInvoiceItem(Models.InvoiceItem invoiceItem)
        {
            var result = await invoiceItemsProvider.UpdateInvoiceItem(invoiceItem);
            if (result.IsSuccess)
            {
                return Ok();
            }
            return NotFound();
        }
    }
}