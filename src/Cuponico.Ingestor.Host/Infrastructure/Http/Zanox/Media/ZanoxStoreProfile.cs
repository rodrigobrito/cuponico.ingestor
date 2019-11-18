using AutoMapper;
using Cuponico.Ingestor.Host.Domain.Stores;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Media
{
    public class ZanoxStoreProfile: Profile
    {
        public ZanoxStoreProfile()
        {
            CreateMap<ZanoxAdmedia, Store>()
                .ForMember(dest => dest.StoreId, map => map.MapFrom(source => source.Program.Id))
                .ForMember(dest => dest.Name, map => map.MapFrom(source => source.Program.Name))
                .ForMember(dest => dest.FriendlyName, map => map.MapFrom(source => source.Program.FriendelyName))
                .ForMember(dest => dest.StoreUrl, map => map.MapFrom(source => source.Tracking.StoreUrl))
                .ForMember(dest => dest.ImageUrl, map => map.MapFrom(source => source.Tracking.ImageUrl));
        }
    }
}