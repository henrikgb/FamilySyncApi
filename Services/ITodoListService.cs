using FamilySyncApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FamilySyncApi.Services.Interfaces;

public interface ITodoListService
{
    Task<List<TodoItem>> GetTodoListAsync();
    Task SaveTodoListAsync(List<TodoItem> todoList);
}
