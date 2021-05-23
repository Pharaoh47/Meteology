using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Meteology.Model;

namespace Meteology.Controllers{
    [Route("api/city")]
    [ApiController]
    // Cities controller holds only method for listing cities
    // all CRUD at EntityController generic class
    public class CityController : EntityController<City>
    {
        public CityController(WeatherContext context) : base(context){}
        
        // List all cities sorted by name
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<City>>> List()
        {
            // Async task for sorted list
            return await _context.Cities
                .OrderBy(p=>p.Name)
                .ToListAsync();
        }
    } 
}