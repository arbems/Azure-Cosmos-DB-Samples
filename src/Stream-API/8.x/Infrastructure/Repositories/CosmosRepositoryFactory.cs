using Application.Interfaces;
using Microsoft.Azure.Cosmos;

namespace Infrastructure.Repositories;
public class CosmosRepositoryFactory : ICosmosRepositoryFactory
{
    public ICosmosRepository Create(CosmosClient cosmosClient, string databaseId, string containerId)
    {
        return new CosmosRepository(cosmosClient, databaseId, containerId);
    }
}
