using Microsoft.AspNetCore.Mvc;
using ShovelHero.Models;

namespace ShovelHero.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationController : ControllerBase
{
    private readonly DataStore _store;

    public ApplicationController(DataStore store)
    {
        _store = store;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_store.Applications);
    }

    [HttpGet("by-demand/{demandId:guid}")]
    public IActionResult GetByDemand(Guid demandId)
    {
        var apps = _store.Applications.Where(a => a.DemandId == demandId).ToList();
        return Ok(apps);
    }

    [HttpPost]
    public IActionResult Create([FromBody] ApplicationCreateDto dto)
    {
        var appx = new Application
        {
            Id = Guid.NewGuid(),
            DemandId = dto.DemandId,
            Name = dto.Name,
            Phone = dto.Phone,
            AvailableTime = dto.AvailableTime,
            Skills = dto.Skills,
            Tools = dto.Tools,
            CreatedAt = DateTime.UtcNow
        };
        _store.Applications.Add(appx);
        return CreatedAtAction(nameof(GetAll), new { id = appx.Id }, appx);
    }
}
