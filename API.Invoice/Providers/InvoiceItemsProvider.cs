using API.Invoice.DB;
using API.Invoice.Interfaces;
using API.Invoice.Models;
using API.Invoice.Profile;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace API.Invoice.Providers
{
    public class InvoiceItemsProvider : IInvoiceItemsProvider
    {
        private readonly IMapper mapper;

        public InvoiceItemsProvider()
        {
            this.mapper = CreateMapper();
        }

        private IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<InvoiceItemProfile>();
            });

            //config.AssertConfigurationIsValid();
            IMapper mapper = config.CreateMapper();

            return mapper;
        }


        public async Task<(bool IsSuccess, IEnumerable<InvoiceItem> InvoiceItems, string ErrorMessage)> GetInvoiceItems(int id)
        {
            try
            {
                using (ZubairEntities dbContext = new ZubairEntities())
                {
                    var invoiceItems = await dbContext.invoiceitems.ToListAsync();

                    if (invoiceItems != null && invoiceItems.Any())
                    {
                        var result = mapper.Map<IEnumerable<invoiceitem>, IEnumerable<Models.InvoiceItem>>(invoiceItems);

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

        public async Task<(bool IsSuccess, string ErrorMessage)> CreateInvoiceItem(InvoiceItem invoiceItem)
        {
            try
            {
                using (ZubairEntities dbContext = new ZubairEntities())
                {
                    if (invoiceItem != null)
                    {
                        var tempInvoiceItem = new invoiceitem();
                        tempInvoiceItem.id = new Guid();
                        tempInvoiceItem.invoice_id = invoiceItem.InvoiceId;
                        tempInvoiceItem.billing_item_id = invoiceItem.BillingItemId;
                        tempInvoiceItem.discount = invoiceItem.Discount;
                        tempInvoiceItem.totalcost = invoiceItem.TotalCost;                        
                        dbContext.invoiceitems.Add(tempInvoiceItem);
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

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateInvoiceItem(InvoiceItem invoiceItem)
        {
            try
            {
                using (ZubairEntities dbContext = new ZubairEntities())
                {
                    var tempInvoice = await dbContext.invoiceitems.Where(c => c.id == invoiceItem.Id).FirstOrDefaultAsync();

                    if (tempInvoice != null)
                    {
                        var tempInvoiceItem = new invoiceitem();
                        tempInvoiceItem.invoice_id = invoiceItem.InvoiceId;
                        tempInvoiceItem.billing_item_id = invoiceItem.BillingItemId;
                        tempInvoiceItem.discount = invoiceItem.Discount;
                        tempInvoiceItem.totalcost = invoiceItem.TotalCost;
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

        public async Task<(bool IsSuccess, IEnumerable<InvoiceItemViewModel> InvoiceItems, string ErrorMessage)> GetInvoiceItemByBillingID(int billingItemId)
        {
            try
            {
                using (ZubairEntities dbContext = new ZubairEntities())
                {
                    var invoiceItems = await dbContext.invoiceitems.Include("billingitem").Include("invoicefiles")
                        .Where(c=>c.billing_item_id ==billingItemId).ToListAsync();

                    if (invoiceItems != null && invoiceItems.Any())
                    {
                        var result = mapper.Map<IEnumerable<invoiceitem>, IEnumerable<Models.InvoiceItemViewModel>>(invoiceItems);

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
    }
}