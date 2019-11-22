using System;

namespace Cuponico.Ingestor.Host.Domain.Advertiser.Categories
{
    public class Category
    {
        public Guid CategoryId { get; set; } = Guid.Empty;
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public Uri CategoryUrl { get; set; }
        public int CouponsCount { get; set; }
        public DateTime ChangedDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedDate { get; set; }

        public bool IsMatchableName(string name)
        {
            var matchableName = ReplaceCommonWordsThatBreakTheMatch(name);
            var localName = ReplaceCommonWordsThatBreakTheMatch(Name);
            return localName.ComputeLevenshteinDistance(matchableName) <= GetMaxAcceptableMatchDistance(matchableName);
        }

        private static string ReplaceCommonWordsThatBreakTheMatch(string name)
        {
            return name.Replace(".com.br", string.Empty)
                       .Replace(".com", string.Empty);
        }

        private static int GetMaxAcceptableMatchDistance(string name)
        {
            if (name.Length <= 6) return 1;
            return (name.Length * 1) / 6;
        }

        protected bool Equals(Category other)
        {
            return CategoryId == other.CategoryId &&
                   Name == other.Name &&
                   FriendlyName == other.FriendlyName &&
                   Equals(CategoryUrl, other.CategoryUrl) &&
                   CouponsCount == other.CouponsCount;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
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
                hashCode = (hashCode * 397) ^ CouponsCount.GetHashCode();
                return hashCode;
            }
        }

        public static Category Create()
        {
            return new Category
            {
                CategoryId = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow
            };
        }
    }
}
