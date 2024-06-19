using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService;

// defines a consumer class AuctionCreatedConsumer that implements the IConsumer<T> interface from MassTransit, where T is the message type AuctionCreated. This class is responsible for handling messages of type AuctionCreated.
public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
  // private readonly field to hold the AutoMapper instance, 
  private readonly IMapper _mapper;
  //assign automapper instance mapper to private field
  public AuctionCreatedConsumer(IMapper mapper)
  {
    _mapper = mapper;
  }
  // consume method: get called when a msg of type auctioncreated is received
  public async Task Consume(ConsumeContext<AuctionCreated> context)
  {
    Console.WriteLine("--> Consuming auction created: " + context.Message.Id);

    // map 'auctionCreated' message to Item entity
    var item = _mapper.Map<Item>(context.Message);

    // save to db
    await item.SaveAsync();
  }
}
