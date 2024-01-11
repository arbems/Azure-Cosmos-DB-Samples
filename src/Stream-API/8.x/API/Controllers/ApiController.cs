using System.Text;
using Application.DataObject.Commands.CreateDataObject;
using Application.DataObject.Queries.GetById;
using Application.Interfaces;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController(CosmosClient cosmosClient, ICosmosSettings settings, IServiceProvider serviceProvider) : ControllerBase
    {
        private readonly CosmosClient _cosmosClient = cosmosClient;
        private readonly ICosmosSettings _settings = settings;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        [HttpGet]
        [Route("{ContainerId}/{Id}")]
        public async Task<Stream?> Get(ISender sender, [FromQuery] GetDataByIdQuery query)
        {
            return await sender.Send(query);
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
        [Route("{ContainerId}")]
        public async Task<Stream?> Post(ISender sender, CreateDataObjectCommand command)
        {
            return await sender.Send(command);
        }

        /*[HttpPut]
        [Route("{containerId}/{id}")]
        public async Task<Stream?> Put(string containerId, string id, [FromForm] string data, [FromQuery] string[] args)
        {
            var container = _settings.Containers
                .FirstOrDefault(a => a.ContainerId == containerId) ?? throw new ArgumentException("ContainerId not found.");

            var repo = ActivatorUtilities.CreateInstance<CosmosRepository>
                (_serviceProvider, _cosmosClient, container.DatabaseId, container.ContainerId);

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
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
        }*/
    }
}
