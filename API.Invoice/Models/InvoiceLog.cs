using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Invoice.Models
{
    public class InvoiceLog
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }

        public int InvoiceStatusID { get; set; }

        public DateTime DateCreated { get; set; }

    }
}