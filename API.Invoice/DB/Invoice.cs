﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Invoice.DB
{
    public class Invoice
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int InvoiceStatusId { get; set; }

        public int ContractorId { get; set; }

        public DateTime CreationDate { get; set; }

        public bool IsActive { get; set; }
    }
}
