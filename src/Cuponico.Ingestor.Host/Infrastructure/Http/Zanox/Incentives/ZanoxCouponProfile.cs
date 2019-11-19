using System;
using AutoMapper;
using Cuponico.Ingestor.Host.Domain.Tickets;
using Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Medias;
using Elevar.Utils;
using System.Linq;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Incentives
{
    public class ZanoxCouponProfile : Profile
    {
        public ZanoxCouponProfile()
        {
            CreateMap<ZanoxAdmediaProgram, Category>()
                .ForMember(dest => dest.Id, map => map.MapFrom(source => source.Id))
                .ForMember(dest => dest.Name, map => map.MapFrom(source => source.Name))
                .ForMember(dest => dest.FriendlyName, map => map.MapFrom(source => source.FriendlyName));

            CreateMap<ZanoxAdmediaProgram, Store>()
                .ForMember(dest => dest.Id, map => map.MapFrom(source => source.Id))
                .ForMember(dest => dest.Name, map => map.MapFrom(source => source.Name))
                .ForMember(dest => dest.FriendlyName, map => map.MapFrom(source => source.FriendlyName));

            CreateMap<IncentiveItem, Coupon>()
                .ForMember(dest => dest.CouponId, map => map.MapFrom(source => source.Id))
                .ForMember(dest => dest.Description, map => map.MapFrom(source => source.Admedia.Items.FirstOrDefault().Description))
                .ForMember(dest => dest.FriendlyDescription, map => map.MapFrom(source => source.Admedia.Items.FirstOrDefault().Description.ToFriendlyName()))
                //.ForMember(dest => dest.Remark, null)
                .ForMember(dest => dest.Discount, map => map.MapFrom(source => source.Percentage))
                .ForMember(dest => dest.Code, map => map.MapFrom(source => source.CouponCode))
                .ForMember(dest => dest.CouponLink, map => map.MapFrom(source => source.Admedia.Items.FirstOrDefault().Tracking.Url))
                .ForMember(dest => dest.Category, map => map.MapFrom(source => source.Admedia.Items.FirstOrDefault().Category))
                .ForMember(dest => dest.Store, map => map.MapFrom(source => source.Admedia.Items.FirstOrDefault().Program))
                .ForMember(dest => dest.New, map => map.MapFrom(source => IsNew(source.CreatedDate)))
                .ForMember(dest => dest.Validity, map => map.MapFrom(source => source.EndDate.ToUniversalTime()))
                .ForMember(dest => dest.IsPercentage, map => map.MapFrom(source => true))
                .ForMember(dest => dest.ChangedDate, map => map.MapFrom(source => source.ChangedDate.ToUniversalTime()));
        }

        private static bool IsNew(DateTime date)
        {
            var now = DateTime.Now;
            return now.Day == date.Day && now.Month == date.Month && now.Year == date.Year;
        }
    }
}
