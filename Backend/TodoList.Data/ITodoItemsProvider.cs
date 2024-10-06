using TodoList.Data.Models;

namespace TodoList.Data;

public interface ITodoItemsProvider
{
    Task Add(TodoItem todoItem);
    Task<TodoItem[]> Get();
    Task<TodoItem> Get(Guid id);
    Task Update(TodoItem todoItem);
    Task<bool> AreAnyNotCompleted(string description);
}