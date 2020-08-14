using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Invoice.Models
{
    public class InvoiceItem
    {
        public Guid Id { get; set; }

        public int InvoiceId { get; set; }

        public int BillingItemId { get; set; }

        public decimal Discount { get; set; }

        public decimal TotalCost { get; set; }
    }
}