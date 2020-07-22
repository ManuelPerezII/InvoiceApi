using API.Invoice.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Invoice.Profile
{
    public class AuthorizedAppProfile : AutoMapper.Profile
    {
        public AuthorizedAppProfile()
        {
            CreateMap<authorizedapp, Models.AuthorizedApp>();
        }
    }
}