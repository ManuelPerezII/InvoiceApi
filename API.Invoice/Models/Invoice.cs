using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Invoice.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        public Guid CustomerId { get; set; }

        public int InvoiceStatusId { get; set; }

        public Guid ContractorId { get; set; }

        public DateTime CreationDate { get; set; }

        public bool IsActive { get; set; }

        public Guid Work_Id { get; set; }
    }
}