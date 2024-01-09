using Application.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Settings;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;
public static class DependencyInjection
{
    public static async Task<IServiceCollection> AddCosmosDb(this IServiceCollection services,
        CosmosDbSettings settings)
    {
        await EnsureDbSetup(settings.ConnectionString, settings.Containers);

        var cosmosClient = await CosmosClient.CreateAndInitializeAsync(
            settings.ConnectionString,
            settings.GetContainersToInitialize(),
            new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions()
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                    IgnoreNullValues = true
                }
            });

        // Microsoft recommends a singleton client instance to be used throughout the application
        // https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.cosmos.cosmosclient?view=azure-dotnet#definition
        // "CosmosClient is thread-safe. Its recommended to maintain a single instance of CosmosClient per lifetime of the application which enables efficient connection management and performance"
        services.AddSingleton(cosmosClient);

        services.AddScoped<IToDoItemRepository, ToDoItemRepository>();

        return services;
    }

    public static async Task EnsureDbSetup(string connString, IReadOnlyList<ContainerInfo> containers)
    {
        using CosmosClient client = new(connString);
        foreach (var container in containers)
        {
            DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(container.DatabaseId);

            await database.Database.CreateContainerIfNotExistsAsync(container.ContainerId, container.PartitionKey);
        }
    }
}
