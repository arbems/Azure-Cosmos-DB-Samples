namespace Infrastructure.Settings;

public class CosmosSettings
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
public class ContainerInfo
{
    public required string DatabaseId { get; set; }
    public required string ContainerId { get; set; }
    public required PartitionKeyInfo PkInfo { get; set; }
}

public class PartitionKeyInfo
{
    public required string PartitionKey { get; set; }
    public required string Template { get; set; }
    public required string Pattern { get; set; }
}
