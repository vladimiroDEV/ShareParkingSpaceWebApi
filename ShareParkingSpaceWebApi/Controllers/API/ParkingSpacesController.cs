using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShareParkingSpaceWebApi.Data;
using ShareParkingSpaceWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using ShareParkingSpaceWebApi.Models.Helpers;
using ShareParkingSpaceWebApi.Models.ParkingSpacesVM;
using ShareParkingSpaceWebApi.Extensions;
using Microsoft.AspNetCore.SignalR;
using ShareParkingSpaceWebApi.Controllers.HUBS;
using Microsoft.AspNetCore.Identity;

namespace ShareParkingSpaceWebApi.Controllers.API
{
    [Produces("application/json")]
    [Route("api/ParkingSpaces/[action]")]
    [Authorize]
    public class ParkingSpacesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IHubContext<ManageParkingHub> _manageParkingHub;
        private UserManager<ApplicationUser> _userManager;

        public ParkingSpacesController(
             UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IHubContext<ManageParkingHub> manageParkingHub)
        {
            _context = context;
            _manageParkingHub = manageParkingHub;
            _userManager = userManager;
        }



        [HttpPost]
        public async Task<IActionResult> AddParkingSpace([FromBody] ParkingSpaces parkingSpaces)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userID = User.getUserId();
            var auto = _context.Auto.Where(a => a.UderID == userID).FirstOrDefault();
            parkingSpaces.AutoID = auto.AutoID;
            parkingSpaces.UserID = userID;
            parkingSpaces.State = ParkingSpaceState.Free;

            var p_ac = createActionParkingSpace(parkingSpaces, ParkingSpaceAction.Create);
            _context.ParkingSpaceActions.Add(p_ac);
            _context.ParkingSpaces.Add(parkingSpaces);

            await _context.SaveChangesAsync();
            var freeSpaces = GetParkingSpaces(parkingSpaces.Location);
            // per aggiornare hub con i pakeggi liberi
            await _manageParkingHub.Clients.Group(parkingSpaces.Location).InvokeAsync("send", freeSpaces);

            return CreatedAtAction("GetParkingSpaces", new { id = parkingSpaces.ID }, parkingSpaces);
        }

        [HttpPost]
        public async Task<IActionResult> ReserveParkingSpace([FromBody]ReserveParkingSpaceVM reserveModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var parking = _context.ParkingSpaces.Where(i => i.ID == reserveModel.parkingID && i.State == ParkingSpaceState.Free).SingleOrDefault();
            if (parking == null) return NotFound();

            parking.State = ParkingSpaceState.Reserved;
            parking.ReservedAutoID = reserveModel.AutoID;


            var p_ac = createActionParkingSpace(parking, ParkingSpaceAction.Reserved);
            _context.ParkingSpaceActions.Add(p_ac);
            await _context.SaveChangesAsync();

            // riaggiorna la mappa a tutti i client 
            var freeSpaces = GetParkingSpaces(parking.Location);
            // per aggiornare hub con i parkeggi liberi
            await _manageParkingHub.Clients.Group(parking.Location).InvokeAsync("send", freeSpaces);


            // avvisa utente del parcheggio riservato

            
            MyPakingVm vm = new MyPakingVm();
            Auto  auto = _context.Auto.Where(i => i.AutoID == parking.ReservedAutoID).SingleOrDefault();
            vm.UserAuto = auto;
            var us = _context.Users.Where(u => u.Id == auto.UderID).SingleOrDefault();
            if (us != null)
                vm.Username = us.DisplayName != "" ? us.DisplayName : us.Email;
            await _manageParkingHub.Clients.Group(parking.UserID).InvokeAsync("send", vm);


            return Ok(ModelState);
        }


        [HttpGet("{userId}")]
        public async Task<IActionResult> getMySharedParking(string userId)
        {

            ParkingSpaces spaces = _context.ParkingSpaces.Where(i => i.UserID == userId).FirstOrDefault();
            if (spaces == null) return NotFound();

            Auto auto = null;
            ApplicationUser us = null;
            MyPakingVm vm = new MyPakingVm();
            vm.lat = spaces.Lat;
            vm.lng = spaces.Long;

            if (spaces.State == ParkingSpaceState.Reserved)
            {
                auto = _context.Auto.Where(a => a.AutoID == spaces.ReservedAutoID).SingleOrDefault();
                us = _context.Users.Where(u => u.Id == auto.UderID).SingleOrDefault();
                vm.UserAuto = auto;
                if (us != null)
                    vm.Username = us.DisplayName != "" ? us.DisplayName : us.Email;
                    
            }

            await _manageParkingHub.Clients.Group(userId).InvokeAsync("send", vm);

            return Ok(vm);

        }

        // GET: api/ParkingSpaces
        //[Route("api/ParkingSpaces/GetParkingSpaces/{location}")]

        [HttpGet("{location}")]
        //[Route("GetParkingSpaces/{location}")]
        public IEnumerable<ParkingSpaces> GetParkingSpaces(string location)
        {
            var userID = User.getUserId();
            return _context.ParkingSpaces
                .Where(s=>s.Location ==location && s.State == ParkingSpaceState.Free && s.UserID != userID)
                .ToList();
        }

        [HttpGet("{id}")]
        public IActionResult GetParkingSpceInfo(long id)
        {
            var parking = _context.ParkingSpaces.Where(i => i.ID == id).SingleOrDefault();
            if (parking == null) return NotFound();
            var auto = _context.Auto.Where(a => a.AutoID == parking.AutoID).SingleOrDefault();
            var userinfo = _context.Users.Where(u => u.Id == parking.UserID).SingleOrDefault();

            return new JsonResult(new ParkingInfoVM()
            {
                ParkingID = parking.ID,
                UserAuto = auto,
                username = userinfo.Email,
                lat = parking.Lat,
                lon = parking.Long

            });





        }

        private bool ParkingSpacesExists(long id)
        {
            return _context.ParkingSpaces.Any(e => e.ID == id);
        }
        private ParkingSpaceActions createActionParkingSpace(ParkingSpaces parking, ParkingSpaceAction action)
        {
            ParkingSpaceActions p_action = new ParkingSpaceActions();
            p_action.Action = action;
            p_action.UserID = parking.UserID;
            p_action.DateAction = DateTime.Now;
            p_action.AutoID = parking.AutoID;
            p_action.ReservedAutoID = parking.ReservedAutoID;
            p_action.Lat = parking.Lat;
            p_action.Long = parking.Long;


            return p_action;

        }


        [HttpPost]
        public async Task<IActionResult> PaidForParkingSpace([FromBody]long ParkingId)
        {
            
            var userid = User.getUserId();
            var destiantionAutoID = _context.ParkingSpaces.Where(i => i.ID == ParkingId).Select(a => a.ReservedAutoID).SingleOrDefault();
            var destinationUserId = _context.Auto.Where(i => i.AutoID == destiantionAutoID).Select(u => u.UderID).SingleOrDefault();

            var sourceUser = _context.Users.Where(u => u.Id == userid).SingleOrDefault();
            sourceUser.Credits = sourceUser.Credits - 1;

            var destUser = _context.Users.Where(u => u.Id == destinationUserId).SingleOrDefault();
            destUser.Credits = destUser.Credits + 1;

            await _context.SaveChangesAsync();
            return Ok(ModelState);

        }
    }
}