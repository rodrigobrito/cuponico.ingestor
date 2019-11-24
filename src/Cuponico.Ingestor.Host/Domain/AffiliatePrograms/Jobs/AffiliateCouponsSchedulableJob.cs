using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Invocable;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets;
using Elevar.Collections;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Jobs
{
    public class AffiliateCouponsSchedulableJob : IInvocable
    {
        private readonly IAffiliateCouponRepository _repositoryFromPartner;
        private readonly IAffiliateCouponRepository _cuponicoRepository;
        private readonly IPublisher _publisher;

        public AffiliateCouponsSchedulableJob(IAffiliateCouponRepository repositoryFromPartner, IAffiliateCouponRepository cuponicoRepository, IPublisher publisher)
        {
            _repositoryFromPartner = repositoryFromPartner ?? throw new ArgumentNullException(nameof(repositoryFromPartner));
            _cuponicoRepository = cuponicoRepository ?? throw new ArgumentNullException(nameof(cuponicoRepository));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        public async Task Invoke()
        {
            var couponsFromPartner = await _repositoryFromPartner.GetAllAsync();
            if (!couponsFromPartner.Any()) return;

            var couponsToCreate = new List<AffiliateCoupon>();
            var couponsToChange = new List<AffiliateCoupon>();
            var couponsToCancel = new List<AffiliateCoupon>();

            var cuponicoCoupons = await _cuponicoRepository.GetAllAsync();
            foreach (var partnerCoupon in couponsFromPartner)
            {
                if (partnerCoupon == null) continue;

                var cuponicoCoupon = cuponicoCoupons?.FirstOrDefault(local => local.CouponId == partnerCoupon.CouponId);
                if (cuponicoCoupon == null)
                {
                    couponsToCreate.Add(partnerCoupon);
                }
                else
                {
                    if (!cuponicoCoupon.Equals(partnerCoupon))
                    {
                        couponsToChange.Add(partnerCoupon);
                    }
                }
            }

            if (cuponicoCoupons != null)
                couponsToCancel.AddRange(cuponicoCoupons.Where(localCoupon => couponsFromPartner.All(lomadee => lomadee.CouponId != localCoupon.CouponId)));

            if (couponsToCreate.Any())
            {
                var baches = couponsToCreate.BatchesOf(50);
                foreach (var bach in baches)
                {
                    var coupons = bach.ToList();
                    await _cuponicoRepository.SaveAsync(coupons);
                    var createdCoupons = AffiliateCouponCreated.CreateMany(coupons);
                    await _publisher.PublishAsync(createdCoupons);
                }
            }

            if (couponsToChange.Any())
            {
                var baches = couponsToChange.BatchesOf(50);
                foreach (var bach in baches)
                {
                    var coupons = bach.ToList();
                    await _cuponicoRepository.SaveAsync(coupons);
                    var changedCoupons = AffiliateCouponChanged.CreateMany(coupons);
                    await _publisher.PublishAsync(changedCoupons);
                }
            }

            if (couponsToCancel.Any())
            {
                var baches = couponsToCancel.BatchesOf(50);
                foreach (var bach in baches)
                {
                    var coupons = bach.ToList();
                    await _cuponicoRepository.DeleteAsync(coupons.Select(x => x.CouponId).ToList());
                    var canceledStores = AffiliateCouponCanceled.CreateMany(coupons);
                    await _publisher.PublishAsync(canceledStores);
                }
            }
        }
    }
}