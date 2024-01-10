using System.Linq.Expressions;
using System.Net;
using Application.Interfaces;
using Domain.Common;
using Infrastructure.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Infrastructure.Repositories;

public abstract class CosmosRepository<T> : IRepository<T>, IContainerContext<T> where T : BaseEntity
{
    public abstract string DatabaseId { get; }

    public abstract string ContainerId { get; }

    public abstract string GenerateId(T entity);

    public abstract PartitionKey ResolvePartitionKey(string entityId);

    /// <summary>
    /// Cosmos DB container
    /// </summary>
    private readonly Container _container;

    public CosmosRepository(CosmosClient cosmosClient)
    {
        _container = cosmosClient.GetContainer(DatabaseId, ContainerId);

        if (_container is null)
        {
            throw new ArgumentException("ContainerId cannot be null or empty.");
        }
    }

    public async Task<T> GetItemAsync(string itemId)
    {
        try
        {
            ItemResponse<T> response = await _container.ReadItemAsync<T>(itemId, ResolvePartitionKey(itemId));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null!;
        }
    }

    public async Task<IEnumerable<T>> GetItemsAsync(string query)
    {
        QueryDefinition queryDefinition = new(query);
        using FeedIterator<T> queryResultSetIterator = _container.GetItemQueryIterator<T>(queryDefinition);
        List<T> items = [];

        while (queryResultSetIterator.HasMoreResults)
        {
            FeedResponse<T> currentResultSet = await queryResultSetIterator.ReadNextAsync();

            items.AddRange(currentResultSet.ToList());
        }

        return items;
    }

    public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> order)
    {
        var queryable = _container.GetItemLinqQueryable<T>();

        using FeedIterator<T> feed = queryable
            .Where(predicate)
            .OrderByDescending(order)
            .ToFeedIterator();

        List<T> items = [];

        while (feed.HasMoreResults)
        {
            var currentResultSet = await feed.ReadNextAsync();

            items.AddRange(currentResultSet.ToList());
        }

        return items;
    }

    public async Task AddItemAsync(T item)
    {
        try
        {
            item.Id = GenerateId(item);

            ItemResponse<T> itemResponse = await _container.ReadItemAsync<T>(item.Id, ResolvePartitionKey(item.Id));
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            ItemResponse<T> itemResponse = await _container.CreateItemAsync(item, ResolvePartitionKey(item.Id));
        }
    }

    public async Task UpdateItemAsync(string itemId, T item)
    {
        await _container.UpsertItemAsync(item, ResolvePartitionKey(itemId));
    }

    public async Task DeleteItemAsync(string itemId)
    {
        ItemResponse<T> itemResponse = await _container.DeleteItemAsync<T>(itemId, ResolvePartitionKey(itemId));
    }
}
