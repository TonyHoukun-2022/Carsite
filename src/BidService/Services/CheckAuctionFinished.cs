
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace BidService;

// background service in an ASP.NET Core application that periodically checks for auctions that have finished.  It then updates the auction status and publishes an event to notify other parts of the system.
public class CheckAuctionFinished : BackgroundService
{
  private readonly ILogger<CheckAuctionFinished> _logger;
  // IServiceProvider: Service provider for creating scopes and resolving services.
  private readonly IServiceProvider _service;
  public CheckAuctionFinished(ILogger<CheckAuctionFinished> logger, IServiceProvider service)
  {
      _service = service;
      _logger = logger;
    
  }
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    _logger.LogInformation("starting check for finished auctions");

    // Registers a callback to log when the service is stopping.
    stoppingToken.Register(() => _logger.LogInformation("==> auction check is stopping"));

    // Enters a loop that periodically checks for finished auctions every 5 seconds, unless the cancellation token is triggered.
    while (!stoppingToken.IsCancellationRequested)
    {
      await CheckAuctions(stoppingToken);

      await Task.Delay(5000, stoppingToken);
    }
  }

  private async Task CheckAuctions(CancellationToken stoppingToken)
  {
    // Queries the database for auctions that have finished but are not marked as finished.
    var finishedAuctions = await DB.Find<Auction>()
      .Match(a => a.AuctionEnd <= DateTime.UtcNow)
      .Match(x => !x.Finished)
      .ExecuteAsync(stoppingToken);

      if (finishedAuctions.Count == 0) return;

      _logger.LogInformation("==> found {count} auctions that have completed", finishedAuctions.Count);

      // Creates a new scope for service resolution.
      using var scope = _service.CreateScope();

      //Gets an IPublishEndpoint from the scope for publishing events.
      var endpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

      foreach (var auction in finishedAuctions)
      {
        // update auction.Finished and save
        auction.Finished = true;
        await auction.SaveAsync(null, stoppingToken);

        var winningBid = await DB.Find<Bid>()
          .Match(a => a.AuctionId == auction.ID)
          .Match(x => x.BidStatus == BidStatus.Accepted)
          .Sort(x => x.Descending(y => y.Amount))
          .ExecuteFirstAsync(stoppingToken);

        //Publishes an AuctionFinished event with the auction results.
        await endpoint.Publish(new AuctionFinished
        {
          ItemSold = winningBid != null,
          AuctionId = auction.ID,
          Winner = winningBid?.Bidder,
          Amount = winningBid?.Amount,
          Seller = auction.Seller
        }, stoppingToken);
      }


  }
}
