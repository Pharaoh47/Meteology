using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Meteology.Model;

namespace Meteology.Controllers{

    // Primary class for CRUD, it is generic on the type of BaseEntity
    [ApiController]
    public class EntityController<TEntity> : ControllerBase where TEntity : BaseEntity
    {
        // Constructor accept data context and we save it
        protected readonly WeatherContext _context;
        public EntityController(WeatherContext context)
        {
            _context = context;
        }

        // Read action, by entity id
        [HttpGet("{id}")]
        public async Task<ActionResult<TEntity>> Get(int id)
        {
            // Awaits our entity
            TEntity entity = await _context.Set<TEntity>().FindAsync(id);
            // Return it or throw an 404
            if (entity == null) return NotFound(); 
            return entity;
        }

        // Create action
        [HttpPost]
        public async Task<ActionResult<TEntity>> Create(TEntity entity)
        {
            // Add our entity
            _context.Set<TEntity>().Add(entity);
            // And awaits its save
            await _context.SaveChangesAsync();
            // Return 201 and entity
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity);
        }

        // Update action
        [HttpPut("{id}")]
        public async Task<ActionResult<TEntity>> Update(int id, TEntity updatedEntity)
        {
            // Ensure an foreign key is correct
            updatedEntity.Id = id;
            // Attach and mark entity for save
            _context.Attach(updatedEntity);
            _context.Entry(updatedEntity).State = EntityState.Modified;
            // Awaits saving entity
            await _context.SaveChangesAsync();
            // Return 200
            return Ok();
        }

        // Delete action
        [HttpDelete("{id}")]
        public async Task<ActionResult<TEntity>> Delete(int id)
        {
            // Check if entity for deletion exist
            TEntity entity = await _context.Set<TEntity>().FindAsync(id);
            // Return 404 if not
            if (entity == null)
                return NotFound(); 
            // Delete entity
            _context.Set<TEntity>().Remove(entity);
            // Awaits its removal
            await _context.SaveChangesAsync();
            // Return 200
            return Ok();
        }    
    }
}