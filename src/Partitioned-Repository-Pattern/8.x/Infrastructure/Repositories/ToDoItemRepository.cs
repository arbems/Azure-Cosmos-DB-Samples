using Application.Interfaces;
using Domain.Entities;
using Microsoft.Azure.Cosmos;

namespace Infrastructure.Repositories;

public class ToDoItemRepository(CosmosClient cosmosClient) : CosmosDbRepository<TodoItem>(cosmosClient), IToDoItemRepository
{
    public override string DatabaseId { get; } = "db1";
    public override string ContainerId { get; } = "Todo";
    public override string GenerateId(TodoItem entity) => $"{entity.ListId}:{Guid.NewGuid()}";
    public override PartitionKey ResolvePartitionKey(string entityId) => new(entityId.Split(':')[0]);

    public async Task<IEnumerable<TodoItem>> GetItemsAsyncByList(string listId)
    {
        return await GetItemsAsync(@$"SELECT * FROM c WHERE c.listId = '{listId}'");
    }

    public async Task<IEnumerable<TodoItem>> GetItemsAsyncByTitle(string title)
    {
        return await GetItemsAsync(@$"SELECT * FROM c WHERE c.title = '{title}'");
    }
}
