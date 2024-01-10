using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Application.Interfaces;
using Azure;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Repositories;

public class CosmosRepository : ICosmosRepository
{
    private readonly string _databaseId;
    private readonly string _containerId;

    public string ResolvePartitionKey(string template, string pattern, params string[] args)
    {
        try {
            string input = string.Format(template, args);

            return Regex.IsMatch(input, pattern)
            ? input
            : throw new Exception("Error resolving partition key.");
        }
        catch {
            throw new Exception("Error resolving partition key, check args and container.");
        }
        
    }

    /// <summary>
    /// Cosmos DB container
    /// </summary>
    private readonly Container _container;

    public CosmosRepository(CosmosClient cosmosClient, string databaseId, string containerId)
    {
        _databaseId = databaseId;
        _containerId = containerId;
        _container = cosmosClient.GetContainer(_databaseId, _containerId);

        if (_container is null)
        {
            throw new ArgumentException("ContainerId cannot be null or empty.");
        }
    }

    public async Task<Stream?> GetItemAsync(string id, string partitionKey)
    {
        ResponseMessage response = await _container.ReadItemStreamAsync(
            id,
            new PartitionKey(partitionKey),
            new ItemRequestOptions { });

        return response.IsSuccessStatusCode
            ? response.Content
            : throw new Exception($"Get item from stream failed. Status code: {response.StatusCode} Message: {response.ErrorMessage}");
    }

    public async Task<Stream?> GetItemQueryAsync(string query)
    {
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

        throw new Exception($"Get item from stream failed. Status code: NotFound (404); Message: Resource Not Found.");
    }

    public async Task<Stream?> UpsertItemAsync(Stream item, string partitionKey)
    {
        ResponseMessage response =
            await _container.UpsertItemStreamAsync(
                partitionKey: new PartitionKey(partitionKey),
                streamPayload: item,
                requestOptions: new ItemRequestOptions { });

        return response.IsSuccessStatusCode
            ? response.Content
            : throw new Exception($"Upsert item from stream failed. Status code: {response.StatusCode} Message: {response.ErrorMessage}");
    }

    public async Task<Stream?> CreateItemAsync(Stream item, string partitionKeyName, string partitionKeyValue)
    {
        var modifiedStream = AddPartitionKeyToStream(item, partitionKeyName, partitionKeyValue);

        ResponseMessage response =
            await _container.CreateItemStreamAsync(
                partitionKey: new PartitionKey(partitionKeyValue),
                streamPayload: modifiedStream);

        return response.IsSuccessStatusCode
            ? response.Content
            : throw new Exception($"Create item from stream failed. Status code: {response.StatusCode} Message: {response.ErrorMessage}");
    }

    public async Task<Stream?> ReplaceItemAsync(Stream item, string id, string partitionKey)
    {
        ResponseMessage response =
            await _container.ReplaceItemStreamAsync(item, id, new PartitionKey(partitionKey));

        return response.IsSuccessStatusCode
            ? response.Content
            : throw new Exception($"Replace item from stream failed. Status code: {response.StatusCode} Message: {response.ErrorMessage}");
    }

    public async Task<Stream?> PatchItemAsync(object[]? patchOperations, string id, string partitionKey)
    {
        try
        {
            ResponseMessage response =
                await _container.PatchItemStreamAsync(
                    id: id,
                    partitionKey: new PartitionKey(partitionKey),
                    patchOperations: patchOperations as PatchOperation[],
                    new PatchItemRequestOptions { });

            return response.IsSuccessStatusCode
                ? response.Content
                : throw new Exception($"Patch item from stream failed. Status code: {response.StatusCode} Message: {response.ErrorMessage}");
        }
        catch
        {
            //Handle and log exception
            throw new Exception($"Error occurred while patching item. Id: {id}, PartitionKey: {partitionKey}");
        }
    }

    public async Task<Stream?> DeleteItemAsync(string id, string partitionKey)
    {
        ResponseMessage response =
            await _container.DeleteItemStreamAsync(id, new PartitionKey(partitionKey));

        return response.IsSuccessStatusCode
            ? response.Content
            : throw new Exception($"Delete item from stream failed. Status code: {response.StatusCode} Message: {response.ErrorMessage}");
    }

    private static MemoryStream AddPartitionKeyToStream(Stream stream, string newPropertyName, JToken newValue)
    {
        string pattern = @"^[^a-zA-Z]*(\w+)[^a-zA-Z]*$";
        string extractedNewPropertyName = string.Empty;
        Match match = Regex.Match(newPropertyName, pattern);
        if (match.Success)
        {
            extractedNewPropertyName = match.Groups[1].Value;
        }

        using var reader = new StreamReader(stream);
        var jsonObject = JObject.Parse(reader.ReadToEnd());

        if (jsonObject.ContainsKey(extractedNewPropertyName))
        {
            jsonObject[extractedNewPropertyName] = newValue;
        }
        else
        {
            jsonObject.Add(extractedNewPropertyName, newValue);
        }

        return new MemoryStream(Encoding.UTF8.GetBytes(jsonObject.ToString()));
    }

}
