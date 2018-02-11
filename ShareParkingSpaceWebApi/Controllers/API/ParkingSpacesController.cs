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

namespace ShareParkingSpaceWebApi.Controllers.API
{
    [Produces("application/json")]
    [Route("api/ParkingSpaces/[action]")]
    [Authorize]
    public class ParkingSpacesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParkingSpacesController(ApplicationDbContext context)
        {
            _context = context;
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
            return Ok();
        }


        // GET: api/ParkingSpaces
        //[Route("api/ParkingSpaces/GetParkingSpaces/{location}")]

        [HttpGet("{location}")]
        //[Route("GetParkingSpaces/{location}")]
        public IEnumerable<ParkingSpaces> GetParkingSpaces(string location)
        {
            return _context.ParkingSpaces.Where(s=>s.Location ==location && s.State == ParkingSpaceState.Free).ToList();
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