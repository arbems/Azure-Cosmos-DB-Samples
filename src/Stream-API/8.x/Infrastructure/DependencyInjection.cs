using System;
using Application.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Settings;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;
public static class DependencyInjection
{
    public static async Task<IServiceCollection> AddCosmosDb(this IServiceCollection services, CosmosSettings settings)
    {
        await EnsureDbSetup(settings.ConnString, settings.Containers);

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

        return services;
    }

    public static IServiceCollection AddSettings(this IServiceCollection services, CosmosSettings settings)
    {
        services.AddSingleton(settings);

        return services;
    }

    public static async Task EnsureDbSetup(string connString, IReadOnlyList<ContainerInfo> containers)
    {
        using CosmosClient client = new(connString);
        foreach (var container in containers)
        {
            DatabaseResponse response = await client.CreateDatabaseIfNotExistsAsync(container.DatabaseId);

            await response.Database
                .CreateContainerIfNotExistsAsync(container.ContainerId, container.PkInfo.PartitionKey);
        }
    }
}
