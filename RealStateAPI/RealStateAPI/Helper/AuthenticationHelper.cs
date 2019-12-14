using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RealStateAPI.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RealStateAPI.Helper
{
    public static class AuthenticationHelper
    {
        public static async  Task<string> GenerateJwtToken(string email, UserModel user, IConfiguration configuration, RoleManager<MongoRole> _roleManager)
        {

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
              
            };
            foreach (var claim in user.Claims)
            {
                
                claims.Add(new Claim(ClaimTypes.Role,claim.ClaimValue));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                configuration["JwtIssuer"],
                configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //private async Task<List<Claim>> GetValidClaims(UserModel user)
        //{
        //    IdentityOptions _options = new IdentityOptions();
        //    var claims = new List<Claim>
        //{
        //    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        //    new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
        //    new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
        //    new Claim(_options.ClaimsIdentity.UserIdClaimType, user.Id.ToString()),
        //    new Claim(_options.ClaimsIdentity.UserNameClaimType, user.UserName)
        //};
        //    var userClaims = await _userManager.GetClaimsAsync(user);
        //    var userRoles = await _userManager.GetRolesAsync(user);
        //    claims.AddRange(userClaims);
        //    foreach (var userRole in userRoles)
        //    {
        //        claims.Add(new Claim(ClaimTypes.Role, userRole));
        //        var role = await _roleManager.FindByNameAsync(userRole);
        //        if (role != null)
        //        {
        //            var roleClaims = await _roleManager.GetClaimsAsync(role);
        //            foreach (Claim roleClaim in roleClaims)
        //            {
        //                claims.Add(roleClaim);
        //            }
        //        }
        //    }
        //    return claims;
        //    //}
    }
}
