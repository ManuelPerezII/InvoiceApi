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

        Task<(bool IsSuccess, IEnumerable<Models.InvoiceViewModel> Invoices, string ErrorMessage)> GetInvoicesByCustomerId(Guid customerId);

        Task<(bool IsSuccess, IEnumerable<Models.InvoiceViewModel> Invoices, string ErrorMessage)> GetInvoicesByContractorId(Guid contractorId);

        Task<(bool IsSuccess, string ErrorMessage)> CreateInvoice(HttpFileCollection files);        
        Task<(bool IsSuccess, string ErrorMessage)> UpdateInvoice(HttpFileCollection files);

        Task<(bool IsSuccess, string ErrorMessage)> UpdateInvoiceStatus(int InvoiceID,int StatusID);
        Task<(bool IsSuccess, string ErrorMessage)> DeleteInvoice(int InvoiceID);        
    }
}
