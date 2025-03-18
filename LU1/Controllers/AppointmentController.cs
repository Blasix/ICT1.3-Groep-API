using LU1_1._3.Models;
using LU1_1._3.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LU1_1._3.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly ILogger<AppointmentController> _logger;
        private readonly AppointmentRepository _appointmentRepository;

        // Inject the AppointmentRepository into the controller
        public AppointmentController(ILogger<AppointmentController> logger, AppointmentRepository appointmentRepository)
        {
            _logger = logger;
            _appointmentRepository = appointmentRepository;
        }

        // GET: /Appointment
        [HttpGet]
        public async Task<ActionResult<List<Appointment>>> GetAll()
        {
            try
            {
                var appointments = await _appointmentRepository.GetAllAsync();
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving all appointments: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /Appointment/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetById(string id)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound(); // Return 404 if no appointment found
                }
                return Ok(appointment); // Return the appointment if found
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving appointment with id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /Appointment/child/{childId}
        [HttpGet("child/{childId}")]
        public async Task<ActionResult<List<Appointment>>> GetByChildId(string childId)
        {
            try
            {
                var appointments = await _appointmentRepository.GetByChildIdAsync(childId);
                if (appointments == null || appointments.Count == 0)
                {
                    return NotFound(); // Return 404 if no appointments found
                }
                return Ok(appointments); // Return the appointments if found
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving appointments for child with id {childId}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: /Appointment
        [HttpPost]
        public async Task<ActionResult<Appointment>> Create([FromBody] Appointment appointment)
        {
            try
            {
                if (appointment == null)
                {
                    return BadRequest("Appointment data is null");
                }

                // Create the appointment using the repository
                var createdAppointment = await _appointmentRepository.CreateAsync(appointment);

                return CreatedAtAction(nameof(GetById), new { id = createdAppointment.Id }, createdAppointment);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating appointment: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: /Appointment/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var existingAppointment = await _appointmentRepository.GetByIdAsync(id);
                if (existingAppointment == null)
                {
                    return NotFound(); // Return 404 if the appointment to delete doesn't exist
                }

                // Delete the appointment using the repository
                await _appointmentRepository.DeleteAsync(id);

                return Ok(); // Return 200 OK after deletion
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting appointment with id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
