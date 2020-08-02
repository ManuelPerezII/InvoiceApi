using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace API.Invoice.Models
{
    public class InvoiceViewModel
    {
        [Key]
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int InvoiceStatusId { get; set; }

        public int ContractorId { get; set; }

        public DateTime CreationDate { get; set; }

        public bool IsActive { get; set; }

        public Customer Customer { get; set; }

        public Contractor Contractor { get; set; }

        public InvoiceStatus InvoiceStatu { get; set; }

        public List<InvoiceItemViewModel> InvoiceItems { get;set;}
        //public List<InvoiceItem> InvoiceItems { get; set; }
    }
}

