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
    public class InvoiceFilesProvider : IInvoiceFilesProvider
    {
        public async Task<(bool IsSuccess, string ErrorMessage)> CreateInvoiceFile(InvoiceFile invoiceFile)
        {
            try
            {
                using (ZubairEntities dbContext = new ZubairEntities())
                {
                    if (invoiceFile != null)
                    {
                        var tempInvoiceFile = new invoicefile();
                        tempInvoiceFile.invoice_item_id = invoiceFile.InvoiceItemId;
                        tempInvoiceFile.name = invoiceFile.Name;
                        tempInvoiceFile.filelocation = invoiceFile.FileLocation;
                        dbContext.invoicefiles.Add(tempInvoiceFile);
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