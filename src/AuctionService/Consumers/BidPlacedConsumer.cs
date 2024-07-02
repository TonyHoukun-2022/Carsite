using AuctionService.Data;
using Contracts;
using MassTransit;

namespace AuctionService;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    private readonly AuctionDbContext _dbContext;

    public BidPlacedConsumer(AuctionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<BidPlaced> context)
    {

        var auction = await _dbContext.Auctions.FindAsync(context.Message.AuctionId);

        // 如果传入的消息中包含的出价状态为"Accepted"并且出价金额大于当前最高出价，或者当前还没有最高出价，则更新拍卖的当前最高出价。
        if (auction.CurrentHighBid == null 
            || context.Message.BidStatus.Contains("Accepted") 
            && context.Message.Amount > auction.CurrentHighBid)
        {
            auction.CurrentHighBid = context.Message.Amount;
            await _dbContext.SaveChangesAsync();
        }
    }
}