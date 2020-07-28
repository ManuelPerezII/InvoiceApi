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
            bool IsSuccessSaving = false;
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
                            using (ZubairEntities dbContext = new ZubairEntities())
                            {
                                using (DbContextTransaction transaction = dbContext.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        foreach (var item in array)
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

                                                            var fileSavePath = Path.Combine(HostingEnvironment.MapPath(ConfigurationManager.AppSettings["fileUploadFolder"]), tempInvoiceFile.name);
                                                            tempInvoiceFile.filelocation = fileSavePath;
                                                            dbContext.invoicefiles.Add(tempInvoiceFile);                                                            
                                                        }
                                                        await dbContext.SaveChangesAsync();
                                                    }
                                                }

                                                InsertInvoiceLog(dbContext, invoiceID, tempInvoice.invoice_status_id);

                                                await dbContext.SaveChangesAsync();
                                                
                                            }
                                        }

                                        IsSuccessSaving = true;
                                        transaction.Commit();
                                    }
                                    catch (Exception ex)
                                    {
                                        transaction.Rollback();
                                        throw ex;
                                    }
                                }
                                
                            } // end using                            
                        }
                    }
                }

                if(IsSuccessSaving)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFile httpPostedFile = files[i];
                        if (!httpPostedFile.FileName.ToLower().Contains("data.json"))
                        {
                            this.UploadFile(httpPostedFile);
                        }
                    }
                    return (true, null);
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

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateInvoice(HttpFileCollection files)
        {
            bool IsSuccessSaving = false;
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
                            using (ZubairEntities dbContext = new ZubairEntities())
                            {                                
                                using (DbContextTransaction transaction = dbContext.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        foreach (var item in array)
                                        {
                                            if (item != null)
                                            {
                                                int invoiceID = item.InvoiceID;
                                                var invoice = await dbContext.invoices.Where(c => c.id == invoiceID).FirstOrDefaultAsync();
                                                // invoice                                
                                                if(invoice != null)
                                                {
                                                    invoice.contractor_id = item.ContractorId;
                                                    invoice.customer_id = item.CustomerID;
                                                    invoice.invoice_status_id = item.InvoiceStatusId;
                                                    invoice.isactive = item.IsActive;
                                                    await dbContext.SaveChangesAsync();
                                                }
                                                                                                
                                                foreach (var it in item.InvoiceItem)
                                                {
                                                    foreach (var bt in it.BillingItem)
                                                    {
                                                        int billingItemId = bt.id;
                                                        var billingItem = await dbContext.billingitems.Where(c => c.id == billingItemId).FirstOrDefaultAsync();
                                                        // update billing item
                                                        if(billingItem != null)
                                                        {
                                                            billingItem.cost = bt.cost;
                                                            billingItem.name = bt.name;

                                                            await dbContext.SaveChangesAsync();
                                                        };

                                                        int invoiceItemId = it.id;
                                                        var invoiceItem = await dbContext.invoiceitems.Where(c => c.id == invoiceItemId).FirstOrDefaultAsync();

                                                        // update invoice items 
                                                        if(invoiceItem !=null)
                                                        {
                                                            invoiceItem.invoice_id = invoiceID;
                                                            invoiceItem.billing_item_id = billingItemId;
                                                            invoiceItem.discount = it.Discount;
                                                            invoiceItem.totalcost = it.TotalCost;

                                                            await dbContext.SaveChangesAsync();
                                                        };
                                                        

                                                        foreach (var invFile in it.InvoiceFile)
                                                        {
                                                            int invoiceFileId = invFile.id;

                                                            var invoiceFile = await dbContext.invoicefiles.Where(c => c.id == invoiceFileId).FirstOrDefaultAsync();
                                                            // create file
                                                            if(invoiceFile !=null)
                                                            {
                                                                invoiceFile.invoiceitem_id = invFile.invoiceItemId;
                                                                invoiceFile.name = invFile.name;

                                                                var fileSavePath = Path.Combine(HostingEnvironment.MapPath(ConfigurationManager.AppSettings["fileUploadFolder"]), invoiceFile.name);
                                                                invoiceFile.filelocation = fileSavePath;
                                                                await dbContext.SaveChangesAsync();
                                                            };                                                            
                                                        }                                                        
                                                    }
                                                }

                                                InsertInvoiceLog(dbContext, invoiceID, invoice.invoice_status_id);
                                                await dbContext.SaveChangesAsync();
                                            }
                                        }

                                        IsSuccessSaving = true;
                                        transaction.Commit();
                                    }
                                    catch (Exception ex)
                                    {
                                        transaction.Rollback();
                                        throw ex;
                                    }
                                }

                            } // end using                            
                        }
                    }
                }

                if (IsSuccessSaving)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFile httpPostedFile = files[i];
                        if (!httpPostedFile.FileName.ToLower().Contains("data.json"))
                        {
                            this.UploadFile(httpPostedFile);
                        }
                    }
                    return (true, null);
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


        private static void InsertInvoiceLog(ZubairEntities dbContext, int invoiceID, int? invoiceStatusId)
        {
            // create logs
            var tempInvoicelog = new invoicelog
            {
                invoice_id = invoiceID,
                invoicestatusid = invoiceStatusId,
                datecreated = DateTime.Now
            };

            dbContext.invoicelogs.Add(tempInvoicelog);
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateInvoiceStatus(int InvoiceID, int StatusID)
        {
            try
            {
                using (ZubairEntities dbContext = new ZubairEntities())
                {
                    var invoice = await dbContext.invoices.Where(c => c.id == InvoiceID).FirstOrDefaultAsync();

                    if (invoice != null)
                    {
                        invoice.invoice_status_id = StatusID;
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