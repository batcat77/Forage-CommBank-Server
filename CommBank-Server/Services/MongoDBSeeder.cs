using MongoDB.Driver;
using CommBank.Models;
using System.Text.Json;

public class MongoDbSeeder
{
    private readonly IMongoDatabase _db;

    public MongoDbSeeder(IMongoDatabase mongoDatabase)
    {
        _db = mongoDatabase;
    }

    public async Task SeedAsync()
    {
        await SeedCollectionAsync<User>("Users", "Users.json");
        await SeedCollectionAsync<Goal>("Goals", "Goals.json");
        await SeedCollectionAsync<Account>("Accounts", "Accounts.json");
        await SeedCollectionAsync<Transaction>("Transactions", "Transactions.json");
        await SeedCollectionAsync<CommBank.Models.Tag>("Tags", "Tags.json");
    }

    private async Task SeedCollectionAsync<T>(string collectionName, string jsonFileName)
    {
        var collection = _db.GetCollection<T>(collectionName);

        var hasAny = await collection.Find(_ => true).AnyAsync();
        if (hasAny)
        {
            Console.WriteLine($"{collectionName} already seeded.");
            return;
        }

        var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "SeedData", jsonFileName);
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"JSON file not found: {jsonPath}");
            return;
        }

        var json = await File.ReadAllTextAsync(jsonPath);
        var items = JsonSerializer.Deserialize<List<T>>(json);

        if (items != null && items.Any())
        {
            await collection.InsertManyAsync(items);
            Console.WriteLine($" Seeded {collectionName} collection.");
        }
        else
        {
            Console.WriteLine($"No valid data found in {jsonPath}");
        }
    }
}