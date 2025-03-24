using System.Security.Claims;
using LU1.Models;
using LU1.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LU1.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/appointments")]
    public class AppointmentController(AppointmentRepository repository, ILogger<AppointmentController> logger) : ControllerBase
    {
        // GET: /appointments
        [HttpGet("{childName}")]
        public async Task<ActionResult<AppointmentItem>> GetAll(string childName)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var objects = await repository.GetByUserIdAndChildName(userId, childName);
            return Ok(objects);
        }



        //// POST: /appointments
        //[HttpPost]
        //public async Task<ActionResult<AppointmentItem>> Create([FromBody] AppointmentItem appointment)
        //{
        //    appointment.Id = Guid.NewGuid();

        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    appointment.UserId = Guid.Parse(userId);

        //    await repository.Add(appointment);
        //    return CreatedAtAction(nameof(GetAll), appointment);
        //}
        //// PUT: /appointments/{id}
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(string id, [FromBody] AppointmentItem appointment)
        //{
        //    appointment.Id = Guid.Parse(id);

        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    appointment.UserId = Guid.Parse(userId);

        //    var currentAppointment = (await repository.GetByUserId(userId)).FirstOrDefault(c => c.Id == Guid.Parse(id));
        //    if (currentAppointment == null)
        //    {
        //        return NotFound();
        //    }

        //    await repository.Update(appointment);
        //    return Ok();
        //}
        //// DELETE: /appointments/{id}
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(string id)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    var appointment = (await repository.GetByUserId(userId)).FirstOrDefault(c => c.Id == Guid.Parse(id));
        //    if (appointment == null)
        //    {
        //        return NotFound();
        //    }

        //    await repository.Delete(appointment);
        //    return Ok();
        //}
    }
}
