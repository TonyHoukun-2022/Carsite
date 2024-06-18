using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;

namespace SearchService.Data;

public class DbInitializer
{
  public static async Task InitDb(WebApplication app)
  {
    await DB.InitAsync("SearchDb", MongoClientSettings
      .FromConnectionString(
        app.Configuration.GetConnectionString("MongoConnection")
    ));

//  index is created on multiple properties of the Item entity.
// DB.Index<Item>() => use index creation method for Item in DB context
    await DB.Index<Item>()
      // Make prop indexed, index type is Text
      .Key(item => item.Make, KeyType.Text)
      .Key(item => item.Model, KeyType.Text)
      .Key(item => item.Color, KeyType.Text)
      .CreateAsync();

      // count Item entities 
      // var count = await DB.CountAsync<Item>();

      using var scope = app.Services.CreateScope();

      var httpClient = scope.ServiceProvider.GetRequiredService<AuctionServiceHttpClient>();

      var items = await httpClient.GetItemFromSearchDb();

      // items.Count retrieves the number of elements in the items collection.
      Console.WriteLine(items.Count + "returned from the auction service");

      if (items.Count > 0) await DB.SaveAsync(items);

      // if (count == 0)
      // {
      //   Console.WriteLine("No data - seed data now");

      //   // This line reads the entire content of the file auctions.json located in the Data directory.
      //   // ReadAllTextAsync is an asynchronous method that reads the content of a file as a string.
      //   var itemData = await File.ReadAllTextAsync("Data/auctions.json");

      //   // creates an instance of JsonSerializerOptions and configures it.
      //   //  JSON property names will be matched to class property names in a case-insensitive manne
      //   var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};

      //   // converts a JSON string into an object or a collection of objects.
      //   // eserializes the JSON string itemData into a list of Item objects.
      //   var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);

      //   await DB.SaveAsync(items);
      // }
  }
}
