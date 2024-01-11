using Application.Interfaces;
using Domain.Entities;
using Microsoft.Azure.Cosmos;

namespace Infrastructure.Repositories;

public class CosmosRepository(CosmosClient cosmosClient) : ICosmosRepository
{
    private readonly CosmosClient _cosmosClient = cosmosClient;
    private Container? _container;
    public void SetContainer(string databaseId, string containerId)
    {
        _container = _cosmosClient.GetContainer(databaseId, containerId);
    }

    public async Task<Stream?> GetItemAsync(string id, PartitionKeyInfo pkInfo, string[] pkArgs)
    {
        if (_container is null) throw new Exception("ContainerId is null or empty.");

        var partitionKeyValue = pkInfo.ResolvePartitionKey(pkArgs);

        ResponseMessage response = await _container.ReadItemStreamAsync(
            id,
            new PartitionKey(partitionKeyValue),
            new ItemRequestOptions { });

        return response.IsSuccessStatusCode
            ? response.Content
            : throw new Exception($"Get item failed. Status code: {response.StatusCode} Message: {response.ErrorMessage}");
    }

    public async Task<Stream?> GetItemQueryAsync(string query)
    {
        if (_container is null) throw new Exception("ContainerId is null or empty.");

        QueryDefinition queryDefinition = new(query);

        using FeedIterator feedIterator = _container.GetItemQueryStreamIterator(
            queryDefinition,
            null,
            new QueryRequestOptions
            {
                MaxItemCount = 100
            });

        while (feedIterator.HasMoreResults)
        {
            ResponseMessage response = await feedIterator.ReadNextAsync();

            if (response.IsSuccessStatusCode)
            {
                return response.Content;
            }
        }

        throw new Exception($"Get items from query failed. Status code: NotFound (404); Message: Resource Not Found.");
    }

    public async Task<Stream?> UpsertItemAsync(Stream item, PartitionKeyInfo pkInfo, string[] pkArgs)
    {
        if (_container is null) throw new Exception("ContainerId is null or empty.");

        var partitionKeyValue = pkInfo.ResolvePartitionKey(pkArgs);
        var modifiedStream = pkInfo.AddPartitionKeyToStream(item, partitionKeyValue);

        ResponseMessage response =
            await _container.UpsertItemStreamAsync(
                partitionKey: new PartitionKey(partitionKeyValue),
                streamPayload: modifiedStream,
                requestOptions: new ItemRequestOptions { });

        return response.IsSuccessStatusCode
            ? response.Content
            : throw new Exception($"Upsert item from stream failed. Status code: {response.StatusCode} Message: {response.ErrorMessage}");
    }

    public async Task<Stream?> CreateItemAsync(Stream item, PartitionKeyInfo pkInfo, string[] pkArgs)
    {
        if (_container is null) throw new Exception("ContainerId is null or empty.");

        var partitionKeyValue = pkInfo.ResolvePartitionKey(pkArgs);
        var modifiedStream = pkInfo.AddPartitionKeyToStream(item, partitionKeyValue);

        ResponseMessage response =
            await _container.CreateItemStreamAsync(
                partitionKey: new PartitionKey(partitionKeyValue),
                streamPayload: modifiedStream);

        return response.IsSuccessStatusCode
            ? response.Content
            : throw new Exception($"Create item from stream failed. Status code: {response.StatusCode} Message: {response.ErrorMessage}");
    }

    public async Task<Stream?> ReplaceItemAsync(Stream item, string id, PartitionKeyInfo pkInfo, string[] itemArgs)
    {
        if (_container is null) throw new Exception("ContainerId is null or empty.");

        var partitionKeyValue = pkInfo.ResolvePartitionKey(itemArgs);
        var modifiedStream = pkInfo.AddPartitionKeyToStream(item, partitionKeyValue);

        ResponseMessage response =
            await _container.ReplaceItemStreamAsync(modifiedStream, id, new PartitionKey(partitionKeyValue));

        return response.IsSuccessStatusCode
            ? response.Content
            : throw new Exception($"Replace item from stream failed. Status code: {response.StatusCode} Message: {response.ErrorMessage}");
    }

    public async Task<Stream?> PatchItemAsync(object[]? patchOperations, string id, PartitionKeyInfo pkInfo, string[] pkArgs)
    {
        if (_container is null) throw new Exception("ContainerId is null or empty.");

        var partitionKeyValue = pkInfo.ResolvePartitionKey(pkArgs);

        try
        {
            ResponseMessage response =
                await _container.PatchItemStreamAsync(
                    id: id,
                    partitionKey: new PartitionKey(partitionKeyValue),
                    patchOperations: patchOperations as PatchOperation[],
                    new PatchItemRequestOptions { });

            return response.IsSuccessStatusCode
                ? response.Content
                : throw new Exception($"Patch item failed. Status code: {response.StatusCode} Message: {response.ErrorMessage}");
        }
        catch
        {
            //Handle and log exception
            throw new Exception($"Error occurred while patching item. Id: {id}, PartitionKey: {partitionKeyValue}");
        }
    }

    public async Task<Stream?> DeleteItemAsync(string id, PartitionKeyInfo pkInfo, string[] pkArgs)
    {
        if (_container is null) throw new Exception("ContainerId is null or empty.");

        var partitionKeyValue = pkInfo.ResolvePartitionKey(pkArgs);

        ResponseMessage response = await _container.DeleteItemStreamAsync(id, new PartitionKey(partitionKeyValue));

        return response.IsSuccessStatusCode
            ? response.Content
            : throw new Exception($"Delete item failed. Status code: {response.StatusCode} Message: {response.ErrorMessage}");
    }
}
