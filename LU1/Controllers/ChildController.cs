using System.Security.Claims;
using LU1.Models;
using LU1.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LU1.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/[controller]")]
    public class ChildController(ChildRepository repository, ILogger<ChildController> logger) : ControllerBase
    {
        // GET: /Child
        [HttpGet]
        public async Task<ActionResult<Child>> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var objects = await repository.GetByUserId(userId);
            return Ok(objects);
        }

        // POST: /Child
        [HttpPost]
        public async Task<ActionResult<Child>> Create([FromBody] Child child)
        {
            child.Id = Guid.NewGuid();
            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            child.UserId = Guid.Parse(userId);
            
            await repository.Add(child);
            return CreatedAtAction(nameof(GetAll), child);
        }

        // PUT: /Child/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Child child)
        {
            child.Id = Guid.Parse(id);
            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            child.UserId = Guid.Parse(userId);
            
            var currentChild = (await repository.GetByUserId(userId)).FirstOrDefault(c => c.Id == Guid.Parse(id));

            if (currentChild == null)
            {
                return NotFound();
            }
            
            await repository.Update(child);
            return Ok();
        }

        // DELETE: /Child/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var child = (await repository.GetByUserId(userId)).FirstOrDefault(c => c.Id == Guid.Parse(id));

            if (child == null)
            {
                return NotFound();
            }
            
            await repository.Delete(id);
            return Ok();
        }
    }
}