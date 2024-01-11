using Application.Interfaces;
using Domain.Entities;

namespace Infrastructure.Settings;

public class CosmosSettings : ICosmosSettings
{
    public required string ConnString { get; set; }
    public required IReadOnlyList<ContainerInfo> Containers { get; set; }

    public List<(string, string)> GetContainersToInitialize()
    {
        return Containers
            .Select(container => (container.DatabaseId, container.ContainerId))
            .ToList();
    }
}
