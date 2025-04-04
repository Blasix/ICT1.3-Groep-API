using System.Security.Claims;
using LU1.Models;
using LU1.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LU1.Controllers;

[ApiController]
[Authorize]
[Route("/[controller]")]
public class NoteController(NoteRepository noteRepository, ChildRepository childRepository) : ControllerBase
{
    // GET: /Note/{childId}
    [HttpGet("{childId}")]
    public async Task<ActionResult<Note>> GetAll(string childId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var children = await childRepository.GetByUserId(userId);
        if (children.FirstOrDefault(c => c.Id == Guid.Parse(childId)) == null)
        {
            return BadRequest("Child not found");
        }
        
        var objects = await noteRepository.GetByChildId(childId);
        return Ok(objects);
    }
    
    // POST: /Note
    [HttpPost]
    public async Task<ActionResult<Note>> Create([FromBody] Note note)
    {
        note.Id = Guid.NewGuid();
        note.NoteDate = DateTime.Now;
                
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var children = await childRepository.GetByUserId(userId);
        if (children.FirstOrDefault(c => c.Id == note.ChildId) == null)
        {
            return BadRequest("Child not found");
        }
                
        await noteRepository.Add(note);
        return CreatedAtAction(nameof(GetAll), note);
    }
    
    // PUT: /Note/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Note note)
    {
        note.Id = Guid.Parse(id);
        note.NoteDate = DateTime.Now;
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var currentNote =
            (await noteRepository.GetByChildId(note.ChildId.ToString()))
            .FirstOrDefault(c => c.Id == Guid.Parse(id));
        if (currentNote == null)
        {
            return NotFound();
        }
        
        var children = await childRepository.GetByUserId(userId);
        if (children.FirstOrDefault(c => c.Id == note.ChildId) == null)
        {
            return BadRequest("Child not found");
        }
                
        await noteRepository.Update(note);
        return Ok();
    }
    
    // DELETE: /Note/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await noteRepository.Delete(id);
        return Ok();
    }
}