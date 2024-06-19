using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;

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

    // map all props from dto to the property Item of auction class.
    // MapFrom(s => s) part indicates that the Item property of the Auction should be mapped from the entire CreateAuctionDto instance. 
    CreateMap<CreateAuctionDto, Auction>()
      .ForMember(auction => auction.Item, options => options.MapFrom(s => s));
    CreateMap<CreateAuctionDto, Item>();
    
    CreateMap<AuctionDto, AuctionCreated>();

    CreateMap<Auction, AuctionUpdated>().IncludeMembers(auction => auction.Item);
    CreateMap<Item, AuctionUpdated>();
  }
}
