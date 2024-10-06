using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TodoList.Data.Models;
using TodoList.Logic;

namespace TodoList.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoItemsController : ControllerBase
{
    private readonly ITodoItemsLogic _todoItemsRules;
    private readonly ILogger<TodoItemsController> _logger;

    public TodoItemsController(ITodoItemsLogic todoItemsRules, ILogger<TodoItemsController> logger)
    {
        _todoItemsRules = todoItemsRules;
        _logger = logger;
    }

    // GET: api/TodoItems
    [HttpGet]
    public async Task<IActionResult> GetTodoItems()
    {
        var results = await _todoItemsRules.Get();
        return Ok(results);
    }

    // GET: api/TodoItems/...
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTodoItem(Guid id)
    {
        var result = await _todoItemsRules.Get(id);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    // PUT: api/TodoItems/... 
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodoItem(Guid id, TodoItem todoItem)
    {
        if (id != todoItem.Id)
        {
            return BadRequest();
        }

        try
        {
            await _todoItemsRules.Update(todoItem);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    } 

    // POST: api/TodoItems 
    [HttpPost]
    public async Task<IActionResult> PostTodoItem(TodoItem todoItem)
    {
        try
        {
            await _todoItemsRules.Add(todoItem);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ExistsException ex)
        {
            return BadRequest(ex.Message);
        }
         
        return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
    }
}
