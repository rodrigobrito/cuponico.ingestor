using AutoMapper;
using Cuponico.Ingestor.Host.Domain.Stores;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Lomadee.Coupons.Stores
{
    public class LomadeeStoreProfile: Profile
    {
        public LomadeeStoreProfile()
        {
            CreateMap<LomadeeStore, Store>()
                .ForMember(dest => dest.StoreId, map => map.MapFrom(source => source.Id))
                .ForMember(dest => dest.Name, map => map.MapFrom(source => source.Name))
                .ForMember(dest => dest.FriendlyName, map => map.MapFrom(source => source.FriendlyName))
                .ForMember(dest => dest.StoreUrl, map => map.MapFrom(source => source.Link))
                .ForMember(dest => dest.ImageUrl, map => map.MapFrom(source => source.Image))
                .ForMember(dest => dest.CouponsCount, map => map.MapFrom(source => source.CouponsCount));
        }
    }
}
