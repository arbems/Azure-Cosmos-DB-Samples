namespace Infrastructure.Settings;

public class CosmosDbSettings
{
    public required string ConnectionString { get; set; }
    public required IReadOnlyList<ContainerInfo> Containers { get; set; }

    public List<(string, string)> GetContainersToInitialize()
    {
        return Containers
            .Select(container => (container.DatabaseId, container.ContainerId))
            .ToList();
    }
}
public class ContainerInfo
{
    public required string DatabaseId { get; set; }
    public required string ContainerId { get; set; }
    public required string PartitionKey { get; set; }
}
