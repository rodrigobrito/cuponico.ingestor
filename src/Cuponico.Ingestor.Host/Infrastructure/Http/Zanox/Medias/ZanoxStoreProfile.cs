using AutoMapper;
using Cuponico.Ingestor.Host.Domain.Stores;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Medias
{
    public class ZanoxStoreProfile: Profile
    {
        public ZanoxStoreProfile()
        {
            CreateMap<ZanoxAdmedia, Store>()
                .ForMember(dest => dest.StoreId, map => map.MapFrom(source => source.Program.Id))
                .ForMember(dest => dest.Name, map => map.MapFrom(source => source.Program.Name))
                .ForMember(dest => dest.FriendlyName, map => map.MapFrom(source => source.Program.FriendlyName))
                .ForMember(dest => dest.Description, map => map.MapFrom(source => source.Program.Description))
                .ForMember(dest => dest.StoreUrl, map => map.MapFrom(source => source.Tracking.Url))
                .ForMember(dest => dest.ImageUrl, map => map.MapFrom(source => source.Program.ImageUri));
        }
    }
}