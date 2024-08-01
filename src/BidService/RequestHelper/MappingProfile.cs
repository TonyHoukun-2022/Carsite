using AutoMapper;
using Contracts;

namespace BidService;

public class MappingProfile : Profile
{
  public MappingProfile()
  {
    CreateMap<Bid, BidDto>();
    // for publish event to auction svc for updating currentHighBid prop in auction
    CreateMap<Bid, BidPlaced>();
  }
}
