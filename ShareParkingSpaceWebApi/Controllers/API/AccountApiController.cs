using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ShareParkingSpaceWebApi.Models;
using ShareParkingSpaceWebApi.Services;
using Microsoft.Extensions.Logging;
using ShareParkingSpaceWebApi.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ShareParkingSpaceWebApi.Controllers.API
{
    [Produces("application/json")]
    [Route("api/AccountApi/[action]")]
    public class AccountApiController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private IEmailSender _emailSender;
        private ILogger<AccountController> _logger;
        private IConfiguration _configuration;

        public AccountApiController(
           UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           IEmailSender emailSender,
           ILogger<AccountController> logger,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _configuration = configuration;
        }


        [HttpGet]
        public async Task<string> Get()
        {
            return "ok";
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<object> Login([FromBody]LoginViewModel model)
        {
         
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
                    return await GenerateJwtToken(model.Email, appUser);
                }
                throw new ApplicationException("INVALID_LOGIN_ATTEMPT");

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<object> Register([FromBody]RegisterViewModel model)
        {  
            
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");
                await _signInManager.SignInAsync(user, isPersistent: false);

                var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
                return await GenerateJwtToken(model.Email, appUser);

                //  var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                // var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                // await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl)

            }

            throw new ApplicationException("INVALID_LOGIN_ATTEMPT");

        }




        #region Methods 

        private async Task<object> GenerateJwtToken(string email, IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["jwt:JwtExpireDays"]));
            var token = new JwtSecurityToken(
               _configuration["jwt:JwtIssuer"],
               _configuration["jwt:JwtIssuer"],
               claims,
               expires: expires,
               signingCredentials: creds
           );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

#endregion  

    }
    }
