using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShareParkingSpaceWebApi.Data;
using ShareParkingSpaceWebApi.Models;
using ShareParkingSpaceWebApi.Models.Helpers;

namespace ShareParkingSpaceWebApi.Controllers.API
{
    [Produces("application/json")]
    [Route("api/ParkingSpaces/[action]")]
    public class ParkingSpacesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParkingSpacesController(ApplicationDbContext context)
        {
            _context = context;
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

        // POST: api/ParkingSpaces
        [HttpPost]
        public async Task<IActionResult> AddParkingSpaces([FromBody] ParkingSpaces parkingSpaces)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var p_action = await CreateParkingAction(parkingSpaces, ParkingSpaceAction.Create);

            _context.ParkingSpaces.Add(parkingSpaces);
            _context.ParkingSpaceActions.Add(p_action);
          
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetParkingSpaces", new { id = parkingSpaces.ID }, parkingSpaces);
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

        private Task<ParkingSpaceActions> CreateParkingAction(ParkingSpaces parkingSpaces, ParkingSpaceAction action)
        {
            ParkingSpaceActions _actions = new ParkingSpaceActions();
            _actions.DateAction = DateTime.Now;
            _actions.Lat = parkingSpaces.Lat;
            _actions.Long = parkingSpaces.Long;
            _actions.Action = action;
            _actions.ReservedUsersAutoID = parkingSpaces.ReservedUsersAutoID;
            _actions.UserID = parkingSpaces.UserID;
           // _actions.UserAutoID = parkingSpaces.UserAutoID;
          

            return Task.FromResult<ParkingSpaceActions>(_actions);

        }
    }
}