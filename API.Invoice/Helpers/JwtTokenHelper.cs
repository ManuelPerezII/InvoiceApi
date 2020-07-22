using API.Invoice.Config;
using API.Invoice.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace API.Invoice.Helpers
{
    public class JwtTokenHelper
    {
        public TokenDto CreateToken(AuthorizedApp authApp)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var issuedAt = DateTime.UtcNow;
            var expires = DateTime.UtcNow.AddDays(30);
            var claimsIdentity = new ClaimsIdentity(new GenericIdentity(authApp.Name), new[]
            {
                new Claim("appToken", authApp.AppToken, ClaimValueTypes.String),
            });

            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(GlobalConfig.Secret));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            //create the token
            var token = tokenHandler.CreateJwtSecurityToken(
                GlobalConfig.Issuer,
                GlobalConfig.Audience,
                claimsIdentity,
                issuedAt,
                expires,
                signingCredentials: signingCredentials);

            return new TokenDto
            {
                Token = tokenHandler.WriteToken(token),
                Expires = expires,
            };
        }
    }
}