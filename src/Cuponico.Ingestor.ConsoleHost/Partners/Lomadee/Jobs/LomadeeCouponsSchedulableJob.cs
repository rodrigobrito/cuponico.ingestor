using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Invocable;
using Cuponico.Ingestor.Host.Partners.Lomadee.Coupons.Tickets;

namespace Cuponico.Ingestor.Host.Partners.Lomadee.Jobs
{
    public class LomadeeCouponsSchedulableJob : IInvocable
    {
        private readonly LomadeeeCouponHttpRepository _httpRepository;
        private readonly LomadeeCouponMongoDbRepository _mongodbRepository;
        public LomadeeCouponsSchedulableJob(LomadeeeCouponHttpRepository httpRepository, LomadeeCouponMongoDbRepository mongodbRepository)
        {
            _httpRepository = httpRepository ?? throw new ArgumentNullException(nameof(httpRepository));
            _mongodbRepository = mongodbRepository ?? throw new ArgumentNullException(nameof(mongodbRepository));
        }

        public async Task Invoke()
        {
            var lomadeeCoupons = await _httpRepository.GetAllAsync();
            if (!lomadeeCoupons.Any()) return;

            foreach (var coupon in lomadeeCoupons)
                coupon.Vigency = coupon.Vigency.ToUniversalTime();

            var couponsToInsert = new List<LomadeeCoupon>();
            var couponsToUpdate = new List<LomadeeCoupon>();
            var couponsToDelete = new List<LomadeeCoupon>();

            var localCoupons = await _mongodbRepository.GetAll();
            foreach (var lomadeeCoupon in lomadeeCoupons)
            {
                if (lomadeeCoupon == null) continue;

                lomadeeCoupon.UpdateProperties();

                var localCoupon = localCoupons?.FirstOrDefault(local => local.Id == lomadeeCoupon.Id);
                if (localCoupon == null)
                {
                    couponsToInsert.Add(lomadeeCoupon);
                }
                else
                {
                    if (!localCoupon.Equals(lomadeeCoupon))
                    {
                        couponsToUpdate.Add(lomadeeCoupon);
                    }
                }
            }

            if (localCoupons != null)
                couponsToDelete.AddRange(localCoupons.Where(localCoupon => lomadeeCoupons.All(lomadee => lomadee.Id != localCoupon.Id)));

            if (couponsToInsert.Any())
                await _mongodbRepository.SaveAsync(couponsToInsert);

            if (couponsToUpdate.Any())
                await _mongodbRepository.SaveAsync(couponsToUpdate);

            if (couponsToDelete.Any())
                await _mongodbRepository.DeleteAsync(couponsToDelete.Select(x => x.Id).ToList());
        }
    }
}
