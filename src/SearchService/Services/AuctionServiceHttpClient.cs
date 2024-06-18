using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services;

public class AuctionServiceHttpClient
{ 
  // declare two private readonly fields, _httpClient and _config, to hold the instances of HttpClient and IConfiguration, respectively. These fields are marked as readonly, meaning they can only be assigned during the declaration or within the constructor, ensuring their values do not change throughout the lifetime of the instance.
  private readonly HttpClient _httpClient;
  private readonly IConfiguration _config;
  public AuctionServiceHttpClient(HttpClient httpClient, IConfiguration config)
  {
    // instances are assigned to the private fields _httpClient and _config, respectively. This allows these services to be used throughout the class.
    _httpClient = httpClient;
    _config = config;
  }

  public async Task<List<Item>> GetItemFromSearchDb()
  {
    // return a string value representing the UpdatedAt timestamp of the most recently updated item.
    // // 'string' specifies the type of the projection result, which in this case will be the UpdatedAt field converted to a string.
    var lastUpdated= await DB.Find<Item, string>()
      .Sort(i => i.Descending(i => i.UpdatedAt))
      // The Project method is used to transform the result of the query.
       // only the UpdatedAt field should be included in the result, and it should be converted to a string.
      .Project(i => i.UpdatedAt.ToString())
      // the first result will be the item with the latest UpdatedAt value.
      .ExecuteFirstAsync();
    
    // makes an asynchronous GET request to the constructed URL and expects the response to be JSON
    //  fetches a list of Item objects from a specified URL using an HttpClient. 
    return await _httpClient.GetFromJsonAsync<List<Item>>(_config["AuctionServiceUrl"] + "/api/auctions?date=" + lastUpdated);
  }
}
