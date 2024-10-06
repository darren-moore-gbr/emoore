using TodoList.Data.Models;

namespace TodoList.Logic
{
    public interface ITodoItemsLogic
    {
        Task Add(TodoItem todoItem);
        Task<TodoItem[]> Get();
        Task<TodoItem> Get(Guid id);
        Task Update(TodoItem todoItem);
    }
}