using AutoMapper;
using Cuponico.Ingestor.Host.Domain.Categories;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Lomadee.Coupons.Categories
{
    public class LomadeeCategoryProfile: Profile
    {
        public LomadeeCategoryProfile()
        {
            CreateMap<LomadeeCategory, Category>()
                .ForMember(dest => dest.CategoryId, map => map.MapFrom(source => source.Id))
                .ForMember(dest => dest.Name, map => map.MapFrom(source => source.Name))
                .ForMember(dest => dest.FriendlyName, map => map.MapFrom(source => source.FriendlyName))
                .ForMember(dest => dest.CategoryUrl, map => map.MapFrom(source => source.Link))
                .ForMember(dest => dest.CouponsCount, map => map.MapFrom(source => source.CouponsCount));
        }
    }
}
