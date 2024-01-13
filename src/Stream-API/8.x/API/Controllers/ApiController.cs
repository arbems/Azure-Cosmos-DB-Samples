using System.Text;
using Application.Interfaces;
using Application.Items.Commands.CreateItem;
using Application.Items.Commands.DeleteItem;
using Application.Items.Commands.UpdateItem;
using Application.Items.Queries.GetItemById;
using Application.Items.Queries.GetItemsByQuery;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        [HttpGet]
        [Route("{ContainerId}/{Id}")]
        public async Task<Stream?> Get(ISender sender, [FromQuery] GetItemById query)
        {
            return await sender.Send(query);
        }

        [HttpGet]
        [Route("{ContainerId}/query")]
        public async Task<Stream?> Get(ISender sender, [FromQuery] GetItemsByQuery query)
        {
            return await sender.Send(query);
        }

        [HttpPost]
        [Route("{ContainerId}")]
        public async Task<Stream?> Post(ISender sender, CreateItemCommand command)
        {
            return await sender.Send(command);
        }

        [HttpPut]
        [Route("{ContainerId}/{Id}")]
        public async Task<Stream?> Put(ISender sender, UpdateItemCommand command)
        {
            return await sender.Send(command);
        }

        [HttpDelete]
        [Route("{ContainerId}/{Id}")]
        public async Task<Stream?> Delete(ISender sender, DeleteItemCommand command)
        {
            return await sender.Send(command);
        }
    }
}
