using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionController: ControllerBase
{
  // dependency injection
  
  //This is a private field that holds an instance of IMapper from AutoMapper. It is used to map between domain models and DTOs.
  private readonly IMapper _mapper;
  private readonly AuctionDbContext _context;
  

  // constructor
  public AuctionController(AuctionDbContext context, IMapper mapper)
  { 
    // assigns the context parameter to the context field, allowing the controller to use the database context.
    _context = context;
    _mapper = mapper;
  }

  [HttpGet]
  public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
  {
    var auctions = await _context.Auctions
      // eager load of the related Item entity. Eager loading is used to load related entities as part of the initial query, reducing the number of queries sent to the database.
      .Include(auction => auction.Item)
      .OrderBy(auction => auction.Item.Make)
      // This executes the query asynchronously and returns the results as a list.
      .ToListAsync();

    // map auctions to list of auctionDtos
    return _mapper.Map<List<AuctionDto>>(auctions);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
  {
    var auction = await _context.Auctions
      .Include(auction => auction.Item)
      //  retrieves the first auction from the database where the ID matches the provided id
      .FirstOrDefaultAsync(auction => auction.Id == id);

    if (auction == null) return NotFound();

    return _mapper.Map<AuctionDto>(auction);
  }

  [HttpPost]
  public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
  {
    // map auctionDto to auction
    var auction = _mapper.Map<Auction>(auctionDto);
    //TODO: add current user as seller

    auction.Seller = "test";

    // add auction to context
    _context.Auctions.Add(auction);

    //save changes made in the context to db
    // If changes were successfully saved (i.e., the number of entries written is greater than 0), the condition evaluates to true.
    var result = await _context.SaveChangesAsync() > 0;

    if(!result) BadRequest("Could not save changes to the DB");

    //   method returns an HTTP 201 (Created) response to the client with the Location header set to the URI of the newly created resource and the specified content 
    return CreatedAtAction(
      nameof(GetAuctionById), 
      new {auction.Id},
      _mapper.Map<AuctionDto>(auction)
    );

  }

  [HttpPut("{id}")]
  public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
  {
    var auction = await _context.Auctions
      .Include(auction => auction.Item)
      .FirstOrDefaultAsync(auction => auction.Id == id);

    if (auction == null) return NotFound();

    //TODO: check seller == username

    auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
    auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
    auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
    auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
    auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

    var result = await _context.SaveChangesAsync() > 0;

    if (!result) return BadRequest("Problem when saving");

    return Ok(result);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult> DeleteAuction(Guid id)
  {
    var auction = await _context.Auctions.FindAsync(id);

    if (auction == null) return NotFound();

    //TODO: check seller == username

    _context.Auctions.Remove(auction);

    var result = await _context.SaveChangesAsync() > 0;

    if (!result) return BadRequest("Could not update db");

    return Ok();
  }
}
