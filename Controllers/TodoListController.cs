using FamilySyncApi.Models;
using FamilySyncApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FamilySyncApi.Controllers;

[Authorize(Roles = "FamilySyncUser")]
[ApiController]
[Route("api/[controller]")]
public class TodoListController : ControllerBase
{
    private readonly IBlobStorageRepository<TodoItem> _repository;
    private const string BlobName = "TodoList.json";

    public TodoListController(IBlobStorageRepository<TodoItem> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<TodoItem>>> Get()
    {
        var items = await _repository.GetAsync(BlobName);
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] List<TodoItem> items)
    {
        await _repository.SaveAsync(BlobName, items);
        return NoContent();
    }
}
