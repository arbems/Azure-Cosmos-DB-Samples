using Infrastructure;
using Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
CosmosDbSettings settings = builder.Configuration.GetSection("CosmosDbSettings").Get<CosmosDbSettings>()!;
await builder.Services.AddCosmosDb(settings);

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
