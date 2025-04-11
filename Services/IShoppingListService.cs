using FamilySyncApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FamilySyncApi.Services.Interfaces;

public interface IShoppingListService
{
    Task<List<ShoppingItem>> GetShoppingListAsync();
    Task SaveShoppingListAsync(List<ShoppingItem> shoppingList);
}
