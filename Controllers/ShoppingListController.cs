using FamilySyncApi.Models;
using FamilySyncApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FamilySyncApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ShoppingListController : ControllerBase
{
    private readonly IBlobStorageRepository<ShoppingItem> _repository;
    private const string BlobName = "ShoppingList.json";

    public ShoppingListController(IBlobStorageRepository<ShoppingItem> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<ShoppingItem>>> Get()
    {
        var items = await _repository.GetAsync(BlobName);
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] List<ShoppingItem> items)
    {
        await _repository.SaveAsync(BlobName, items);
        return NoContent();
    }
}
