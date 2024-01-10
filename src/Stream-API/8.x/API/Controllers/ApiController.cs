using System.ComponentModel;
using System.Text;
using Infrastructure.Repositories;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController(CosmosClient cosmosClient, CosmosSettings settings, IServiceProvider serviceProvider) : ControllerBase
    {
        private readonly CosmosClient _cosmosClient = cosmosClient;
        private readonly CosmosSettings _settings = settings;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        [HttpGet]
        [Route("{containerId}/{id}")]
        public async Task<Stream?> Get(string containerId, string id, [FromQuery] string[] args)
        {
            var container = _settings.Containers
                .FirstOrDefault(a => a.ContainerId == containerId) ?? throw new ArgumentException("ContainerId not found.");

            var repo = ActivatorUtilities.CreateInstance<CosmosRepository>
                (_serviceProvider, _cosmosClient, container.DatabaseId, container.ContainerId);

            return await repo.GetItemAsync(id, repo.ResolvePartitionKey(container.PkInfo.Template, container.PkInfo.Pattern, args));
        }

        [HttpGet]
        [Route("{containerId}")]
        public async Task<Stream?> GetByQuery(string containerId, [FromQuery] string query)
        {
            var container = _settings.Containers
                .FirstOrDefault(a => a.ContainerId == containerId) ?? throw new ArgumentException("ContainerId not found.");

            var repo = ActivatorUtilities.CreateInstance<CosmosRepository>
                (_serviceProvider, _cosmosClient, container.DatabaseId, container.ContainerId);

            return await repo.GetItemQueryAsync(query);
        }

        [HttpPost]
        [Route("{containerId}")]
        public async Task<Stream?> Post(string containerId, [FromForm] string item, [FromQuery] string[] args)
        {
            var container = _settings.Containers
                .FirstOrDefault(a => a.ContainerId == containerId) ?? throw new ArgumentException("ContainerId not found.");

            var repo = ActivatorUtilities.CreateInstance<CosmosRepository>
                (_serviceProvider, _cosmosClient, container.DatabaseId, container.ContainerId);

            var cosmosRepository = ActivatorUtilities.CreateInstance<CosmosRepository>(
                _serviceProvider, 
                _cosmosClient,
                container.DatabaseId,
                container.ContainerId);

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(item));
            return await cosmosRepository.CreateItemAsync(stream, container.PkInfo.PartitionKey, repo.ResolvePartitionKey(container.PkInfo.Template, container.PkInfo.Pattern, args));
        }

        [HttpPut]
        [Route("{containerId}/{id}")]
        public async Task<Stream?> Put(string containerId, string id, [FromForm] string item, [FromQuery] string[] args)
        {
            var container = _settings.Containers
                .FirstOrDefault(a => a.ContainerId == containerId) ?? throw new ArgumentException("ContainerId not found.");

            var repo = ActivatorUtilities.CreateInstance<CosmosRepository>
                (_serviceProvider, _cosmosClient, container.DatabaseId, container.ContainerId);

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(item));
            return await repo.ReplaceItemAsync(stream, id, repo.ResolvePartitionKey(container.PkInfo.Template, container.PkInfo.Pattern, args));
        }

        [HttpDelete]
        [Route("{containerId}/{id}")]
        public async Task<Stream?> Delete(string containerId, string id, [FromQuery] string[] args)
        {
            var container = _settings.Containers
                .FirstOrDefault(a => a.ContainerId == containerId) ?? throw new ArgumentException("ContainerId not found.");

            var repo = ActivatorUtilities.CreateInstance<CosmosRepository>
                (_serviceProvider, _cosmosClient, container.DatabaseId, container.ContainerId);

            return await repo.DeleteItemAsync(id, repo.ResolvePartitionKey(container.PkInfo.Template, container.PkInfo.Pattern, args));
        }
    }
}
