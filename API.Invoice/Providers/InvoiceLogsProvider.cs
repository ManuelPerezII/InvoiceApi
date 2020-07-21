using API.Invoice.DB;
using API.Invoice.Interfaces;
using API.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace API.Invoice.Providers
{
    public class InvoiceLogsProvider : IInvoiceLogsProvider
    {
        public async Task<(bool IsSuccess, string ErrorMessage)> CreateInvoiceLog(InvoiceLog invoiceLog)
        {
            try
            {
                using (ZubairEntities dbContext = new ZubairEntities())
                {
                    if (invoiceLog != null)
                    {
                        var tempInvoiceItem = new invoicelog();
                        tempInvoiceItem.invoice_id = invoiceLog.InvoiceId;
                        tempInvoiceItem.invoicestatusid = invoiceLog.InvoiceStatusID;
                        tempInvoiceItem.datecreated = DateTime.Now;                        
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