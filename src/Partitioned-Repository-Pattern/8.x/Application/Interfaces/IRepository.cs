using System.Linq.Expressions;
using Domain.Common;

namespace Application.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetItemsAsync(string query);
    Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> order);
    Task<T> GetItemAsync(string itemId);
    Task AddItemAsync(T item);
    Task UpdateItemAsync(string itemId, T item);
    Task DeleteItemAsync(string itemId);
}
