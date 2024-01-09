using Domain.Common;
using Microsoft.Azure.Cosmos;

namespace Infrastructure.Interfaces;

public interface IContainerContext<T> where T : BaseEntity
{
    string DatabaseId { get; }
    string ContainerId { get; }
    string GenerateId(T entity);
    PartitionKey ResolvePartitionKey(string entityId);
}
