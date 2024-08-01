using AuctionService.Data;
using Grpc.Core;

namespace AuctionService;

public class GrpcAuctionService: GrpcAuction.GrpcAuctionBase
{
  private readonly AuctionDbContext _dbContext;
  public GrpcAuctionService (AuctionDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public override async Task<GrpcAuctionRes> GetAuction(GetAuctionReq req, ServerCallContext context)
  {
    Console.WriteLine("--> received grpc req for auction");

    var auction = await _dbContext.Auctions.FindAsync(Guid.Parse(req.Id)) 
      ?? throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
      
    var res = new GrpcAuctionRes
    {
      Auction = new GrpcAuctionModel
      {
        AuctionEnd = auction.AuctionEnd.ToString(),
        Id = auction.Id.ToString(),
        ReservePrice = auction.ReservePrice,
        Seller = auction.Seller,
      }
    };

    return res;
  }
}