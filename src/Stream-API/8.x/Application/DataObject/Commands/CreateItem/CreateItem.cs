using System.ComponentModel;
using System.Text;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DataObject.Commands.CreateDataObject;

public record CreateDataObjectCommand : IRequest<Stream?>
{
    public required string ContainerId { get; init; }
    [FromForm]
    public required string Data { get; init; }
    [FromForm]
    public required string[] Args { get; init; }
}

public class CreateTodoItemCommandHandler(IServiceProvider serviceProvider, CosmosClient cosmosClient, ICosmosSettings settings)
    : IRequestHandler<CreateDataObjectCommand, Stream?>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly CosmosClient _cosmosClient = cosmosClient;
    private readonly ICosmosSettings _settings = settings;

    public async Task<Stream?> Handle(CreateDataObjectCommand request, CancellationToken cancellationToken)
    {
        var container = _settings.Containers.FirstOrDefault(a => a.ContainerId == request.ContainerId) 
            ?? throw new ArgumentException("ContainerId not found.");

        using var scope = _serviceProvider.CreateScope();
        var cosmosRepositoryFactory = scope.ServiceProvider.GetRequiredService<ICosmosRepositoryFactory>();
        var cosmosRepository = cosmosRepositoryFactory.Create(_cosmosClient, container.DatabaseId, container.ContainerId);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(request.Data));
        return await cosmosRepository.CreateItemAsync(
            stream, 
            container.PkInfo,
            request.Args);
    }
}
