using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Application.Items.Commands.DeleteItem;

public record DeleteItemCommand : IRequest<Stream?>
{
    public required string ContainerId { get; init; }
    public required string Id { get; init; }
    [FromForm]
    public required string[] Args { get; init; }
}

public class DeleteItemCommandHandler(ICosmosRepository cosmosRepository, ICosmosSettings settings) 
    : IRequestHandler<DeleteItemCommand, Stream?>
{
    private readonly ICosmosRepository _cosmosRepository = cosmosRepository;
    private readonly ICosmosSettings _settings = settings;

    public async Task<Stream?> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var containerSettings = _settings.Containers.FirstOrDefault(a => a.ContainerId == request.ContainerId)
            ?? throw new ArgumentException("ContainerId not found.");

        _cosmosRepository.SetContainer(containerSettings.DatabaseId, containerSettings.ContainerId);
        return await _cosmosRepository.DeleteItemAsync(
            request.Id,
            containerSettings.PkInfo,
            request.Args);
    }

}
