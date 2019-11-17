using System;

namespace Cuponico.Ingestor.Host.Domain.Stores
{
    public class Store
    {
        public long StoreId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FriendlyName { get; set; }
        public Uri ImageUrl { get; set; }
        public Uri StoreUrl { get; set; }
        public int CouponsCount { get; set; }
        public DateTime ChangedDate { get; set; } = DateTime.Now.ToUniversalTime();

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
    }
}
