using System;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets
{
    public class AffiliateStore
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public Uri ImageUrl { get; set; }
        public Uri StoreUrl { get; set; }

        protected bool Equals(AffiliateStore other)
        {
            return Id == other.Id && Name == other.Name && FriendlyName == other.FriendlyName && Equals(ImageUrl, other.ImageUrl) && Equals(StoreUrl, other.StoreUrl);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AffiliateStore)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FriendlyName != null ? FriendlyName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ImageUrl != null ? ImageUrl.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (StoreUrl != null ? StoreUrl.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
