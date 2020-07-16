﻿using API.Invoice.Interfaces;
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
using API.Invoice.Profile;

namespace API.Invoice.Providers
{
    public class InvoicesProvider : IInvoicesProvider
    {        
        private readonly IMapper mapper;
        private readonly ILogger<InvoicesProvider> logger;

        //public InvoicesProvider(ZubairEntities dbContext, ILogger<InvoicesProvider> logger,IMapper mapper)
        //{
        //    this.dbContext = dbContext;
        //    this.logger = logger;
        //    this.mapper = mapper;
        //}
        //public InvoicesProvider(IMapper mapper)
        //{
        //    //this.logger = logger;
        //    this.mapper = mapper;
        //    this.dbContext = new ZubairEntities();
        //}
        public InvoicesProvider()
        {
            this.mapper = CreateMapper();     
        }

        private IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<InvoiceProfile>();
            });

            //config.AssertConfigurationIsValid();
            IMapper mapper = config.CreateMapper();

            return mapper;
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.InvoiceViewModel> Invoices, string ErrorMessage)> GetInvoicesAsync()
        {
            try
            {                
                var invoices = await Connection.Instance.DbContext.invoices.Include("contractor").Include("customer").Include("invoicestatu")
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
                    Connection.Instance.DbContext.invoices.Add(tempInvoice);
                    await Connection.Instance.DbContext.SaveChangesAsync();

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
                var tempInvoice = await Connection.Instance.DbContext.invoices.Where(c => c.id == invoice.Id).FirstOrDefaultAsync();

                if (tempInvoice != null)
                {
                    tempInvoice.contractor_id = invoice.ContractorId;
                    tempInvoice.customer_id = invoice.CustomerId;                    
                    tempInvoice.invoice_status_id = invoice.InvoiceStatusId;
                    tempInvoice.isactive = invoice.IsActive;
                    await Connection.Instance.DbContext.SaveChangesAsync();

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
                var invoice = await Connection.Instance.DbContext.invoices.Where(c=> c.id == InvoiceID).FirstOrDefaultAsync();
                             
                if (invoice != null)
                {                    
                    invoice.isactive = false;
                    await Connection.Instance.DbContext.SaveChangesAsync();

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