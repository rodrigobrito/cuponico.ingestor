using System;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories
{
    public class AffiliateCategory
    {
        public long CategoryId { get; set; }
        public string AffiliateProgram { get; set; }
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public Uri CategoryUrl { get; set; }
        public int CouponsCount { get; set; }
        public DateTime ChangedDate { get; set; } = DateTime.UtcNow;

        protected bool Equals(AffiliateCategory other)
        {
            return CategoryId == other.CategoryId && Name == other.Name && AffiliateProgram == other.AffiliateProgram && FriendlyName == other.FriendlyName && Equals(CategoryUrl, other.CategoryUrl) && CouponsCount == other.CouponsCount;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((AffiliateCategory)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = CategoryId.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AffiliateProgram != null ? AffiliateProgram.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FriendlyName != null ? FriendlyName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CategoryUrl != null ? CategoryUrl.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CouponsCount.GetHashCode());
                return hashCode;
            }
        }
    }
}
