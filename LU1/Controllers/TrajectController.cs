using LU1.Models;
using LU1.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LU1.Controllers;

[ApiController]
[Authorize]
[Route("/[controller]")]
public class TrajectController(TrajectRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Traject>> GetAll()
    {
        var objects = await repository.GetAll();
        return Ok(objects);
    }
}