using AutoMapper;
using Cuponico.Ingestor.Host.Partners.Coupons;
using Cuponico.Ingestor.Host.Partners.Lomadee.Coupons.Tickets;

namespace Cuponico.Ingestor.Host.Partners.Lomadee.Coupons
{
    public class LomadeeCouponProfile : Profile
    {
        public LomadeeCouponProfile()
        {
            CreateMap<LomadeeCategory, Category>()
                .ForMember(dest => dest.Id, map => map.MapFrom(source => source.Id))
                .ForMember(dest => dest.Name, map => map.MapFrom(source => source.Name));

            CreateMap<LomadeeStore, Store>()
                .ForMember(dest => dest.Id, map => map.MapFrom(source => source.Id))
                .ForMember(dest => dest.Name, map => map.MapFrom(source => source.Name))
                .ForMember(dest => dest.Link, map => map.MapFrom(source => source.Link))
                .ForMember(dest => dest.Image, map => map.MapFrom(source => source.Image));

            CreateMap<LomadeeCoupon, Coupon>()
                .ForMember(dest => dest.Id, map => map.MapFrom(source => source.Id))
                .ForMember(dest => dest.Description, map => map.MapFrom(source => source.Description))
                .ForMember(dest => dest.FriendlyDescription, map => map.MapFrom(source => source.FriendlyDescription))
                .ForMember(dest => dest.Remark, map => map.MapFrom(source => source.Remark))
                .ForMember(dest => dest.Discount, map => map.MapFrom(source => source.Discount))
                .ForMember(dest => dest.Code, map => map.MapFrom(source => source.Code))
                .ForMember(dest => dest.Link, map => map.MapFrom(source => source.Link))
                .ForMember(dest => dest.Category, map => map.MapFrom(source => source.Category))
                .ForMember(dest => dest.Store, map => map.MapFrom(source => source.Store))
                .ForMember(dest => dest.New, map => map.MapFrom(source => source.New))
                .ForMember(dest => dest.Vigency, map => map.MapFrom(source => source.Vigency));
        }
    }
}
