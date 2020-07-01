using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Invoice.Interfaces
{
    public interface IInvoicesProvider
    {
        Task<(bool IsSuccess, IEnumerable<Models.Invoice> Invoices, string ErrorMessage)> GetInvoicesAsync();
    }
}
