using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Hubs;

namespace NotificationService.Consumers
{
  public class BidPlacedConsumer : IConsumer<BidPlaced>
  {
    private readonly IHubContext<NotificationHub> _hubContext;
    public BidPlacedConsumer(IHubContext<NotificationHub> hubContext)
    {
      _hubContext = hubContext;
    }
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
      Console.WriteLine("--> Bid placed message received");
      //send bidplaced message to client side
      await _hubContext.Clients.All.SendAsync("BidPlaced", context.Message);
    }
  }
}
