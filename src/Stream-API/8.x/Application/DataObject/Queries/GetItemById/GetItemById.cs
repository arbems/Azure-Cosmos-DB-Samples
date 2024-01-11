using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DataObject.Queries.GetById;

public record GetDataByIdQuery : IRequest<Stream?>
{
    [FromRoute]
    public required string ContainerId { get; init; }
    [FromRoute]
    public required string Id { get; init; }
    [FromQuery(Name = "args")]
    public required string[] Args { get; init; }
}

public class GetDataByIdQueryHandler(IServiceProvider serviceProvider, CosmosClient cosmosClient, ICosmosSettings settings) : IRequestHandler<GetDataByIdQuery, Stream?>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly CosmosClient _cosmosClient = cosmosClient;
    private readonly ICosmosSettings _settings = settings;

    public async Task<Stream?> Handle(GetDataByIdQuery request, CancellationToken cancellationToken)
    {
        var container = _settings.Containers.FirstOrDefault(a => a.ContainerId == request.ContainerId)
            ?? throw new ArgumentException("ContainerId not found.");

        using var scope = _serviceProvider.CreateScope();
        var cosmosRepositoryFactory = scope.ServiceProvider.GetRequiredService<ICosmosRepositoryFactory>();
        var cosmosRepository = cosmosRepositoryFactory.Create(_cosmosClient, container.DatabaseId, container.ContainerId);

        return await cosmosRepository.GetItemAsync(
            request.Id,
            container.PkInfo,
            request.Args);
    }
}
