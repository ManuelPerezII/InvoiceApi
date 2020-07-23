﻿using API.Invoice.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace API.Invoice.Controllers
{
    [RoutePrefix("api/invoiceitem")]
    public class InvoiceItemController : ApiController
    {
        private readonly IInvoiceItemsProvider invoiceItemsProvider;
        public InvoiceItemController(IInvoiceItemsProvider invoiceItemsProvider)
        {
            this.invoiceItemsProvider = invoiceItemsProvider;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetInvoiceItemsAsync(int id)
        {
            var (IsSuccess,InvoiceItems,ErrorMessage)  = await invoiceItemsProvider.GetInvoiceItems(id);
            if (IsSuccess)
            {
                return Json(InvoiceItems);
            }
            return BadRequest(ErrorMessage);
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreateInvoiceItem(Models.InvoiceItem invoiceItem)
        {
            var (IsSuccess, ErrorMessage) = await invoiceItemsProvider.CreateInvoiceItem(invoiceItem);
            if (IsSuccess)
            {
                return Ok();
            }
            return BadRequest(ErrorMessage);

        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateInvoiceItem(Models.InvoiceItem invoiceItem)
        {
            var (IsSuccess, ErrorMessage) = await invoiceItemsProvider.UpdateInvoiceItem(invoiceItem);
            if (IsSuccess)
            {
                return Ok();
            }
            return BadRequest(ErrorMessage);
        }        
    }
}