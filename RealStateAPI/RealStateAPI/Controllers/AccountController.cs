using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RealStateAPI.Helper;
using RealStateAPI.Models;

namespace RealStateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private UserManager<UserModel> _userManager;
        private SignInManager<UserModel> _signInManager;
        private  IConfiguration _configuration;
        private RoleManager<MongoRole> _roleManager;

        public AccountController(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, IConfiguration configuration, RoleManager<MongoRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        

        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new UserModel { UserName = model.EmailAddress};
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    Response.StatusCode = 201;
                    return Content($"User with {model.EmailAddress} was created");
                }
                else
                {
                    Response.StatusCode = 400;
                    return Content(result.ToString());
                }
                

            }
            else
            {
                Console.WriteLine("Test");
                Response.StatusCode = 400;
                return Content("Model is not Valid");
            }

        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.EmailAddress, model.Password, false, false);
                if (result.Succeeded)
                {
                    var appUser = _userManager.Users.SingleOrDefault(r => r.UserName == model.EmailAddress);
                    var token = await AuthenticationHelper.GenerateJwtToken(model.EmailAddress, appUser, _configuration, _roleManager);
                    Response.StatusCode = 200;
                    var rootData = new LoginResponse(token, appUser.UserName);
                    return Ok(rootData);
                }
                if (result.IsLockedOut)
                {
                    Response.StatusCode = 400;
                    return Content("Account is locked");
                }
                else
                {
                    Response.StatusCode = 400;
                    return Content(result.ToString());
                }
            }

            Response.StatusCode = 400;
            return Content("Model is incorrect");
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("GetUsers")]
        [Authorize(Roles = Role.User)]
        public async Task<ActionResult> UserData()
        {
            var user = await _userManager.GetUserAsync(User);
            var userData = new 
            {
                Name = user.UserName,

            };
            return Ok(userData);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("AddRole")]
        public async Task<ActionResult> AddToRole(AddRoleModel AddRoles)
        {
            var u = await _userManager.FindByNameAsync(AddRoles.UserName);
            if (u == null) return NotFound();
            var result = new List<string>();
            foreach (var roleName in AddRoles.Roles)
            {
                var role = await _roleManager.RoleExistsAsync(roleName);
                if (!role)
                {
                    result.Add(roleName);
                    continue;
                }
                await _userManager.AddToRoleAsync(u, roleName);
                await _userManager.AddClaimAsync(u, new Claim(ClaimTypes.Role, roleName));
            }
            Response.StatusCode = 200;
            if (result.Count > 0)
            {
                var body = String.Join("", result);
                return Ok($"following role(s) [{body}]  were not added");
            }
            
            return Ok();
        }


    }
}