using System.Text;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Application.Items.Commands.CreateItem;

public record CreateItemCommand : IRequest<Stream?>
{
    public required string ContainerId { get; init; }
    [FromForm]
    public required string Item { get; init; }
    [FromForm]
    public required string[] Args { get; init; }
}

public class CreateTodoItemCommandHandler(ICosmosRepository cosmosRepository, ICosmosSettings settings)
    : IRequestHandler<CreateItemCommand, Stream?>
{
    private readonly ICosmosRepository _cosmosRepository = cosmosRepository;
    private readonly ICosmosSettings _settings = settings;

    public async Task<Stream?> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var containerSettings = _settings.Containers.FirstOrDefault(a => a.ContainerId == request.ContainerId)
            ?? throw new ArgumentException("ContainerId not found.");

        _cosmosRepository.SetContainer(containerSettings.DatabaseId, containerSettings.ContainerId);
        using var item = new MemoryStream(Encoding.UTF8.GetBytes(request.Item));
        return await _cosmosRepository.CreateItemAsync(
            item,
            containerSettings.PkInfo,
            request.Args);
    }
}
