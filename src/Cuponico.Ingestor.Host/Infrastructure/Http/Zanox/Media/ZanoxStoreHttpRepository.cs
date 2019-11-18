using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Cuponico.Ingestor.Host.Domain.Stores;
using Elevar.Utils;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Media
{
    public class ZanoxStoreHttpRepository: IStoreRepository
    {
        private readonly HttpClient _client;
        private readonly ZanoxHttpSettings _zanoxSettings;
        private readonly IMapper _mapper;

        public ZanoxStoreHttpRepository(ZanoxHttpSettings zanoxSettings, HttpClient client, IMapper mapper)
        {
            _client = client.ThrowIfNull(nameof(client));
            _zanoxSettings = zanoxSettings.ThrowIfNull(nameof(zanoxSettings));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
        }

        private async Task<ZanoxAdmediaResponse> GetStartPageMediaAsync(int page = 0)
        {
            var responseString = await _client.GetStringAsync($"{_zanoxSettings.GetStartpageMediaUri}&items=50&page={page}");
            return JsonConvert.DeserializeObject<ZanoxAdmediaResponse>(responseString, _zanoxSettings.JsonSettings);
        }

        public async Task<IList<Store>> GetAllAsync()
        {
            var response = await GetAllZanoxMedia();
            return _mapper.Map<IList<Store>>(response.Admedium.Items);
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

        public Task SaveAsync(IList<Store> categories)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(IList<long> ids)
        {
            throw new NotImplementedException();
        }
    }
}