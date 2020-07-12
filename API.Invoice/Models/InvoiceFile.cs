using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Invoice.Models
{
    public class InvoiceFile
    {
        public int Id { get; set; }

        public int InvoiceItemId { get; set; }

        public string Name { get; set; }

        public string FileLocation { get; set; }
    }
}