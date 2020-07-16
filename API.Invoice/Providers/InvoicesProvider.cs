using API.Invoice.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using API.Invoice.DB;
using AutoMapper;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Configuration;
using Microsoft.AspNetCore.Routing.Template;

namespace API.Invoice.Providers
{
    public class InvoicesProvider : IInvoicesProvider
    {
        private readonly ZubairEntities dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<InvoicesProvider> logger;
        public InvoicesProvider(ZubairEntities dbContext, ILogger<InvoicesProvider> logger,IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.InvoiceViewModel> Invoices, string ErrorMessage)> GetInvoicesAsync()
        {
            try
            {
                var invoices = await dbContext.invoices.Include("contractor").Include("customer").Include("invoicestatu")
                               .Include("invoiceitems").Include("invoiceitems.billingitem").ToListAsync();

                if (invoices != null && invoices.Any())
                {
                    var result = mapper.Map<IEnumerable<invoice>, IEnumerable<Models.InvoiceViewModel>>(invoices);

                    return (true, result, null);
                }
                return (false, null, "Not Found");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> CreateInvoice(Models.Invoice invoice)
        {
            try
            {                
                if (invoice != null)
                {
                    var tempInvoice = new invoice();
                    tempInvoice.contractor_id = invoice.ContractorId;
                    tempInvoice.customer_id = invoice.CustomerId;
                    tempInvoice.creationdate = DateTime.Now;
                    tempInvoice.invoice_status_id = invoice.InvoiceStatusId;
                    tempInvoice.isactive = invoice.IsActive;
                    dbContext.invoices.Add(tempInvoice);
                    await dbContext.SaveChangesAsync();

                    return (true, null);
                }
                return (false, "Not Found");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateInvoice(Models.Invoice invoice)
        {
            try
            {
                var tempInvoice = await dbContext.invoices.Where(c => c.id == invoice.Id).FirstOrDefaultAsync();

                if (tempInvoice != null)
                {
                    tempInvoice.contractor_id = invoice.ContractorId;
                    tempInvoice.customer_id = invoice.CustomerId;                    
                    tempInvoice.invoice_status_id = invoice.InvoiceStatusId;
                    tempInvoice.isactive = invoice.IsActive;
                    await dbContext.SaveChangesAsync();

                    return (true, null);
                }
                return (false, "Not Found");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteInvoice(int InvoiceID)
        {
            try
            {
                var invoice = await dbContext.invoices.Where(c=> c.id == InvoiceID).FirstOrDefaultAsync();
                             
                if (invoice != null)
                {                    
                    invoice.isactive = false;
                    await dbContext.SaveChangesAsync();

                    return (true, null);
                }
                return (false, "Not Found");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, ex.Message);
            }
        }
    }
}