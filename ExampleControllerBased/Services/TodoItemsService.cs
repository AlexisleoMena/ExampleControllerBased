using ExampleControllerBased.Data;
using ExampleControllerBased.DTOs;
using ExampleControllerBased.Models;
using Microsoft.EntityFrameworkCore;

namespace ExampleControllerBased.Services
{
    public class TodoItemsService
    {
        private readonly TodoContext _context;
        public TodoItemsService(TodoContext todoContext)
        {
            _context = todoContext;
        }

        public async Task<IEnumerable<TodoItemDTO>> GetAll()
        {
            return await _context.TodoItems
                .Select(x => new TodoItemDTO(x))
                .ToListAsync();
        }

        public async Task<TodoItemDTO?> GetById(long id)
        {
            var todo = await _context.TodoItems.FindAsync(id);
            return (todo != null) ? new TodoItemDTO(todo) : null;
        }

        public async Task<bool> Update(TodoItemDTO itemDTO)
        {
            var todo = await _context.TodoItems.FindAsync(itemDTO.Id);
            if (todo == null) return false;

            todo.Name = itemDTO.Name;
            todo.IsComplete = itemDTO.IsComplete;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<(bool created, TodoItemDTO todoDTOCreated)> Create(TodoItemDTO todoItemDTO)
        {
            var todoItem = new TodoItem
            {
                IsComplete = todoItemDTO.IsComplete,
                Name = todoItemDTO.Name
            };
            _context.TodoItems.Add(todoItem);
            bool created = await _context.SaveChangesAsync() > 0;
            return (created, new TodoItemDTO(todoItem));
        }

        public async Task<bool> Delete(TodoItem todoItem)
        {
            _context.TodoItems.Remove(todoItem);
            return await _context.SaveChangesAsync() > 0;
        }

        //public bool TodoItemExists(long id)
        //{
        //    return (_context.TodoItems?.Any(e => e.Id == id)).GetValueOrDefault();
        //}
    }
}
