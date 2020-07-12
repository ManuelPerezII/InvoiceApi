using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Invoice.Models
{
    public class BillingItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Cost { get; set; }
    }
}