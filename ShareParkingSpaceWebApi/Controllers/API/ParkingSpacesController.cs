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
        [HttpGet]
        public IEnumerable<ParkingSpaces> GetParkingSpaces()
        {
            return _context.ParkingSpaces;
        }

        // GET: api/ParkingSpaces/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetParkingSpaces([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var parkingSpaces = await _context.ParkingSpaces.SingleOrDefaultAsync(m => m.ID == id);

            if (parkingSpaces == null)
            {
                return NotFound();
            }

            return Ok(parkingSpaces);
        }

        // PUT: api/ParkingSpaces/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParkingSpaces([FromRoute] long id, [FromBody] ParkingSpaces parkingSpaces)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != parkingSpaces.ID)
            {
                return BadRequest();
            }

            _context.Entry(parkingSpaces).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParkingSpacesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/ParkingSpaces/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParkingSpaces([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var parkingSpaces = await _context.ParkingSpaces.SingleOrDefaultAsync(m => m.ID == id);
            if (parkingSpaces == null)
            {
                return NotFound();
            }

            _context.ParkingSpaces.Remove(parkingSpaces);
            await _context.SaveChangesAsync();

            return Ok(parkingSpaces);
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