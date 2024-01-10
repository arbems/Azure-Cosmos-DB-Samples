namespace Application.Interfaces;

public interface ICosmosRepository
{
    string ResolvePartitionKey(string template, string pattern, params string[] args);
    Task<Stream?> GetItemAsync(string id, string partitionKey);
    Task<Stream?> GetItemQueryAsync(string query);
    Task<Stream?> CreateItemAsync(Stream item, string partitionKeyName, string partitionKeyValue);
    Task<Stream?> ReplaceItemAsync(Stream item, string id, string partitionKey);
    Task<Stream?> PatchItemAsync(object[]? patchOperations, string id, string partitionKey);
    Task<Stream?> UpsertItemAsync(Stream item, string partitionKey);
    Task<Stream?> DeleteItemAsync(string id, string partitionKey);
}
