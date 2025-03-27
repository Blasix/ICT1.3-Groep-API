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
        [HttpGet("{childName}")]
        public async Task<ActionResult<AppointmentItem>> GetAll(string childName)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var objects = await repository.GetAppointmentsByUserIdAndChildName(userId, childName);
            return Ok(objects);
        }

        // POST: /appointments
        [HttpPost]
        public async Task<IActionResult> PostAppointment(AppointmentItem appointment)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await repository.Add(appointment);
            return Ok();
        }

        // DELETE: /appointments/{Appointment Name}
        [HttpDelete("{childName}/{appointmentName}")]
        public async Task<IActionResult> Delete(string childName, string appointmentName)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var appointment = (await repository.GetAppointmentIdByUserIdChildNameAndAppointmentName(userId, childName, appointmentName));

            await repository.Delete(userId, childName, appointment.id);
            return Ok();
        }

        [HttpGet("validation/{childName}/{appointmentName}")]
        public async Task<ActionResult<bool>> ValidateAppointmentName(string childName, string appointmentName)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var appointment = (await repository.GetAppointmentIdByUserIdChildNameAndAppointmentName(userId, childName, appointmentName));
            return appointment != null;
        }
    }
}
