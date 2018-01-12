using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShareParkingSpaceWebApi.Data;
using ShareParkingSpaceWebApi.Models;
using Microsoft.Extensions.Logging;
using ShareParkingSpaceWebApi.Models.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using ShareParkingSpaceWebApi.Extensions;

namespace ShareParkingSpaceWebApi.Controllers.API
{
    [Produces("application/json")]
    [Route("api/MaanageAccount/[action]")]
    public class MaanageAccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MaanageAccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public MaanageAccountController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
             ILogger<MaanageAccountController> logger)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }



        #region UserInfo
        [HttpGet]
        public IActionResult GetUserInfo()
        {

            var userID  = User.getUserId();
            var user = _context.Users.Where(u => u.Id == userID).Include(a => a.Auto).SingleOrDefault();
                //await  _userManager.FindByIdAsync(userID);


            if (user == null) return NotFound();

            UserInfoViewModel model = new UserInfoViewModel();
            model.DisplayName = user.DisplayName;
            model.Name = user.Name;
            model.Surname = user.Surname;
            model.Credits = user.Credits;
            model.Auto = user.Auto;

            return Ok(model);

        }

        [HttpPost]
        public IActionResult UpdateUserInfo(UserInfoViewModel model)
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
        #region UserAuto

        #endregion

        #region UserCredit

        #endregion



        // GET: api/MaanageAccount
        [HttpGet]
        public IEnumerable<Auto> GetUserAuto()
        {
            return _context.UserAuto;
        }

        // GET: api/MaanageAccount/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAuto([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userAuto = await _context.UserAuto.SingleOrDefaultAsync(m => m.AutoID == id);

            if (userAuto == null)
            {
                return NotFound();
            }

            return Ok(userAuto);
        }

     

        // POST: api/MaanageAccount
        [HttpPost]
        public async Task<IActionResult> AddUserAuto([FromBody] Auto userAuto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.UserAuto.Add(userAuto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserAuto", new { id = userAuto.AutoID }, userAuto);
        }

     

        private bool UserAutoExists(long id)
        {
            return _context.UserAuto.Any(e => e.AutoID == id);
        }
    }
}