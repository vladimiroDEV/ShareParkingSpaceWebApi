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

namespace ShareParkingSpaceWebApi.Controllers.API
{
    [Produces("application/json")]
    [Route("api/ParkingSpaces/[action]")]
    [Authorize]
    public class ParkingSpacesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IHubContext<ManageParkingHub> _manageParkingHub;

        public ParkingSpacesController(
            ApplicationDbContext context,
            IHubContext<ManageParkingHub> manageParkingHub)
        {
            _context = context;
            _manageParkingHub = manageParkingHub;
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

            var spaces  = _context.ParkingSpaces.Where(i => i.UserID == parking.UserID).ToList();
            await _manageParkingHub.Clients.Group(parking.UserID).InvokeAsync("send", spaces);


            return Ok(ModelState);
        }


        // GET: api/ParkingSpaces
        //[Route("api/ParkingSpaces/GetParkingSpaces/{location}")]

        [HttpGet("{location}")]
        //[Route("GetParkingSpaces/{location}")]
        public IEnumerable<ParkingSpaces> GetParkingSpaces(string location)
        {
            return _context.ParkingSpaces.Where(s=>s.Location ==location && s.State == ParkingSpaceState.Free).ToList();
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
    }
}