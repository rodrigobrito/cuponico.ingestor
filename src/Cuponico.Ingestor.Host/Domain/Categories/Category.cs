using System;

namespace Cuponico.Ingestor.Host.Domain.Categories
{
    public class Category
    {
        public long CategoryId { get; set; }
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public Uri CategoryUrl { get; set; }
        public int CouponsCount { get; set; }
        public DateTime ChangedDate { get; set; }

        protected bool Equals(Category other)
        {
            return CategoryId == other.CategoryId && Name == other.Name && FriendlyName == other.FriendlyName && Equals(CategoryUrl, other.CategoryUrl) && CouponsCount == other.CouponsCount;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Category)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = CategoryId.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FriendlyName != null ? FriendlyName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CategoryUrl != null ? CategoryUrl.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CouponsCount.GetHashCode());
                return hashCode;
            }
        }
    }
}
