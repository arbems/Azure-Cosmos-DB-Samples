using Domain.Entities;

namespace Application.Interfaces;
public interface ICosmosSettings
{
    string ConnString { get; set; }
    IReadOnlyList<ContainerInfo> Containers { get; set; }

    List<(string, string)> GetContainersToInitialize();
}
