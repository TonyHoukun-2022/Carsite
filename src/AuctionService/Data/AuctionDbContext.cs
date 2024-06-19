using AuctionService.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.AddInboxStateEntity();

    modelBuilder.AddOutboxMessageEntity();

    modelBuilder.AddOutboxStateEntity();
  }
}
