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

    // sorts the query results based on the Make property of the Item objects.
    query.Sort(item => item.Ascending(item => item.Make));

    if (!string.IsNullOrEmpty(searchParams.SearchTerm))
    { 
      // Match(Search.Full, searchTerm) filters the results to match the full-text search term.
      // SortByTextScore() sorts the results based on their relevance to the search term, ensuring the most relevant items appear first.
      query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();
    }

    query = searchParams.OrderBy switch
    {
      "make" => query.Sort(item => item.Ascending(item => item.Make)),
      "new" => query.Sort(item => item.Descending(item => item.CreatedAt)),
      // default sorting
      _ => query.Sort(item => item.Ascending(item => item.AuctionEnd))
    };

    query = searchParams.FilterBy switch
    {
      "finished" => query.Match(i => i.AuctionEnd < DateTime.UtcNow),
      "endingSoon" => query.Match(i => i.AuctionEnd < DateTime.UtcNow.AddHours(6)
       && i.AuctionEnd > DateTime.UtcNow),
      _ => query.Match(i => i.AuctionEnd > DateTime.UtcNow)

    };

    if (!string.IsNullOrEmpty(searchParams.Seller))
    {
      query.Match(i => i.Seller == searchParams.Seller);
    }

     if (!string.IsNullOrEmpty(searchParams.Winner))
    {
      query.Match(i => i.Winner == searchParams.Winner);
    }

    // sets the current page number for the paged search.
    query.PageNumber(searchParams.PageNumber);
    query.PageSize(searchParams.PageSize);

    var result = await query.ExecuteAsync();

    return Ok( new 
    {
      results = result.Results,
      pageCount = result.PageCount,
      totalCount = result.TotalCount
    });
  }
}
