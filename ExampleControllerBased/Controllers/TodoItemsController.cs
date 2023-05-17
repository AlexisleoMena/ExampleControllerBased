using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExampleControllerBased.Data;
using ExampleControllerBased.Models;
using ExampleControllerBased.DTOs;
using ExampleControllerBased.Services;

namespace ExampleControllerBased.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly TodoItemsService _service;
        public TodoItemsController(TodoContext context, TodoItemsService service)
        {
            _context = context;
            _service = service;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
            return Ok(await _service.GetAll());
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
            return await _service.GetById(id)
                is TodoItemDTO todoDTO ? Ok(todoDTO) : NotFound();
        }

        // PUT: api/TodoItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItemDTO todoItemDTO)
        {
            if (id != todoItemDTO.Id) return BadRequest();
            var existingItem = await _service.GetById(id);
            if (existingItem is null) return NotFound();
            try
            {
                await _service.Update(todoItemDTO);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict();
            }
            return NoContent();
        }

        // POST: api/TodoItems
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItemDTO todoItemDTO)
        {
            var (created, todoDTOCreated) = await _service.Create(todoItemDTO);
            if(!created) return BadRequest();
            return CreatedAtAction(nameof(GetTodoItem), new { id = todoDTOCreated.Id }, todoDTOCreated);
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null) return NotFound();

            bool deleted = await _service.Delete(todoItem);
            if (!deleted) return BadRequest();

            return NoContent();
        }
        
    }
}
