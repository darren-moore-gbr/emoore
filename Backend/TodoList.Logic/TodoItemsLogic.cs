using Microsoft.Extensions.Logging;
using TodoList.Data;
using TodoList.Data.Models;

namespace TodoList.Logic;

public class TodoItemsLogic : ITodoItemsLogic
{
    private readonly ITodoItemsProvider _todoItemsProvider;
    private readonly ILogger<TodoItemsLogic> _logger;

    public TodoItemsLogic(ITodoItemsProvider todoItemsProvider, ILogger<TodoItemsLogic> logger)
    {
        _todoItemsProvider = todoItemsProvider;
        _logger = logger;
    }

    public async Task<TodoItem[]> Get()
    {
        try
        {
            return await _todoItemsProvider.Get();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get failed");
            throw;
        }
    }

    public async Task<TodoItem> Get(Guid id)
    {
        try
        {
            return await _todoItemsProvider.Get(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Get({id}) failed");
            throw;
        }
    }

    public async Task Update(TodoItem todoItem)
    {
        try
        {
            await _todoItemsProvider.Update(todoItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Update({todoItem.Id}) failed");
            throw;
        }
    }

    public async Task Add(TodoItem todoItem)
    {
        if (string.IsNullOrEmpty(todoItem?.Description))
        {
            throw new ValidationException("Description is required");
        }

        try
        {
            if (await _todoItemsProvider.AreAnyNotCompleted(todoItem.Description))
            {
                throw new ExistsException("Description already exists");
            }
            await _todoItemsProvider.Add(todoItem);
        }
        catch (ExistsException ex)
        {
            _logger.LogInformation(ex, $"Add already existed");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Add failed");
            throw;
        }
    }
}
