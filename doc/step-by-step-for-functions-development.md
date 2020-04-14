# Easy step by step for functions development
## 1. Requirements
- Visual Studio 2019 
  - feature develop Azure Function. (doc [en-us](https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs) / [ja-jp](https://docs.microsoft.com/ja-jp/azure/azure-functions/functions-develop-vs))
- Cosmos DB local emulator (doc [en-us](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator) / [ja-jp](https://docs.microsoft.com/ja-jp/azure/cosmos-db/local-emulator))
- Azure Storage emulator (doc [en-us](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator) / [ja-jp](https://docs.microsoft.com/ja-jp/azure/storage/common/storage-use-emulator))


## 2. clone 
any thing.

## 3. open solution using Visual studio
- Covid19Radar.Functions.sln

## 4. local.settings.json
- Copy from `local.settings.json.example` to `local.settings.json` 
- Edit configurations for local
```
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true", // Do not change. for Storage Emulator
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",                // Do not change.
        "COSMOS_ENDPOINT_URI": "https://localhost:8081",     // Do not change. for Cosmos DB Emulator
        "COSMOS_PRIMARY_KEY": "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                                                             // Do not change if not change key
        "COSMOS_DATABASE_ID": "Example",                     // Do not change.
        "COSMOS_AUTO_GENERATE": true                         // Do not change.
    }
}
```

## 4. build and Debug
