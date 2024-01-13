using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Application.Items.Queries.GetItemsByQuery;

public record GetItemsByQuery : IRequest<Stream?>
{
    [FromRoute]
    public required string ContainerId { get; init; }
    [FromQuery(Name = "queryString")]
    public required string QueryString { get; init; }
}

public class GetItemByIdQueryHandler(ICosmosRepository cosmosRepository, ICosmosSettings settings) : IRequestHandler<GetItemsByQuery, Stream?>
{
    private readonly ICosmosRepository _cosmosRepository = cosmosRepository;
    private readonly ICosmosSettings _settings = settings;

    public async Task<Stream?> Handle(GetItemsByQuery request, CancellationToken cancellationToken)
    {
        var containerSettings = _settings.Containers.FirstOrDefault(a => a.ContainerId == request.ContainerId)
            ?? throw new ArgumentException("ContainerId not found.");

        _cosmosRepository.SetContainer(containerSettings.DatabaseId, containerSettings.ContainerId);
        return await _cosmosRepository.GetItemQueryAsync(
            request.QueryString);
    }
}
