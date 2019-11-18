using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Cuponico.Ingestor.Host.Domain.Stores;
using Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Programs;
using Elevar.Utils;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Medias
{
    public class ZanoxStoreHttpRepository : IStoreRepository
    {
        private readonly HttpClient _client;
        private readonly ZanoxHttpSettings _zanoxSettings;
        private readonly ZanoxProgramHttpRepository _programRepository;
        private readonly IMapper _mapper;

        public ZanoxStoreHttpRepository(ZanoxHttpSettings zanoxSettings, HttpClient client, IMapper mapper, ZanoxProgramHttpRepository programRepository)
        {
            _client = client.ThrowIfNull(nameof(client));
            _zanoxSettings = zanoxSettings.ThrowIfNull(nameof(zanoxSettings));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
            _programRepository = programRepository.ThrowIfNull(nameof(programRepository));
        }

        private async Task<ZanoxAdmediaResponse> GetStartPageMediaAsync(int page = 0)
        {
            var responseString = await _client.GetStringAsync($"{_zanoxSettings.GetStoresUri}&items=50&page={page}");
            return JsonConvert.DeserializeObject<ZanoxAdmediaResponse>(responseString, _zanoxSettings.JsonSettings);
        }

        public async Task<IList<Store>> GetAllAsync()
        {
            var response = await GetAllZanoxMedia();
            UpdateProperties(response);
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

        private void UpdateProperties(ZanoxAdmediaResponse media)
        {
            if (media?.Admedium?.Items != null)
            {
                Parallel.ForEach(media.Admedium.Items, new ParallelOptions { MaxDegreeOfParallelism = 20 }, async admediumItem =>
                {
                    var response = await _programRepository.GetProgramAsync(admediumItem.Program.Id.ToString());
                    var program = response?.Programs?.FirstOrDefault();
                    if (program != null) admediumItem.Program.Description = program.Description; //program.DescriptionLocal.Replace("<![CDATA[", string.Empty).Replace("]]>", string.Empty);
                });
            }
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