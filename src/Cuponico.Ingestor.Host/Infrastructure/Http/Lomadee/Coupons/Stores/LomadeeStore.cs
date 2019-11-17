using System;
using Elevar.Utils;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Lomadee.Coupons.Stores
{
    public class LomadeeStore
    {
        [BsonId]
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        public string FriendlyName => Name.ToFriendlyName();

        [JsonProperty("image")]
        public Uri Image { get; set; }

        [JsonProperty("link")]
        public Uri Link { get; set; }

        public int CouponsCount { get; set; }

        protected bool Equals(LomadeeStore other)
        {
            return Id == other.Id && Name == other.Name && FriendlyName == other.FriendlyName && Equals(Image, other.Image) && Equals(Link, other.Link);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LomadeeStore) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FriendlyName != null ? FriendlyName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Image != null ? Image.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Link != null ? Link.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
