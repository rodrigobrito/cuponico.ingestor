using System;

namespace Cuponico.Ingestor.Host.Domain.Advertiser.Stores
{
    public class Store
    {
        public Guid StoreId { get; set; } = Guid.Empty;
        public string Name { get; set; }
        public string Description { get; set; }
        public string FriendlyName { get; set; }
        public Uri ImageUrl { get; set; }
        public Uri StoreUrl { get; set; }
        public int CouponsCount { get; set; }
        public DateTime ChangedDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedDate { get; set; }

        public bool IsMatchableName(string name)
        {
            var matchableName = ReplaceCommonWordsThatBreakTheMatch(name);
            var localName = ReplaceCommonWordsThatBreakTheMatch(Name);
            var distance = localName.ComputeLevenshteinDistance(matchableName);
            var acceptableDistance = GetMaxAcceptableMatchDistance(matchableName);
            var isMatchable = distance <= acceptableDistance;
            return isMatchable;
        }

        private string ReplaceCommonWordsThatBreakTheMatch(string name)
        {
            return name.Replace(".com.br", string.Empty)
                       .Replace(".com", string.Empty);
        }

        private static int GetMaxAcceptableMatchDistance(string name)
        {
            if (name.Length <= 6) return 1;
            return (name.Length * 1) / 6;
        }

        protected bool Equals(Store other)
        {
            return StoreId == other.StoreId && 
                   Name == other.Name &&
                   Description == other.Description &&
                   FriendlyName == other.FriendlyName && 
                   Equals(ImageUrl, other.ImageUrl) && 
                   Equals(StoreUrl, other.StoreUrl) &&
                   CouponsCount == other.CouponsCount;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Store)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = StoreId.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FriendlyName != null ? FriendlyName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ImageUrl != null ? ImageUrl.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (StoreUrl != null ? StoreUrl.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ CouponsCount.GetHashCode();
                return hashCode;
            }
        }

        public static Store Create()
        {
            return new Store
            {
                StoreId = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow
            };
        }

        public static Store GetDefaultStore()
        {
            return new Store
            {
                StoreId = Defaults.StoreId,
                Name = Defaults.StoreName,
                FriendlyName = Defaults.StoreFriendlyName,
                CreatedDate = Defaults.StoreCreatedDate,
                ChangedDate = Defaults.StoreChangedDate,
                ImageUrl = Defaults.StoreImageUrl,
                StoreUrl = Defaults.StoreStoreUrl
            };
        }
    }
}