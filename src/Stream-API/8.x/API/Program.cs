using Application;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
CosmosSettings settings = builder.Configuration.GetSection("Cosmos").Get<CosmosSettings>()!;
await builder.Services.AddInfrastructureServices(settings);
builder.Services.AddApplicationServices();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
