using AuctionService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data;

// AuctionDbContext inherits from the DbContext class,
public class AuctionDbContext : DbContext
{
  // public constructor for AuctionDbContext class
  // call the base constructor of the DbContext class with the provided options
  public AuctionDbContext(DbContextOptions options) : base(options)
  {
  }

  // collections of auction entity in db
  public DbSet<Auction> Auctions { get; set; }
}
