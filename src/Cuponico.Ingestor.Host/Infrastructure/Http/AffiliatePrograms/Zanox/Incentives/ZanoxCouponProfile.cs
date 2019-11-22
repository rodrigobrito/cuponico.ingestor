using System;
using System.Linq;
using AutoMapper;
using Cuponico.Ingestor.Host.Domain;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets;
using Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Medias;
using Elevar.Utils;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Incentives
{
    public class ZanoxCouponProfile : Profile
    {
        public ZanoxCouponProfile()
        {
            CreateMap<ZanoxAdmediaProgram, AffiliateCategory>()
                .ForMember(dest => dest.Id, map => map.MapFrom(source => source.Id))
                .ForMember(dest => dest.Name, map => map.MapFrom(source => source.Name))
                .ForMember(dest => dest.FriendlyName, map => map.MapFrom(source => source.FriendlyName));

            CreateMap<ZanoxAdmediaProgram, AffiliateStore>()
                .ForMember(dest => dest.Id, map => map.MapFrom(source => source.Id))
                .ForMember(dest => dest.Name, map => map.MapFrom(source => source.Name))
                .ForMember(dest => dest.FriendlyName, map => map.MapFrom(source => source.FriendlyName));

            CreateMap<IncentiveItem, AffiliateCoupon>()
                .ForMember(dest => dest.CouponId, map => map.MapFrom(source => source.Id))
                .ForMember(dest => dest.AffiliateProgram, map => map.MapFrom(source => Affiliates.Zanox))
                .ForMember(dest => dest.Description, map => map.MapFrom(source => source.Caption))
                .ForMember(dest => dest.FriendlyDescription, map => map.MapFrom(source => source.Caption.ToFriendlyName()))
                .ForMember(dest => dest.Remark, map => map.MapFrom(source => source.Admedia.Items.FirstOrDefault().Description.Contains(source.Admedia.Items.FirstOrDefault().Instruction) ? source.Restrictions : source.Admedia.Items.FirstOrDefault().Instruction))
                .ForMember(dest => dest.Discount, map => map.MapFrom(source => source.Percentage > 0 ? source.Percentage : source.Total))
                .ForMember(dest => dest.Code, map => map.MapFrom(source => source.CouponCode))
                .ForMember(dest => dest.CouponLink, map => map.MapFrom(source => source.Admedia.Items.FirstOrDefault().Tracking.Url))
                .ForMember(dest => dest.Category, map => map.MapFrom(source => source.Admedia.Items.FirstOrDefault().Category))
                .ForMember(dest => dest.Store, map => map.MapFrom(source => source.Admedia.Items.FirstOrDefault().Program))
                .ForMember(dest => dest.New, map => map.MapFrom(source => IsNew(source.CreatedDate)))
                .ForMember(dest => dest.Validity, map => map.MapFrom(source => source.EndDate.ToUniversalTime()))
                .ForMember(dest => dest.IsPercentage, map => map.MapFrom(source => source.Percentage > 0))
                .ForMember(dest => dest.Shipping, map => map.MapFrom(source => source.Shipping))
                .ForMember(dest => dest.ChangedDate, map => map.MapFrom(source => source.ChangedDate.ToUniversalTime()));
        }

        private static bool IsNew(DateTime date)
        {
            var now = DateTime.Now;
            return now.Day == date.Day && now.Month == date.Month && now.Year == date.Year;
        }
    }
}
