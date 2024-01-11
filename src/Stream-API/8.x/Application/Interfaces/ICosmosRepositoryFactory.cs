using Microsoft.Azure.Cosmos;

namespace Application.Interfaces;
public interface ICosmosRepositoryFactory
{
    ICosmosRepository Create(CosmosClient cosmosClient, string databaseId, string containerId);
}
