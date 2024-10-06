using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoList.Data.Models;

namespace TodoList.Data;

public class TodoItemsProvider : ITodoItemsProvider
{
    private readonly TodoContext _context;
    private readonly ILogger<TodoItemsProvider> _logger;

    public TodoItemsProvider(TodoContext context, ILogger<TodoItemsProvider> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TodoItem[]> Get()
    {
        return await _context.TodoItems.Where(x => !x.IsCompleted).ToArrayAsync();
    }

    public async Task<TodoItem> Get(Guid id)
    {
        return await _context.TodoItems.FindAsync(id);
    }

    public async Task<bool> AreAnyNotCompleted(string description)
    {
        return await _context.TodoItems.AnyAsync(x => x.Description.ToLowerInvariant() == description.ToLowerInvariant() && !x.IsCompleted);
    }

    public async Task Update(TodoItem todoItem)
    {
        _context.Entry(todoItem).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TodoItemIdExists(todoItem.Id))
            {
                throw new NotFoundException(nameof(TodoItem));
            }
            else
            {
                throw;
            }
        }
    }

    public async Task Add(TodoItem todoItem)
    {
        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();
    }

    private bool TodoItemIdExists(Guid id)
    {
        return _context.TodoItems.Any(x => x.Id == id);
    }
}
