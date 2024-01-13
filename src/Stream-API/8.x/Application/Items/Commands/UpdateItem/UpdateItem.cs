using System.Text;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Application.Items.Commands.UpdateItem;

public record UpdateItemCommand : IRequest<Stream?>
{
    public required string ContainerId { get; init; }
    public required string Id { get; init; }
    [FromForm]
    public required string Item { get; init; }
    [FromForm]
    public required string[] Args { get; init; }
}

public class UpdateItemCommandHandler(ICosmosRepository cosmosRepository, ICosmosSettings settings)
    : IRequestHandler<UpdateItemCommand, Stream?>
{
    private readonly ICosmosRepository _cosmosRepository = cosmosRepository;
    private readonly ICosmosSettings _settings = settings;

    public async Task<Stream?> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        var containerSettings = _settings.Containers.FirstOrDefault(a => a.ContainerId == request.ContainerId)
            ?? throw new ArgumentException("ContainerId not found.");

        _cosmosRepository.SetContainer(containerSettings.DatabaseId, containerSettings.ContainerId);
        using var item = new MemoryStream(Encoding.UTF8.GetBytes(request.Item));
        return await _cosmosRepository.ReplaceItemAsync(
            item,
            request.Id,
            containerSettings.PkInfo,
            request.Args);
    }
}
