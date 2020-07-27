using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace API.Invoice.Interfaces
{
    public interface IInvoicesProvider
    {
        Task<(bool IsSuccess, IEnumerable<Models.InvoiceViewModel> Invoices, string ErrorMessage)> GetInvoicesAsync();
        Task<(bool IsSuccess, string ErrorMessage)> CreateInvoice(HttpFileCollection files);        
        Task<(bool IsSuccess, string ErrorMessage)> UpdateInvoice(Models.Invoice invoice);

        Task<(bool IsSuccess, string ErrorMessage)> UpdateInvoiceStatus(int InvoiceID,int StatusID);
        Task<(bool IsSuccess, string ErrorMessage)> DeleteInvoice(int InvoiceID);        
    }
}
