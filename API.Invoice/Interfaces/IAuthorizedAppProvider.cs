using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace API.Invoice.Interfaces
{
    public interface IAuthorizedAppProvider
    {
        Task<(bool IsSuccess,Models.AuthorizedApp AuthorizedApp, string ErrorMessage)> GetAuth(string token,string password);
    }
}