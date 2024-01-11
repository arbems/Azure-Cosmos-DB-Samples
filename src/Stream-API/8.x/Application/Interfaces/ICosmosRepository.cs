using Domain.Entities;

namespace Application.Interfaces;

public interface ICosmosRepository
{
    void SetContainer(string databaseId, string containerId);
    Task<Stream?> GetItemAsync(string id, PartitionKeyInfo pkinfo, string[] args);
    Task<Stream?> GetItemQueryAsync(string query);
    Task<Stream?> CreateItemAsync(Stream item, PartitionKeyInfo pkinfo, string[] args);
    Task<Stream?> ReplaceItemAsync(Stream item, string id, PartitionKeyInfo pkinfo, string[] args);
    Task<Stream?> PatchItemAsync(object[]? patchOperations, string id, PartitionKeyInfo pkinfo, string[] args);
    Task<Stream?> UpsertItemAsync(Stream item, PartitionKeyInfo pkinfo, string[] args);
    Task<Stream?> DeleteItemAsync(string id, PartitionKeyInfo pkinfo, string[] args);
}
