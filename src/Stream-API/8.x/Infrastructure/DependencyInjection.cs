using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;
public static class DependencyInjection
{
    public static async Task<IServiceCollection> AddInfrastructureServices(this IServiceCollection services, ICosmosSettings settings)
    {
        await InitializeDatabaseIfNotExists(settings.ConnString, settings.Containers);

        await InitializeCosmos(services, settings);

        InitializeSettings(services, settings);

        return services;
    }

    public static async Task InitializeDatabaseIfNotExists(string connString, IReadOnlyList<ContainerInfo> containers)
    {
        using CosmosClient client = new(connString);
        foreach (var container in containers)
        {
            DatabaseResponse response = await client.CreateDatabaseIfNotExistsAsync(container.DatabaseId);

            await response.Database
                .CreateContainerIfNotExistsAsync(container.ContainerId, container.PkInfo.PartitionKey);
        }
    }

    private static async Task InitializeCosmos(IServiceCollection services, ICosmosSettings settings)
    {
        var cosmosClient = await CosmosClient.CreateAndInitializeAsync(
            settings.ConnString,
            settings.GetContainersToInitialize(),
            new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions()
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                    IgnoreNullValues = true
                }
            });

        services.AddSingleton(cosmosClient);

        services.AddTransient<ICosmosRepository, CosmosRepository>();
    }

    public static void InitializeSettings(IServiceCollection services, ICosmosSettings settings)
    {
        services.AddSingleton(sp => settings);
    }
}
