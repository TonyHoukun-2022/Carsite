using AuctionService;
using Grpc.Net.Client;
using MongoDB.Driver.Core.WireProtocol.Messages;

namespace BidService.Services
{
    public class GrpcAuctionClient
    {   
      private readonly ILogger<GrpcAuctionClient> _logger;
      private readonly IConfiguration _config;
      public GrpcAuctionClient(ILogger<GrpcAuctionClient> logger, IConfiguration config)
      {
        _logger = logger;
        _config = config;
      }

      public Auction GetAuction(string id)
      {
        _logger.LogInformation("calling grpc server");
        
        var channel = GrpcChannel.ForAddress(_config["GrpcAuction"]);

        var client = new GrpcAuction.GrpcAuctionClient(channel);

        var req = new GetAuctionReq { Id = id };

        try
        {
          // receive res from grpc server side => Auction svc
          var res = client.GetAuction(req);

          var auction = new Auction
          {
           ID = res.Auction.Id,
           AuctionEnd = DateTime.Parse(res.Auction.AuctionEnd),
           Seller = res.Auction.Seller,
           ReservePrice = res.Auction.ReservePrice,
          };

          return auction;
        }
        catch (Exception e)
        {
          _logger.LogError(e, "Could not call grpc server");

          return null;        
        };
      }
    }
}