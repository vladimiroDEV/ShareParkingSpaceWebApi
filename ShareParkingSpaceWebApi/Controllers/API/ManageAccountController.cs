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
using ShareParkingSpaceWebApi.Models.Helpers;
using ShareParkingSpaceWebApi.Controllers.HUBS;
using Microsoft.AspNetCore.SignalR;

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
        private IHubContext<ManageParkingHub> _manageParkingHub;

        public ManageAccountController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHubContext<ManageParkingHub> manageParkingHub,
             ILogger<ManageAccountController> logger)
        {
            _context = context;
            _manageParkingHub = manageParkingHub;
            _logger = logger;
            _userManager = userManager;
        }

        #region UserInfo
        [HttpGet]
        public async Task<IActionResult> GetUserInfo()
        {

            var userID = User.getUserId();
            var user = _context.Users.Where(u => u.Id == userID).SingleOrDefault();
            //await  _userManager.FindByIdAsync(userID);


            if (user == null) return NotFound();
            Auto auto = _context.Auto.Where(a => a.UderID == userID).SingleOrDefault();

            UserInfoViewModel model = new UserInfoViewModel();

            model.DisplayName = user.DisplayName;
            model.Name = user.Name;
            model.UserId = user.Id;
            model.Surname = user.Surname;
            model.Credits = user.Credits;
            model.Auto = auto;
            model.Email = user.Email;
         

            return Ok(model);

        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserProfile([FromBody]UserProfileViewModel model)
        {
            var userID = User.getUserId();
            var userInfo = _context.Users.Where(u => u.Id == userID).FirstOrDefault();
            if (userInfo == null) return NotFound();

            userInfo.DisplayName = model.DisplayName;
            userInfo.Name = model.Name;
            userInfo.Surname = model.Surname;

            _context.SaveChanges();
            return  await GetUserInfo();

        }

        #endregion


        #region Auto
        [HttpPost]
        public async Task<IActionResult> UpdateAutoInfo([FromBody]UserAutoViewModel model)
        {
            var userID = User.getUserId();
            var userInfo = _context.Users.Where(u => u.Id == userID).FirstOrDefault();
            if (userInfo == null) return NotFound();

            var auto = _context.Auto.Where(a => a.UderID == userID).SingleOrDefault();

           
            // non esiste nel database auto asociata all user 
            if (auto == null)
            {
                auto = new Auto();
                auto.CarBrend = model.CarBrend;
                auto.CarModel = model.CarModel;
                auto.CarColor = model.CarColor;
                auto.NumberPlate = model.NumberPlate;
                auto.UderID = userID;
                _context.Auto.Add(auto);

            }else
            {
                auto.CarBrend = model.CarBrend;
                auto.CarModel = model.CarModel;
                auto.CarColor = model.CarColor;
                auto.NumberPlate = model.NumberPlate;
            }
            _context.SaveChanges();
            return await  GetUserInfo();
                

        }


        #endregion

        #region Credit 

        public async Task<IActionResult> UpdateUserCredit([FromBody]UserCreditViewModel model)
        {
            var userID = User.getUserId();
            var userInfo = _context.Users.Where(u => u.Id == userID).FirstOrDefault();
            if (userInfo == null) return NotFound();
            CreditTransactions transaction = new CreditTransactions();
            transaction.Amount = model.Credit;
            transaction.UserSrc = userID;
            transaction.Date = DateTime.Now;
            transaction.Action = model.Action;



            switch (model.Action) {
                case CreditAction.Refill:
                    userInfo.Credits += model.Credit;
                    
                    break;
                case CreditAction.Withdraw:
                    userInfo.Credits -= model.Credit;
                    break;

                default:
                    throw new NotImplementedException();
                    break;
            }


            _context.CreditTransactions.Add(transaction);
            _context.SaveChanges();
            return await GetUserInfo();

        }

        #endregion  




    }
}