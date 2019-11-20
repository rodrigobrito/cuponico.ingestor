namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets
{
    public class AffiliateCategory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        protected bool Equals(AffiliateCategory other)
        {
            return Id == other.Id && Name == other.Name && FriendlyName == other.FriendlyName;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AffiliateCategory)obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode() * 397) ^ (Name != null ? Name.GetHashCode() : 0) ^ (FriendlyName != null ? FriendlyName.GetHashCode() : 0);
            }
        }
    }
}
