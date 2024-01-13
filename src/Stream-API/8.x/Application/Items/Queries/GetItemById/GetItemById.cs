using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Application.Items.Queries.GetItemById;

public record GetItemById : IRequest<Stream?>
{
    [FromRoute]
    public required string ContainerId { get; init; }
    [FromRoute]
    public required string Id { get; init; }
    [FromQuery(Name = "args")]
    public required string[] Args { get; init; }
}

public class GetItemByIdQueryHandler(ICosmosRepository cosmosRepository, ICosmosSettings settings) : IRequestHandler<GetItemById, Stream?>
{
    private readonly ICosmosRepository _cosmosRepository = cosmosRepository;
    private readonly ICosmosSettings _settings = settings;

    public async Task<Stream?> Handle(GetItemById request, CancellationToken cancellationToken)
    {
        var containerSettings = _settings.Containers.FirstOrDefault(a => a.ContainerId == request.ContainerId)
            ?? throw new ArgumentException("ContainerId not found.");

        _cosmosRepository.SetContainer(containerSettings.DatabaseId, containerSettings.ContainerId);
        return await _cosmosRepository.GetItemAsync(
            request.Id,
            containerSettings.PkInfo,
            request.Args);
    }
}
