using System;

namespace Cuponico.Ingestor.Host.Domain.Advertiser.Coupons
{
    public class Store
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public Uri ImageUrl { get; set; }
        public Uri StoreUrl { get; set; }
        public static Store GetDefaultStore()
        {
            return new Store
            {
                Id = Defaults.StoreId.ToString(),
                Name = Defaults.StoreName,
                FriendlyName = Defaults.StoreFriendlyName,
                ImageUrl = Defaults.StoreImageUrl,
                StoreUrl = Defaults.StoreStoreUrl
            };
        }
    }
}