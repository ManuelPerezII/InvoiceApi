using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace API.Invoice.Interfaces
{
    public interface IInvoiceFilesProvider
    {
        Task<(bool IsSuccess, string ErrorMessage)> CreateInvoiceFile(Models.InvoiceFile invoiceFile);
    }
}