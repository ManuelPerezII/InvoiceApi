using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;

namespace API.Invoice.Profile
{
    public static class AutomapperConfig
    {
        public static MapperConfiguration RegisterMappings()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new InvoiceProfile());                
            });
        }
    }
}