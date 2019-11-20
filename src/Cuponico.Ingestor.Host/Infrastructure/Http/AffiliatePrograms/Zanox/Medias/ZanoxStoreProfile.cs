using AutoMapper;
using Cuponico.Ingestor.Host.Domain;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Medias
{
    public class ZanoxStoreProfile: Profile
    {
        public ZanoxStoreProfile()
        {
            CreateMap<ZanoxAdmedia, AffiliateStore>()
                .ForMember(dest => dest.StoreId, map => map.MapFrom(source => source.Program.Id))
                .ForMember(dest => dest.AffiliateProgram, map => map.MapFrom(source => Affiliates.Zanox))
                .ForMember(dest => dest.Name, map => map.MapFrom(source => source.Program.Name))
                .ForMember(dest => dest.FriendlyName, map => map.MapFrom(source => source.Program.FriendlyName))
                .ForMember(dest => dest.Description, map => map.MapFrom(source => source.Program.Description))
                .ForMember(dest => dest.StoreUrl, map => map.MapFrom(source => source.Tracking.Url))
                .ForMember(dest => dest.ImageUrl, map => map.MapFrom(source => source.Program.ImageUri));
        }
    }
}