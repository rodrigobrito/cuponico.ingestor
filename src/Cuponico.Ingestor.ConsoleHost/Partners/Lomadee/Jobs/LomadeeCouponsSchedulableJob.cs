using AutoMapper;
using Coravel.Invocable;
using Cuponico.Ingestor.Host.Kafka;
using Cuponico.Ingestor.Host.Partners.Coupons;
using Cuponico.Ingestor.Host.Partners.Lomadee.Coupons.Tickets;
using Elevar.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cuponico.Ingestor.Host.Partners.Lomadee.Jobs
{
    public class LomadeeCouponsSchedulableJob : IInvocable
    {
        private readonly LomadeeeCouponHttpRepository _httpRepository;
        private readonly LomadeeCouponMongoDbRepository _mongodbRepository;
        private readonly IMapper _mapper;
        private readonly KafkaProducer<CouponKey, Coupon> _producer;

        public LomadeeCouponsSchedulableJob(LomadeeeCouponHttpRepository httpRepository, LomadeeCouponMongoDbRepository mongodbRepository, IMapper mapper, KafkaProducer<CouponKey, Coupon> producer)
        {
            _httpRepository = httpRepository ?? throw new ArgumentNullException(nameof(httpRepository));
            _mongodbRepository = mongodbRepository ?? throw new ArgumentNullException(nameof(mongodbRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _producer = producer ?? throw new ArgumentNullException(nameof(producer));
        }

        public async Task Invoke()
        {
            var lomadeeCoupons = await _httpRepository.GetAllAsync();
            if (!lomadeeCoupons.Any()) return;

            foreach (var coupon in lomadeeCoupons)
                coupon.Vigency = coupon.Vigency.ToUniversalTime();

            var couponsCreated = new List<LomadeeCoupon>();
            var couponsChanged = new List<LomadeeCoupon>();
            var couponsCanceled = new List<LomadeeCoupon>();

            var localCoupons = await _mongodbRepository.GetAll();
            foreach (var lomadeeCoupon in lomadeeCoupons)
            {
                if (lomadeeCoupon == null) continue;

                lomadeeCoupon.UpdateProperties();

                var localCoupon = localCoupons?.FirstOrDefault(local => local.Id == lomadeeCoupon.Id);
                if (localCoupon == null)
                {
                    couponsCreated.Add(lomadeeCoupon);
                }
                else
                {
                    if (!localCoupon.Equals(lomadeeCoupon))
                    {
                        couponsChanged.Add(lomadeeCoupon);
                    }
                }
            }

            if (localCoupons != null)
                couponsCanceled.AddRange(localCoupons.Where(localCoupon => lomadeeCoupons.All(lomadee => lomadee.Id != localCoupon.Id)));

            if (couponsCreated.Any())
            {
                await _mongodbRepository.SaveAsync(couponsCreated);
                PublishChanges(Events.CouponCreated, couponsCreated);
            }

            if (couponsChanged.Any())
                await _mongodbRepository.SaveAsync(couponsChanged);

            if (couponsCanceled.Any())
                await _mongodbRepository.DeleteAsync(couponsCanceled.Select(x => x.Id).ToList());
        }

        private void PublishChanges(string eventName, IList<LomadeeCoupon> lomadeeCoupons)
        {
            var coupons = _mapper.Map<IList<Coupon>>(lomadeeCoupons);
            var kvps = coupons.Select(c => new KeyValuePair<CouponKey, Coupon>(c.Key, c)).ToList();
            var batches = kvps.BatchesOf(1000).Select(c => c.ToList()).ToList();
            foreach (var batch in batches)
            {
                foreach (var keyValuePair in batch)
                {
                    _producer.Send(eventName, keyValuePair.Key, keyValuePair.Value);
                }
                //_producer.Send(eventName, batch.ToList(), report => Console.WriteLine(report.ToString()));
            }
        }
    }
}