# Stream API - Azure Cosmos DB Sample
This repository contains an example web API that uses Stream Operations with Azure Cosmos DB, ASP.NET Core, and Clean architecture.

Project features:
* Generic Documents
* Initializes the client with containers provided
* Cosmos settings: ContaninerInfo & PartitionKeyInfo
* Partitioned Repository Pattern: Stream Operations
* Cosmos Settings: Containers
* Stream operations
* Resolve Partition Key with arguments & container settings
* Optimistic concurrency control: etag
* JSON Schema Validator
* Evaluate Cosmos Error

## Getting Started
Add connectionString and databaseName Cosmos settings:
```
"Cosmos": {
    "ConnString": "<connectionString>",
    "Containers": [
      {
        "DatabaseId": "<databaseName>",
        "ContainerId": "Todo",
        "PkInfo": {
          "PartitionKey": "/listId",
          "Template": "{0}",
          "Pattern": "^-?\\d+$"
        }
      },
      {
        "DatabaseId": "<databaseName>",
        "ContainerId": "Post",
        "PkInfo": {
          "PartitionKey": "/partitionKey",
          "Template": "{0}:{1}",
          "Pattern": "^\\d+:\\d+$"
        }
      }
    ]
  }
```

Create item container "Todo":
```
{
  "id": "e4983838-dacb-4d48-bf73-08402383a208",
  "listId": "123",
  "title": "sunt aut facere repellat provident occaecati",
  "note": "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto",
  "reminder": "2024-01-10T15:00:00",
  "list": {
    "title": "Nostrum rerum est autem sunt rem eveniet",
    "items": []
  }
}
```
Query container "Todo":
```
select * from c where c.listId = '123' and c.id = 'e4983838-dacb-4d48-bf73-08402383a208'
```
Create item container "Post":
```
  {
    "id": "70b9f4fc-0542-4c55-aae8-256834a10126",
    "userId": 1,
    "categoryId": 10,
    "title": "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
    "body": "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"
  }
```

## Technologies
* [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* [Azure Cosmos DB .NET SDK v3 for API for NoSQL](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/sdk-dotnet-v3)

## Versions
The main branch is now on .NET 8.0.

## Learn more
[Partitioned Repository Pattern - Azure Cosmos DB Sample](https://arbems.com)