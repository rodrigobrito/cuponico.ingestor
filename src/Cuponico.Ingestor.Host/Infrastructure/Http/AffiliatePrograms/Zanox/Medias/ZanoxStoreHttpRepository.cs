using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets;
using Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Programs;
using Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Zanox;
using Cuponico.Ingestor.Host.Infrastructure.Settings.Zanox;
using Elevar.Utils;
using Newtonsoft.Json;
using AffiliateStore = Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores.AffiliateStore;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Medias
{
    public class ZanoxStoreHttpRepository : IAffiliateStoreRepository
    {
        private readonly HttpClient _client;
        private readonly ZanoxHttpSettings _zanoxSettings;
        private readonly ZanoxProgramHttpRepository _programRepository;
        private readonly IAffiliateCouponRepository _couponRepository;
        private readonly IMapper _mapper;

        public ZanoxStoreHttpRepository(ZanoxHttpSettings zanoxSettings, HttpClient client, IMapper mapper, ZanoxProgramHttpRepository programRepository, ZanoxCouponMongoDbRepository couponRepository)
        {
            _client = client.ThrowIfNull(nameof(client));
            _zanoxSettings = zanoxSettings.ThrowIfNull(nameof(zanoxSettings));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
            _programRepository = programRepository.ThrowIfNull(nameof(programRepository));
            _couponRepository = couponRepository.ThrowIfNull(nameof(ZanoxCouponMongoDbRepository));
        }

        private async Task<ZanoxAdmediaResponse> GetStartPageMediaAsync(int page = 0)
        {
            var responseString = await _client.GetStringAsync($"{_zanoxSettings.GetStoresUri}&items=50&page={page}");
            return JsonConvert.DeserializeObject<ZanoxAdmediaResponse>(responseString, _zanoxSettings.JsonSettings);
        }

        public async Task<IList<AffiliateStore>> GetAllAsync()
        {
            var response = await GetAllZanoxMedia();
            UpdateProperties(response);
            var stores = _mapper.Map<IList<AffiliateStore>>(response.Admedium.Items);
            var coupons = await _couponRepository.GetAllAsync();
            foreach (var store in stores)
            {
                store.CouponsCount = coupons.Count(c => c.Store != null && c.Store.Id == store.StoreId);
            }
            return stores;
        }

        private async Task<ZanoxAdmediaResponse> GetAllZanoxMedia()
        {
            var page = 0;
            var response = await GetStartPageMediaAsync(page);
            while (response.Items > 0)
            {
                page++;
                var moreStores = await GetStartPageMediaAsync(page);
                if (moreStores.Admedium != null)
                {
                    foreach (var admediumItem in moreStores.Admedium.Items)
                    {
                        response.Admedium.Items.Add(admediumItem);
                    }
                }
                response.Items = moreStores.Items;
            }
            return response;
        }

        private void UpdateProperties(ZanoxAdmediaResponse media)
        {
            if (media?.Admedium?.Items != null)
            {
                Parallel.ForEach(media.Admedium.Items, new ParallelOptions { MaxDegreeOfParallelism = 20 },  admediumItem =>
                {
                    try
                    {
                        var response = _programRepository.GetProgramAsync(admediumItem.Program.Id.ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
                        var program = response?.Programs?.FirstOrDefault();
                        if (program == null) return;

                        admediumItem.Program.Description = program.Description; //program.DescriptionLocal.Replace("<![CDATA[", string.Empty).Replace("]]>", string.Empty);
                        if (program.Image != null)
                        {
                            var image = program.Image.ToString().ToLower().Replace("http://", "https://");
                            admediumItem.Program.ImageUri = new Uri(image);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.InnerException);
                    }
                });
            }
        }

        public Task SaveAsync(IList<AffiliateStore> categories)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(IList<long> ids)
        {
            throw new NotImplementedException();
        }
    }
}