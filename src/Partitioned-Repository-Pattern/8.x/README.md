# Partitioned Repository Pattern - Azure Cosmos DB Sample
This repository contains an example of web API using Partitioned Repository Pattern with Azure Cosmos DB, ASP.NET Core and Clean architecture.

## Getting Started
Add connectionString and databaseName Cosmos settings:
```
"Cosmos": {
    "ConnString": "<connectionString>",
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
[Partitioned Repository Pattern - Azure Cosmos DB Sample](https://arbems.com)