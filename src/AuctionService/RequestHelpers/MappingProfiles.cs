using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;

namespace AuctionService.RequestHelpers;

public class MappingProfiles : Profile
{
  public MappingProfiles()
  {
    // mapping btw Auction and AuctionDto
    // IncludeMembers => include props from Item obj when mapping to AuctionDto
    CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Item);
    //  ensures that properties of Item can be mapped directly to properties of AuctionDto.
    CreateMap<Item, AuctionDto>();

    // map all properties of a CreateAuctionDto object to the Item property of an Auction object. This is useful when the CreateAuctionDto contains fields that should populate the Item object within an Auction.
    // auction => auction.Item => specifies destination object that is being configured.
    // options => options.MapFrom(s => s)
    // specifies the source of the value that will be mapped to the destination member.
    CreateMap<CreateAuctionDto, Auction>()
      .ForMember(auction => auction.Item, options => options.MapFrom(s => s));
    CreateMap<CreateAuctionDto, Item>();
  }
}
