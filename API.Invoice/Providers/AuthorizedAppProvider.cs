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
    public class AuthorizedAppProvider : IAuthorizedAppProvider
    {
        private readonly IMapper mapper;

        public AuthorizedAppProvider()
        {
            this.mapper = CreateMapper();
        }

        private IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<AuthorizedAppProfile>();
            });
            
            IMapper mapper = config.CreateMapper();

            return mapper;
        }

        public async Task<(bool IsSuccess, AuthorizedApp AuthorizedApp, string ErrorMessage)> GetAuth(string token, string password)
        {
            try
            {
                using (ZubairEntities dbContext = new ZubairEntities())
                {
                    var authorizedApp = await dbContext.authorizedapps
                                        .Where(c=>c.AppToken == token
                                        && c.AppSecret == password && DateTime.UtcNow < c.TokenExpiration)
                                        .FirstOrDefaultAsync();

                    if (authorizedApp != null)
                    {
                        var result = mapper.Map<authorizedapp, Models.AuthorizedApp>(authorizedApp);

                        return (true, result, null);
                    }
                }

                return (false, null, "Not Found");
            }
            catch (Exception ex)
            {                
                return (false, null, ex.Message);
            }
        }
    }
}