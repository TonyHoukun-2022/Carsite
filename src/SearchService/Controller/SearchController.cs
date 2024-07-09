using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelpers;
using ZstdSharp.Unsafe;

namespace SearchService;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
  [HttpGet]
  public async Task<ActionResult<List<Item>>> SearchItem([FromQuery]SearchParams searchParams)
  {
    // initializes a paged search query for the Item collection.
    // query is targeting Item collection
    var query = DB.PagedSearch<Item, Item>();

    query.Sort(item => item.Ascending(item => item.Make));

    if (!string.IsNullOrEmpty(searchParams.SearchTerm))
    { 
      query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();
    }
    
    /** query = searchParams.OrderBy switch
    //     {
    //         "make" => query.Sort(x => x.Ascending(a => a.Make)).Sort(x => x.Ascending(a => a.Model)),
    //         "new" => query.Sort(x => x.Descending(a => a.CreatedAt)),
    //         "endingSoon" => query.Sort(x => x.Ascending(a => a.AuctionEnd)),
    //         _ => query // 默认情况下不进行额外排序
    }; **/

    switch (searchParams.FilterBy)
    {
      case "finished":
          query.Match(x => x.AuctionEnd < DateTime.UtcNow);
          break;
      case "endingSoon":
          query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6) && x.AuctionEnd > DateTime.UtcNow);
          break;
      default:
          query.Match(x => x.AuctionEnd > DateTime.UtcNow);
          break;
    }

    if (!string.IsNullOrEmpty(searchParams.Seller))
    {
      query.Match(i => i.Seller == searchParams.Seller);
    }

     if (!string.IsNullOrEmpty(searchParams.Winner))
    {
      query.Match(i => i.Winner == searchParams.Winner);
    }

    /** sets the current page number for the paged search.
    // query.PageNumber(searchParams.PageNumber);
    query.PageSize(searchParams.PageSize); **/

    var result = await query.ExecuteAsync();

    var auctionItems = result.Results.ToList();

    // sorting after fetch data
    IEnumerable<Item> sortedItems = auctionItems;
    if (!string.IsNullOrEmpty(searchParams.OrderBy)) {
      sortedItems =searchParams.OrderBy switch
      {
        "make" => auctionItems.OrderBy(x => x.Make).ThenBy(x => x.Model),
        "new" => auctionItems.OrderByDescending(x => x.CreatedAt),
        "endingSoon" => auctionItems.OrderBy(x => x.AuctionEnd),
         _ => auctionItems
      };
    }

    // in-memory pagination
    var paginatedItems = sortedItems
      .Skip((searchParams.PageNumber - 1) * searchParams.PageSize)
      .Take(searchParams.PageSize)
      .ToList();

    var totalCount = sortedItems.Count();
    var pageCount = (int)Math.Ceiling(totalCount / (double)searchParams.PageSize);


    return Ok( new 
    {
      results = paginatedItems,
      pageCount,
      totalCount
    });
  }
}
