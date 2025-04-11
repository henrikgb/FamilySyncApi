using FamilySyncApi.Models;
using FamilySyncApi.Repositories;
using FamilySyncApi.Repositories.Interfaces;
using FamilySyncApi.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace FamilySyncApi.Services.Implementations;

public class TodoListService : ITodoListService
{
    private readonly IBlobStorageRepository<TodoItem> _repository;
    private const string BlobName = "TodoList.json";

    public TodoListService(IBlobStorageRepository<TodoItem> repository)
    {
        _repository = repository;
    }

    public Task<List<TodoItem>> GetTodoListAsync()
    {
        return _repository.GetAsync(BlobName);
    }

    public Task SaveTodoListAsync(List<TodoItem> todoList)
    {
        return _repository.SaveAsync(BlobName, todoList);
    }
}
