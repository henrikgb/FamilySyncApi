using FamilySyncApi.Models;
using FamilySyncApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FamilySyncApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalendarScheduleController : ControllerBase
{
    private readonly IBlobStorageRepository<ScheduleItem> _repository;
    private const string BlobName = "CalendarSchedule.json";

    public CalendarScheduleController(IBlobStorageRepository<ScheduleItem> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<ScheduleItem>>> Get()
    {
        var items = await _repository.GetAsync(BlobName);
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] List<ScheduleItem> items)
    {
        await _repository.SaveAsync(BlobName, items);
        return NoContent();
    }
}
