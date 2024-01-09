using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController(IToDoItemRepository repo) : ControllerBase
    {
        private readonly IToDoItemRepository _repo = repo;

        [HttpGet]
        [Route("{itemId}")]
        public async Task<TodoItem> Get(string itemId)
        {
            return await _repo.GetItemAsync(itemId);
        }

        [HttpGet]
        public async Task<IEnumerable<TodoItem>> GetAll()
        {
            return await _repo.GetItemsAsync(x => true, a => a.Id);
        }

        [HttpGet]
        [Route("list/{listId}")]
        public async Task<IEnumerable<TodoItem>> GetByList(string listId)
        {
            return await _repo.GetItemsAsyncByList(listId);
        }

        [HttpPost]
        public async Task Post(TodoItem item)
        {
            await _repo.AddItemAsync(item);
        }

        [HttpPut]
        public async Task Put(string itemId, TodoItem item)
        {
            await _repo.UpdateItemAsync(itemId, item);
        }

        [HttpDelete]
        public async Task Delete(string itemId)
        {
            await _repo.DeleteItemAsync(itemId);
        }
    }
}
