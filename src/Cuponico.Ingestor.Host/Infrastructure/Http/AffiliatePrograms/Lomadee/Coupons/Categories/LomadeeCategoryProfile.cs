using AutoMapper;
using Cuponico.Ingestor.Host.Domain;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Lomadee.Coupons.Categories
{
    public class LomadeeCategoryProfile: Profile
    {
        public LomadeeCategoryProfile()
        {
            CreateMap<LomadeeCategory, AffiliateCategory>()
                .ForMember(dest => dest.CategoryId, map => map.MapFrom(source => source.Id))
                .ForMember(dest => dest.AffiliateProgram, map => map.MapFrom(source => Affiliates.Lomadee))
                .ForMember(dest => dest.Name, map => map.MapFrom(source => source.Name))
                .ForMember(dest => dest.FriendlyName, map => map.MapFrom(source => source.FriendlyName))
                .ForMember(dest => dest.CategoryUrl, map => map.MapFrom(source => source.Link))
                .ForMember(dest => dest.CouponsCount, map => map.MapFrom(source => source.CouponsCount));
        }
    }
}
