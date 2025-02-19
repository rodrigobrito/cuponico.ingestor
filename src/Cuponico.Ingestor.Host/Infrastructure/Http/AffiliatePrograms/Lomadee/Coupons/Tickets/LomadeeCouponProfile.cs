﻿using AutoMapper;
using Cuponico.Ingestor.Host.Domain;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Lomadee.Coupons.Tickets
{
    public class LomadeeCouponProfile : Profile
    {
        public LomadeeCouponProfile()
        {
            CreateMap<LomadeeCategory, AffiliateCategory>()
                .ForMember(dest => dest.Id, map => map.MapFrom(source => source.Id))
                .ForMember(dest => dest.Name, map => map.MapFrom(source => source.Name))
                .ForMember(dest => dest.FriendlyName, map => map.MapFrom(source => source.FriendlyName));

            CreateMap<LomadeeStore, AffiliateStore>()
                .ForMember(dest => dest.Id, map => map.MapFrom(source => source.Id))
                .ForMember(dest => dest.Name, map => map.MapFrom(source => source.Name))
                .ForMember(dest => dest.FriendlyName, map => map.MapFrom(source => source.FriendlyName))
                .ForMember(dest => dest.StoreUrl, map => map.MapFrom(source => source.Link))
                .ForMember(dest => dest.ImageUrl, map => map.MapFrom(source => source.Image));

            CreateMap<LomadeeCoupon, AffiliateCoupon>()
                .ForMember(dest => dest.CouponId, map => map.MapFrom(source => source.Id))
                .ForMember(dest => dest.AffiliateProgram, map => map.MapFrom(source => Affiliates.Lomadee))
                .ForMember(dest => dest.Description, map => map.MapFrom(source => source.Description))
                .ForMember(dest => dest.FriendlyDescription, map => map.MapFrom(source => source.FriendlyDescription))
                .ForMember(dest => dest.Remark, map => map.MapFrom(source => source.Remark))
                .ForMember(dest => dest.Discount, map => map.MapFrom(source => source.Discount))
                .ForMember(dest => dest.Code, map => map.MapFrom(source => source.Code))
                .ForMember(dest => dest.CouponLink, map => map.MapFrom(source => source.Link))
                .ForMember(dest => dest.Category, map => map.MapFrom(source => source.Category))
                .ForMember(dest => dest.Store, map => map.MapFrom(source => source.Store))
                .ForMember(dest => dest.New, map => map.MapFrom(source => source.New))
                .ForMember(dest => dest.Validity, map => map.MapFrom(source => source.Vigency));
        }
    }
}
