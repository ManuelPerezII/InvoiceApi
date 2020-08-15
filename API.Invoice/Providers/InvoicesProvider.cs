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
        //private readonly ILogger<InvoicesProvider> logger;

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
                               .Include("invoiceitems").Include("invoiceitems.billingitem").Include("invoiceitems.invoicefiles").ToListAsync();

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
                //logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.InvoiceViewModel> Invoices, string ErrorMessage)> GetInvoicesByCustomerId(Guid customerId)
        {
            try
            {
                using (ZubairEntities dbContext = new ZubairEntities())
                {
                    var invoices = await dbContext.invoices.Where(c => c.customer_id == customerId).Include("contractor")
                        .Include("customer").Include("invoicestatu").Include("invoiceitems").Include("invoiceitems.billingitem")
                        .Include("invoiceitems.invoicefiles").ToListAsync();

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
                //logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.InvoiceViewModel> Invoices, string ErrorMessage)> GetInvoicesByContractorId(Guid contractorId)
        {
            try
            {
                using (ZubairEntities dbContext = new ZubairEntities())
                {
                    var invoices = await dbContext.invoices.Where(c=> c.contractor_id == contractorId).Include("contractor")
                        .Include("customer").Include("invoicestatu").Include("invoiceitems").Include("invoiceitems.billingitem")
                        .Include("invoiceitems.invoicefiles").ToListAsync();

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
                //logger?.LogError(ex.ToString());
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
                            dynamic array = JsonConvert.DeserializeObject((new StreamReader(httpPostedFile.InputStream)).ReadToEnd());
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

                                                await SaveInvoiceSubData(dbContext, item, invoiceID, tempInvoice);

                                                #region "Commented"
                                                //foreach (var it in item.InvoiceItem)
                                                //{
                                                //    foreach (var bt in it.BillingItem)
                                                //    {
                                                //        // create billing item
                                                //        var tempBillingItem = new billingitem
                                                //        {
                                                //            cost = bt.cost,
                                                //            name = bt.name
                                                //        };

                                                //        dbContext.billingitems.Add(tempBillingItem);
                                                //        await dbContext.SaveChangesAsync();
                                                //        dbContext.Entry(tempBillingItem).GetDatabaseValues();
                                                //        int billingItemId = tempBillingItem.id;

                                                //        // create invoice items 
                                                //        var tempInvoiceItem = new invoiceitem
                                                //        {
                                                //            invoice_id = invoiceID,
                                                //            billing_item_id = billingItemId,
                                                //            discount = it.Discount,
                                                //            totalcost = it.TotalCost
                                                //        };

                                                //        dbContext.invoiceitems.Add(tempInvoiceItem);
                                                //        await dbContext.SaveChangesAsync();
                                                //        dbContext.Entry(tempInvoiceItem).GetDatabaseValues();
                                                //        int invoiceItemId = tempInvoiceItem.id;

                                                //        foreach (var invFile in it.InvoiceFile)
                                                //        {
                                                //            // create file
                                                //            var tempInvoiceFile = new invoicefile
                                                //            {
                                                //                invoiceitem_id = invoiceItemId,
                                                //                name = invFile.name
                                                //            };

                                                //            tempInvoiceFile.filelocation = GetFilePath(tempInvoiceFile.name);
                                                //            dbContext.invoicefiles.Add(tempInvoiceFile);                                                            
                                                //        }
                                                //        await dbContext.SaveChangesAsync();
                                                //    }
                                                //}

                                                //InsertInvoiceLog(dbContext, invoiceID, tempInvoice.invoice_status_id);

                                                //await dbContext.SaveChangesAsync();
                                                #endregion
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
                //logger?.LogError(ex.ToString());
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
                            dynamic array = JsonConvert.DeserializeObject((new StreamReader(httpPostedFile.InputStream)).ReadToEnd());
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
                                                int invoiceID = item.id;
                                                var invoice = await dbContext.invoices.Where(c => c.id == invoiceID).FirstOrDefaultAsync();
                                                // invoice                                
                                                if (invoice != null)
                                                {
                                                    invoice.contractor_id = item.ContractorId;
                                                    invoice.customer_id = item.CustomerID;
                                                    invoice.invoice_status_id = item.InvoiceStatusId;
                                                    invoice.isactive = item.IsActive;
                                                    await dbContext.SaveChangesAsync();

                                                    await SaveInvoiceSubData(dbContext, item, invoiceID, invoice);
                                                }
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
                //logger?.LogError(ex.ToString());
                return (false, ex.Message);
            }
        }

        private async Task SaveInvoiceSubData(ZubairEntities dbContext, dynamic item, int invoiceID, invoice invoice)
        {
            try
            {
                foreach (var it in item.InvoiceItem)
                {
                    foreach (var bt in it.BillingItem)
                    {
                        int billingItemId = await SaveBillingItem(dbContext, bt);

                        Guid invoiceItemId = Guid.Empty;
                        var invoiceItem = await dbContext.invoiceitems.Where(c => c.billing_item_id == billingItemId).FirstOrDefaultAsync();
                        invoiceItemId = await SaveInvoiceItem(dbContext, invoiceID, it, billingItemId, invoiceItemId, invoiceItem);

                        foreach (var invFile in it.InvoiceFile)
                        {
                            await SaveInvoiceFile(dbContext, invoiceItemId, invFile);
                        }
                    }
                }

                InsertInvoiceLog(dbContext, invoiceID, invoice.invoice_status_id);
                await dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        private async Task SaveInvoiceFile(ZubairEntities dbContext, Guid invoiceItemId, dynamic invFile)
        {
            int? invoiceFileId = invFile.id;

            if(invoiceFileId !=null)
            {
                var invoiceFile = await dbContext.invoicefiles.Where(c => c.id == invoiceFileId).FirstOrDefaultAsync();
                // create file
                if (invoiceFile != null)
                {
                    invoiceFile.invoice_item_id = invFile.invoiceItemId;
                    invoiceFile.name = invFile.name;

                    invoiceFile.filelocation = GetFilePath(invoiceFile.name);
                }
            }            
            else
            {
                var tempInvoiceFile = new invoicefile
                {
                    invoice_item_id = invoiceItemId,
                    name = invFile.name
                };

                tempInvoiceFile.filelocation = GetFilePath(tempInvoiceFile.name);
                dbContext.invoicefiles.Add(tempInvoiceFile);
            }
            await dbContext.SaveChangesAsync();
        }

        private static async Task<int?> SaveBillingItem(ZubairEntities dbContext, dynamic bt)
        {
            int? billingItemId = bt.id;            
            if(billingItemId !=null)
            {
                var billingItem = await dbContext.billingitems.Where(c => c.id == billingItemId).FirstOrDefaultAsync();
                // update billing item
                if (billingItem != null)
                {
                    billingItem.cost = bt.cost;
                    billingItem.name = bt.name;
                    await dbContext.SaveChangesAsync();
                }
            }            
            else
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
                billingItemId = tempBillingItem.id;
            }

            return billingItemId;
        }

        private static async Task<Guid> SaveInvoiceItem(ZubairEntities dbContext, int invoiceID, dynamic it, int billingItemId, Guid invoiceItemId, invoiceitem invoiceItem)
        {
            // update invoice items 
            if (invoiceItem != null)
            {
                invoiceItem.invoice_id = invoiceID;
                invoiceItem.billing_item_id = billingItemId;
                invoiceItem.discount = it.Discount;
                invoiceItem.totalcost = it.TotalCost;
                invoiceItem.taxes = it.Taxes;
                invoiceItemId = invoiceItem.id;
                await dbContext.SaveChangesAsync();
            }
            else
            {
                // create invoice items 
                var tempInvoiceItem = new invoiceitem
                {
                    id = Guid.NewGuid(),
                    invoice_id = invoiceID,
                    billing_item_id = billingItemId,
                    discount = it.Discount,
                    totalcost = it.TotalCost
                };

                dbContext.invoiceitems.Add(tempInvoiceItem);
                await dbContext.SaveChangesAsync();
                dbContext.Entry(tempInvoiceItem).GetDatabaseValues();
                invoiceItemId = tempInvoiceItem.id;
            }

            return invoiceItemId;
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
                //logger?.LogError(ex.ToString());
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

        public string GetFilePath(string fileName)
        {
            return Path.Combine(HostingEnvironment.MapPath(ConfigurationManager.AppSettings["fileUploadFolder"]), fileName);
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
                //logger?.LogError(ex.ToString());
                return (false, ex.Message);
            }
        }
    }
}