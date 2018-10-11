using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Models;

namespace TodoApi.Controllers{
    [Route("api/todos")]
    [ApiController]
    public class TodoController : ControllerBase{
       private readonly TodoContext _context;

        public TodoController(TodoContext context){
            _context = context;

            if(!_context.TodoItems.Any()){
                _context.TodoItems.Add(new TodoItem { Name = "None"});
                _context.SaveChanges();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.TodoItems.ToListAsync());

        [HttpGet("{id}", Name = "GetTodo")]
        public async Task<IActionResult> GetById(long id){
            var item = await _context.TodoItems.FirstOrDefaultAsync( i => i.Id == id);
            if(item == null) return NotFound();

            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodo(TodoItem item){
            _context.TodoItems.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtRoute("GetTodo", new { id = item.Id}, item);
        }

        [HttpPut]
        public async Task<IActionResult> EditTodo(int id, TodoItem item){
            var fetchedItem = await _context.TodoItems.FirstOrDefaultAsync(i => i.Id == id);
            if(fetchedItem == null) return NotFound();

            fetchedItem.Name = item.Name;
            fetchedItem.IsComplete = item.IsComplete;
            _context.TodoItems.Update(fetchedItem);
            await _context.SaveChangesAsync();

            return Ok(fetchedItem);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTodo(int id){
            var fetchedItem = await _context.TodoItems.FirstOrDefaultAsync(i => i.Id == id);
            if(fetchedItem == null) return Ok();
            _context.TodoItems.Remove(fetchedItem);
            _context.SaveChanges();
            return NoContent();
        }
    }
}