using API.Invoice.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using API.Invoice.DB;
using AutoMapper;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Configuration;
using Microsoft.AspNetCore.Routing.Template;
using API.Invoice.Profile;
using System.IO;
using System.Web.Hosting;

namespace API.Invoice.Providers
{
    public class InvoicesProvider : IInvoicesProvider
    {        
        private readonly IMapper mapper;
        private readonly ILogger<InvoicesProvider> logger;

        //public InvoicesProvider(ZubairEntities dbContext, ILogger<InvoicesProvider> logger,IMapper mapper)
        //{
        //    this.dbContext = dbContext;
        //    this.logger = logger;
        //    this.mapper = mapper;
        //}
        //public InvoicesProvider(IMapper mapper)
        //{
        //    //this.logger = logger;
        //    this.mapper = mapper;
        //    this.dbContext = new ZubairEntities();
        //}
        public InvoicesProvider()
        {
            this.mapper = CreateMapper();     
        }

        private IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<InvoiceProfile>();
            });

            //config.AssertConfigurationIsValid();
            IMapper mapper = config.CreateMapper();

            return mapper;
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.InvoiceViewModel> Invoices, string ErrorMessage)> GetInvoicesAsync()
        {
            try
            {
                using (ZubairEntities dbContext = new ZubairEntities())
                {
                    var invoices = await dbContext.invoices.Include("contractor").Include("customer").Include("invoicestatu")
                               .Include("invoiceitems").Include("invoiceitems.billingitem").ToListAsync();

                    if (invoices != null && invoices.Any())
                    {
                        var result = mapper.Map<IEnumerable<invoice>, IEnumerable<Models.InvoiceViewModel>>(invoices);

                        return (true, result, null);
                    }
                }

                return (false, null, "Not Found");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> CreateInvoice(HttpFileCollection files)
        {
            try
            {
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile httpPostedFile = files[i];
                    if (httpPostedFile == null)
                    {
                        return (false, "Not Found");
                    }
                    if (httpPostedFile.FileName.ToLower().Contains("data.json"))
                    {
                        if (httpPostedFile.ContentLength > 0)
                        {
                            string str = (new StreamReader(httpPostedFile.InputStream)).ReadToEnd();

                            dynamic array = JsonConvert.DeserializeObject(str);
                            foreach (var item in array)
                            {
                                using (ZubairEntities dbContext = new ZubairEntities())
                                {
                                    if (item != null)
                                    {
                                        // invoice                                
                                        var tempInvoice = new invoice 
                                        {
                                            contractor_id = item.ContractorId,
                                            customer_id = item.CustomerID,
                                            creationdate = DateTime.Now,
                                            invoice_status_id = item.InvoiceStatusId,
                                            isactive = item.IsActive
                                        };
                                        
                                        dbContext.invoices.Add(tempInvoice);
                                        await dbContext.SaveChangesAsync();

                                        dbContext.Entry(tempInvoice).GetDatabaseValues();
                                        int invoiceID = tempInvoice.id;

                                        foreach (var it in item.InvoiceItem)
                                        {
                                            foreach (var bt in it.BillingItem)
                                            {
                                                // create billing item
                                                var tempBillingItem = new billingitem
                                                {
                                                    cost = bt.cost,
                                                    name = bt.name
                                                };
                                                
                                                dbContext.billingitems.Add(tempBillingItem);
                                                await dbContext.SaveChangesAsync();
                                                dbContext.Entry(tempBillingItem).GetDatabaseValues();
                                                int billingItemId = tempBillingItem.id;

                                                // create invoice items 
                                                var tempInvoiceItem = new invoiceitem 
                                                {
                                                    invoice_id = invoiceID,
                                                    billing_item_id = billingItemId,
                                                    discount = it.Discount,
                                                    totalcost = it.TotalCost
                                                };
                                                
                                                dbContext.invoiceitems.Add(tempInvoiceItem);
                                                await dbContext.SaveChangesAsync();
                                                dbContext.Entry(tempInvoiceItem).GetDatabaseValues();
                                                int invoiceItemId = tempInvoiceItem.id;

                                                foreach (var invFile in it.InvoiceFile)
                                                {
                                                    // create file
                                                    var tempInvoiceFile = new invoicefile
                                                    {
                                                        invoiceitem_id = invoiceItemId,
                                                        name = invFile.name
                                                    };
                                                    
                                                    var fileSavePath = Path.Combine(HostingEnvironment.MapPath(ConfigurationManager.AppSettings["fileUploadFolder"]), invFile.name);
                                                    tempInvoiceFile.filelocation = fileSavePath; 
                                                    dbContext.invoicefiles.Add(tempInvoiceFile);
                                                }
                                            }
                                        }

                                        // create logs
                                        var tempInvoicelog = new invoicelog 
                                        {
                                            invoice_id = invoiceID,
                                            invoicestatusid = item.InvoiceStatusId,
                                            datecreated = DateTime.Now
                                        };
                                        
                                        dbContext.invoicelogs.Add(tempInvoicelog);

                                        await dbContext.SaveChangesAsync();
                                        return (true, null);
                                    }
                                } // end using
                            }
                        }
                    }
                }

                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile httpPostedFile = files[i];
                    if (!httpPostedFile.FileName.ToLower().Contains("data.json"))
                    {
                        this.UploadFile(httpPostedFile);
                    }                                            
                }

               return (false, "Not Found");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, ex.Message);
            }
        }

        private void UploadFile(HttpPostedFile httpPostedFile)
        {
            var fileSavePath = Path.Combine(HostingEnvironment.MapPath(ConfigurationManager.AppSettings["fileUploadFolder"]), httpPostedFile.FileName);
            httpPostedFile.SaveAs(fileSavePath);
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateInvoice(Models.Invoice invoice)
        {
            try
            {
                using (ZubairEntities dbContext = new ZubairEntities())
                {
                    var tempInvoice = await dbContext.invoices.Where(c => c.id == invoice.Id).FirstOrDefaultAsync();

                    if (tempInvoice != null)
                    {
                        tempInvoice.contractor_id = invoice.ContractorId;
                        tempInvoice.customer_id = invoice.CustomerId;
                        tempInvoice.invoice_status_id = invoice.InvoiceStatusId;
                        tempInvoice.isactive = invoice.IsActive;
                        await dbContext.SaveChangesAsync();

                        return (true, null);
                    }
                }
                return (false, "Not Found");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteInvoice(int InvoiceID)
        {
            try
            {
                using(ZubairEntities dbContext = new ZubairEntities())
                {
                    var invoice = await dbContext.invoices.Where(c => c.id == InvoiceID).FirstOrDefaultAsync();

                    if (invoice != null)
                    {
                        invoice.isactive = false;
                        await dbContext.SaveChangesAsync();

                        return (true, null);
                    }
                }
                
                return (false, "Not Found");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, ex.Message);
            }
        }
    }
}