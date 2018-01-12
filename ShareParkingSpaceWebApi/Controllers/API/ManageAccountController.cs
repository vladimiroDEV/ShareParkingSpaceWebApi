using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShareParkingSpaceWebApi.Data;
using Microsoft.AspNetCore.Identity;
using ShareParkingSpaceWebApi.Models;
using ShareParkingSpaceWebApi.Models.AccountViewModels;
using ShareParkingSpaceWebApi.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace ShareParkingSpaceWebApi.Controllers.API
{

    [Produces("application/json")]
    [Route("api/ManageAccount/[action]")]
    [Authorize]
    public class ManageAccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ManageAccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public ManageAccountController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
             ILogger<ManageAccountController> logger)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        #region UserInfo
        [HttpGet]
        public IActionResult GetUserInfo()
        {

            var userID = User.getUserId();
            var user = _context.Users.Where(u => u.Id == userID).SingleOrDefault();
            //await  _userManager.FindByIdAsync(userID);


            if (user == null) return NotFound();
            Auto auto = _context.Auto.Where(a => a.UderID == userID).SingleOrDefault();

            UserInfoViewModel model = new UserInfoViewModel();

            model.DisplayName = user.DisplayName;
            model.Name = user.Name;
            model.Surname = user.Surname;
            model.Credits = user.Credits;
            model.Auto = auto;

            return Ok(model);

        }

        [HttpPost]
        
        public IActionResult UpdateUserInfo([FromBody]UserInfoViewModel model)
        {
            var userID = User.getUserId();
            var userInfo = _context.Users.Where(u => u.Id == userID).FirstOrDefault();
            if (userInfo == null) return NotFound();

            userInfo.DisplayName = model.DisplayName;
            userInfo.Name = model.Name;
            userInfo.Surname = model.Surname;

            _context.SaveChanges();
            return Ok();

        }


        #endregion


        
    }
}