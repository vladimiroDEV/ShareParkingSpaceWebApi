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
using Microsoft.AspNetCore.Cors;
using System.Net;

namespace ShareParkingSpaceWebApi.Controllers.API
{
    [Produces("application/json")]
    [Route("api/AccountApi/[action]")]
    [DisableCors]
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
        public async Task<string> GetOk()
        {
            return "ok";
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
         
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
                    var token =  await GenerateJwtToken(model.Email, appUser);
                return Ok(token);  

                }

            return NotFound(); 
                //throw new ApplicationException("INVALID_LOGIN_ATTEMPT");

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {  
            
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");
                await _signInManager.SignInAsync(user, isPersistent: false);

                var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
                var token =  await GenerateJwtToken(model.Email, appUser);
                return Ok(token);

                //  var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                // var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                // await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl)

            }
          //  throw new StatusCodeResult(HttpStatusCode.NotFound);
            throw new ApplicationException("INVALID_LOGIN_ATTEMPT");

        }


        //[HttpPost]
        //[AllowAnonymous]
        //[Route("api/authentication/FacebookLogin")]
        //public async Task<IActionResult> FacebookLogin([FromBody] FacebookToken facebookToken)
        //{
        //    //check token
        //    var httpClient = new HttpClient { BaseAddress = new Uri("https://graph.facebook.com/v2.9/") };
        //    var response = await httpClient.GetAsync($"me?access_token={facebookToken.Token}&fields=id,name,email,first_name,last_name,age_range,birthday,gender,locale,picture");
        //    if (!response.IsSuccessStatusCode) return BadRequest();
        //    var result = await response.Content.ReadAsStringAsync();
        //    var facebookAccount = JsonConvert.DeserializeObject<FacebookAccount>(result);

        //    //register if required
        //    var facebookUser = _context.FacebookUsers.SingleOrDefault(x => x.Id == facebookAccount.Id);
        //    if (facebookUser == null)
        //    {
        //        var user = new ApplicationUser { UserName = facebookAccount.Name, Email = facebookAccount.Email };
        //        var result2 = await _userManager.CreateAsync(user);
        //        if (!result2.Succeeded) return BadRequest();
        //        facebookUser = new FacebookUser { Id = facebookAccount.Id, UserId = user.Id };
        //        _context.FacebookUsers.Add(facebookUser);
        //        _context.SaveChanges();
        //    }

        //    //send bearer token
        //    return Ok(GetToken(facebookUser.UserId));
        //}



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
