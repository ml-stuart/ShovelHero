using Microsoft.AspNetCore.Mvc;
using ShovelHero.Models;

namespace ShovelHero.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DemandController : ControllerBase
{
    private readonly DataStore _store;

    public DemandController(DataStore store)
    {
        _store = store;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_store.Demands);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var demand = _store.Demands.FirstOrDefault(d => d.Id == id);
        if (demand == null) return NotFound();
        return Ok(demand);
    }

    [HttpPost]
    public IActionResult Create([FromBody] DemandCreateDto dto)
    {
        var newDemand = new Demand
        {
            Id = Guid.NewGuid(),
            TaskType = dto.TaskType,
            AddressCode = dto.AddressCode,
            RequiredCount = dto.RequiredCount,
            MeetingPoint = dto.MeetingPoint,
            RiskNote = dto.RiskNote,
            ContactInfo = dto.ContactInfo,
            CreatedAt = DateTime.UtcNow
        };
        _store.Demands.Add(newDemand);
        return CreatedAtAction(nameof(GetById), new { id = newDemand.Id }, newDemand);
    }
}
