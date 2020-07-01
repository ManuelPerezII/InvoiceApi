using API.Invoice.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Invoice.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoicesController: ControllerBase
    {
        private readonly IInvoicesProvider invoicesProvider;
        public InvoicesController(IInvoicesProvider invoicesProvider)
        {
            this.invoicesProvider = invoicesProvider;
        }
        [HttpGet]
        public async Task<IActionResult> GetInvoicesAsync()
        {
            var result =await invoicesProvider.GetInvoicesAsync();
            if(result.IsSuccess)
            {
                return Ok(result.Invoices);
            }
            return NotFound();
        }
    }
}
