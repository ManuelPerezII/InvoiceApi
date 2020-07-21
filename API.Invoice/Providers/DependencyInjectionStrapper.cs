using API.Invoice.Interfaces;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Invoice.Providers
{
    public class DependencyInjectionStrapper
    {
        public static void AddRegistrationsToContainer(Container container)
        {
            container.Register<IInvoicesProvider, InvoicesProvider>(Lifestyle.Singleton);
            container.Register<IInvoiceItemsProvider, InvoiceItemsProvider>(Lifestyle.Singleton);
            container.Register<IInvoiceLogsProvider, InvoiceLogsProvider>(Lifestyle.Singleton);
            container.Register<IInvoiceFilesProvider, InvoiceFilesProvider>(Lifestyle.Singleton);
        }
    }
}