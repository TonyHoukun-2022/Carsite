using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
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
  private readonly IPublishEndpoint _publishEndpoint;
  

  // constructor
  public AuctionController(AuctionDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint)
  { 
    // assigns the context parameter to the context field, allowing the controller to use the database context.
    _context = context;
    _mapper = mapper;
    _publishEndpoint = publishEndpoint;
  }

  [HttpGet]
  public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)
  {
    //  query against the Auctions table, ordering the results by the Make property of the associated Item. The query is converted to an IQueryable to enable further querying.
    var query = _context.Auctions.OrderBy(a => a.Item.Make).AsQueryable();

    if (!string.IsNullOrEmpty(date))
    {
      // include only auctions updated after the specified date
      query = query.Where(a => a.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
    }

    // rojects the Auction entities to AuctionDto objects using AutoMapper and executes the query asynchronously, returning the results as a list. The ProjectTo method uses the AutoMapper configuration to perform the projection.
    return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
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

    var auctionToBePublished = _mapper.Map<AuctionDto>(auction);

    // publish a msg of type auctioncreated to all subscribed consumers into service bus
    await _publishEndpoint.Publish(
      _mapper.Map<AuctionCreated>(auctionToBePublished)
    );

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

    // publish auctionupdated type of message to service bus
    await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

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

    await _publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });

    var result = await _context.SaveChangesAsync() > 0;

    if (!result) return BadRequest("Could not update db");

    return Ok();
  }
}
