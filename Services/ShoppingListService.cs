using FamilySyncApi.Models;
using FamilySyncApi.Repositories;
using FamilySyncApi.Services.Interfaces;

namespace FamilySyncApi.Services.Implementations;

public class ShoppingListService : IShoppingListService
{
    private readonly IBlobStorageRepository<ShoppingItem> _repository;
    private const string BlobName = "ShoppingList.json";

    public ShoppingListService(IBlobStorageRepository<ShoppingItem> repository)
    {
        _repository = repository;
    }

    public Task<List<ShoppingItem>> GetShoppingListAsync()
    {
        return _repository.GetAsync(BlobName);
    }

    public Task SaveShoppingListAsync(List<ShoppingItem> shoppingList)
    {
        return _repository.SaveAsync(BlobName, shoppingList);
    }
}
