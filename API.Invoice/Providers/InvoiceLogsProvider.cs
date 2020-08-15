using API.Invoice.DB;
using API.Invoice.Interfaces;
using API.Invoice.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using API.Invoice.Profile;

namespace API.Invoice.Providers
{
    public class InvoiceLogsProvider : IInvoiceLogsProvider
    {
        private readonly IMapper mapper;
        //}
        public InvoiceLogsProvider()
        {
            this.mapper = CreateMapper();
        }

        private IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<InvoiceLogProfile>();
            });

            //config.AssertConfigurationIsValid();
            IMapper mapper = config.CreateMapper();

            return mapper;
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.InvoiceLog> InvoiceItems, string ErrorMessage)> GetInvoiceLogsByInvoiceId(int invoiceId)
        {
            try
            {
                using (ZubairEntities dbContext = new ZubairEntities())
                {
                    var invoiceLogs = await dbContext.invoicelogs.Where(c=> c.invoice_id == invoiceId).ToListAsync();

                    if (invoiceLogs != null && invoiceLogs.Any())
                    {
                        var result = mapper.Map<IEnumerable<invoicelog>, IEnumerable<Models.InvoiceLog>>(invoiceLogs);

                        return (true, result, null);
                    }
                }

                return (false, null, "Not Found");
            }
            catch (Exception ex)
            {
                //logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }
        public async Task<(bool IsSuccess, string ErrorMessage)> CreateInvoiceLog(InvoiceLog invoiceLog)
        {
            try
            {
                using (ZubairEntities dbContext = new ZubairEntities())
                {                    
                    if (invoiceLog != null)
                    {
                        var tempInvoiceItem = new invoicelog 
                        { 
                            invoice_id = invoiceLog.InvoiceId,
                            invoicestatusid = invoiceLog.InvoiceStatusID,
                            datecreated = DateTime.Now
                        };
                        
                        dbContext.invoicelogs.Add(tempInvoiceItem);
                        await dbContext.SaveChangesAsync();

                        return (true, null);
                    }
                }
                return (false, "Not Found");
            }
            catch (Exception ex)
            {
                //logger?.LogError(ex.ToString());
                return (false, ex.Message);
            }
        }
    }
}