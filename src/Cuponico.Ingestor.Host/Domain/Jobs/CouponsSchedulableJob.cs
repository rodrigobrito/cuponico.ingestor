using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Invocable;
using Cuponico.Ingestor.Host.Domain.Tickets;

namespace Cuponico.Ingestor.Host.Domain.Jobs
{
    public class CouponsSchedulableJob : IInvocable
    {
        private readonly ICouponRepository _repositoryFromPartner;
        private readonly ICouponRepository _cuponicoRepository;
        //private readonly KafkaProducer<CouponKey, Coupon> _producer;

        public CouponsSchedulableJob(ICouponRepository repositoryFromPartner, ICouponRepository cuponicoRepository)
        {
            _repositoryFromPartner = repositoryFromPartner ?? throw new ArgumentNullException(nameof(repositoryFromPartner));
            _cuponicoRepository = cuponicoRepository ?? throw new ArgumentNullException(nameof(cuponicoRepository));
        }

        public async Task Invoke()
        {
            var couponsFromPartner = await _repositoryFromPartner.GetAllAsync();
            if (!couponsFromPartner.Any()) return;

            var couponsToCreate = new List<Coupon>();
            var couponsToChange = new List<Coupon>();
            var couponsToCancel = new List<Coupon>();

            var cuponicoCoupons = await _cuponicoRepository.GetAllAsync();
            foreach (var partnerCoupon in couponsFromPartner)
            {
                if (partnerCoupon == null) continue;

                var cuponicoCoupon = cuponicoCoupons?.FirstOrDefault(local => local.Id == partnerCoupon.Id);
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
                couponsToCancel.AddRange(cuponicoCoupons.Where(localCoupon => couponsFromPartner.All(lomadee => lomadee.Id != localCoupon.Id)));

            if (couponsToCreate.Any())
            {
                if (!HasDuplicateUrl(couponsToCreate))
                {
                    await _cuponicoRepository.SaveAsync(couponsToCreate);
                    //PublishChanges(Events.CouponCreated, couponsCreated);
                }
            }

            if (couponsToChange.Any())
            {
                if (!HasDuplicateUrl(couponsToChange))
                    await _cuponicoRepository.SaveAsync(couponsToChange);
            }

            if (couponsToCancel.Any())
                await _cuponicoRepository.DeleteAsync(couponsToCancel.Select(x => x.Id).ToList());
        }

        //private void PublishChanges(string eventName, IList<LomadeeCoupon> lomadeeCoupons)
        //{
        //    var coupons = _mapper.Map<IList<Coupon>>(lomadeeCoupons);
        //    var kvps = coupons.Select(c => new KeyValuePair<CouponKey, Coupon>(c.Key, c)).ToList();
        //    var batches = kvps.BatchesOf(1000).Select(c => c.ToList()).ToList();
        //    foreach (var batch in batches)
        //    {
        //        foreach (var keyValuePair in batch)
        //        {
        //            _producer.Send(eventName, keyValuePair.Key, keyValuePair.Value);
        //        }
        //        //_producer.Send(eventName, batch.ToList(), report => Console.WriteLine(report.ToString()));
        //    }
        //}

        private static bool HasDuplicateUrl(IEnumerable<Coupon> lomadeeCoupons)
        {
            return lomadeeCoupons.GroupBy(created => created.Link.ToString())
                                 .Select(link => link.Count()).Any(count => count > 2);
        }
    }
}