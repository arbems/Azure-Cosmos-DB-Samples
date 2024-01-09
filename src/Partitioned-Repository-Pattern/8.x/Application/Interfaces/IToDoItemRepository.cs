using Domain.Entities;

namespace Application.Interfaces;

public interface IToDoItemRepository : IRepository<TodoItem>
{
    Task<IEnumerable<TodoItem>> GetItemsAsyncByList(string listId);
    Task<IEnumerable<TodoItem>> GetItemsAsyncByTitle(string title);
}
