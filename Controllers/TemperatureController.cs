using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Meteology.Model;

namespace Meteology.Controllers{
    [Route("api/temperature")]
    [ApiController]
    // Temperature controller holds only methods for listing temperature measurements
    // All CRUD at EntityController generic class
    public class TemperatureController : EntityController<Temperature>
    {
        public TemperatureController(WeatherContext context) : base(context){}

        // List all measurements
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<Temperature>>> List()
        {
            // Async task for sorted list
            return await _context.Temperatures
                .OrderBy(t => t.Date)
                .ToListAsync();
        }
        
        // List measurements by city
        [HttpGet("list/{cityId}")]
        public async Task<ActionResult<IEnumerable<Temperature>>> List(int cityId)
        {
            // Async task for sorted list for one city
            return await _context.Temperatures
                .Where(t => t.CityId == cityId)
                .OrderBy(t => t.Date)
                .ToListAsync();
        }
    } 
}