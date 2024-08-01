using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace BidService;

// class AuctionCreatedConsumer that implements the IConsumer<AuctionCreated> interface. The purpose of this class is to handle messages of type AuctionCreated (defined in Contracts), which are consumed from a message broker.
public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
  // will be called when a message of type AuctionCreated is received
  public async Task Consume(ConsumeContext<AuctionCreated> context)
  {
    var auction = new Auction{
      ID = context.Message.Id.ToString(),
      Seller = context.Message.Seller,
      AuctionEnd = context.Message.AuctionEnd,
      ReservePrice = context.Message.ReservePrice,
    };

    await auction.SaveAsync();
  }
}
