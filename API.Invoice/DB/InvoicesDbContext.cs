using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Invoice.DB
{
    public class InvoicesDbContext: DbContext
    {
        public DbSet<Invoice> Invoices { get; set; }

        public InvoicesDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}
