using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

public class GetDataByIdQueryHandler(ICosmosRepository cosmosRepository, ICosmosSettings settings) : IRequestHandler<GetDataByIdQuery, Stream?>
{
    private readonly ICosmosRepository _cosmosRepository = cosmosRepository;
    private readonly ICosmosSettings _settings = settings;

    public async Task<Stream?> Handle(GetDataByIdQuery request, CancellationToken cancellationToken)
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
