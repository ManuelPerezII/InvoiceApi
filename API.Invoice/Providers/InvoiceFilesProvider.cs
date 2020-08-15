using API.Invoice.DB;
using API.Invoice.Interfaces;
using API.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using API.Invoice.Profile;
using AutoMapper;

namespace API.Invoice.Providers
{
    public class InvoiceFilesProvider : IInvoiceFilesProvider
    {
        private readonly IMapper mapper;
        //}
        public InvoiceFilesProvider()
        {
            this.mapper = CreateMapper();
        }

        private IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<InvoiceFileProfile>();
            });

            //config.AssertConfigurationIsValid();
            IMapper mapper = config.CreateMapper();

            return mapper;
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.InvoiceFile> InvoiceItems, string ErrorMessage)> GetInvoiceFilesByCustomerId(Guid customerId)
        {
            try
            {
                using (ZubairEntities dbContext = new ZubairEntities())
                {
                    var invoiceFiles = await dbContext.invoicefiles.ToListAsync();

                    if (invoiceFiles != null && invoiceFiles.Any())
                    {
                        var result = mapper.Map<IEnumerable<invoicefile>, IEnumerable<Models.InvoiceFile>>(invoiceFiles);

                        return (true, result, null);
                    }
                }

                return (false, null, "Not Found");
            }
            catch (Exception ex)
            {
                //logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }


        public async Task<(bool IsSuccess, string ErrorMessage)> CreateInvoiceFile(InvoiceFile invoiceFile)
        {
            try
            {
                using (ZubairEntities dbContext = new ZubairEntities())
                {
                    if (invoiceFile != null)
                    {
                        var tempInvoiceFile = new invoicefile();
                        tempInvoiceFile.invoice_item_id = invoiceFile.InvoiceItemId;
                        tempInvoiceFile.name = invoiceFile.Name;
                        tempInvoiceFile.filelocation = invoiceFile.FileLocation;
                        dbContext.invoicefiles.Add(tempInvoiceFile);
                        await dbContext.SaveChangesAsync();

                        return (true, null);
                    }
                }
                return (false, "Not Found");
            }
            catch (Exception ex)
            {
                //logger?.LogError(ex.ToString());
                return (false, ex.Message);
            }
        }
    }
}