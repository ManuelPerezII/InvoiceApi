﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace API.Invoice.Interfaces
{
    public interface IInvoiceLogsProvider
    {
        Task<(bool IsSuccess, string ErrorMessage)> CreateInvoiceLog(Models.InvoiceLog invoiceLog);

        Task<(bool IsSuccess, IEnumerable<Models.InvoiceLog> InvoiceItems, string ErrorMessage)> GetInvoiceLogsByInvoiceId(int invoiceId);
    }
}   