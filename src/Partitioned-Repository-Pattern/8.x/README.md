# Partitioned Repository Pattern - Azure Cosmos DB Sample
This repository contains an example of web API using Partitioned Repository Pattern with Azure Cosmos DB, ASP.NET Core and Clean architecture.

## Getting Started
Add settings:
```
"CosmosDbSettings": {
    "ConnectionString": "<connString>",
    "Containers": [
      {
        "DatabaseId": "<databaseName>",
        "ContainerId": "Todo",
        "PartitionKey": "/listId"
      }
    ]
  }
```

## Technologies
* [.NET 8](https://dotnet.microsoft.com/es-es/download/dotnet/8.0)

## Versions
The main branch is now on .NET 8.0.

## Learn more
[Aprueba examen AZ-204: Desarrollo de soluciones para Microsoft Azure](https://arbems.com)
