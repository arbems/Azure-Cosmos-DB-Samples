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