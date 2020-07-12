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

        public Task<(bool IsSuccess, string ErrorMessage)> CreateInvoice()
        {
            throw new NotImplementedException();
        }

        public Task<(bool IsSuccess, string ErrorMessage)> DeleteInvoice(int InvoiceID)
        {
            throw new NotImplementedException();
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Invoice> Invoices, string ErrorMessage)> GetInvoicesAsync()
        {
            try
            {             
                var invoices = await dbContext.invoices.ToListAsync();
                if (invoices != null && invoices.Any())
                {
                    var result = mapper.Map<IEnumerable<DB.invoice>, IEnumerable<Models.Invoice>>(invoices);

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

        public Task<(bool IsSuccess, string ErrorMessage)> UpdateInvoice()
        {
            throw new NotImplementedException();
        }
    }
}