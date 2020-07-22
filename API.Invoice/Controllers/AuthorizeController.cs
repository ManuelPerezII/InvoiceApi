using API.Invoice.Helpers;
using API.Invoice.Interfaces;
using API.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace API.Invoice.Controllers
{
    [RoutePrefix("api/authorize")]
    public class AuthorizeController: ApiController
    {
        private readonly JwtTokenHelper jwtTokenHelper = new JwtTokenHelper();
        private readonly IAuthorizedAppProvider authorizedAppProvider;

        public AuthorizeController(IAuthorizedAppProvider authorizedAppProvider)
        {
            this.authorizedAppProvider = authorizedAppProvider;
        }

        public async Task<IHttpActionResult> Post(AuthorizeRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await authorizedAppProvider.GetAuth(request.AppToken,request.AppSecret);
            if (result.IsSuccess)
            {                
                var token = jwtTokenHelper.CreateToken(result.AuthorizedApp);
                return Ok(token);                
            }

            return Unauthorized();

        }
    }
}