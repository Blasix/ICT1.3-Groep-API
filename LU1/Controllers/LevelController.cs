using System.Security.Claims;
using LU1.Models;
using LU1.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LU1.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("/[controller]")]
    public class LevelController(LevelsRepository repository, ILogger<LevelController> logger) : ControllerBase
    {
        // GET: /Level/{step}/{trajectId}
        [HttpGet("{step}/{trajectId}")]
        public async Task<IActionResult> GetLevelsByStepAndTraject(int step, string trajectId)
        {
            var levels = await repository.GetLevelsByStepAndTrajectId(step, trajectId);
            if (levels == null || !levels.Any())
            {
                return NotFound();
            }
            return Ok(levels);
        }
    }
}
