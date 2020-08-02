using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace API.Invoice.Interfaces
{
    public interface IInvoiceItemsProvider
    {
        Task<(bool IsSuccess, IEnumerable<Models.InvoiceItem> InvoiceItems, string ErrorMessage)> GetInvoiceItems(int id);

        Task<(bool IsSuccess, string ErrorMessage)> CreateInvoiceItem(Models.InvoiceItem invoiceItem);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateInvoiceItem(Models.InvoiceItem invoiceItem);

        Task<(bool IsSuccess, IEnumerable<Models.InvoiceItemViewModel> InvoiceItems, string ErrorMessage)> GetInvoiceItemByBillingID(int billingItemId);
    }
}