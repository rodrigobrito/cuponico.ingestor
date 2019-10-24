﻿using System;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Ingestor.ConsoleHost.Partners.Lomadee.Coupons.Categories
{
    public class LomadeeCategory
    {
        [BsonId]
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("link")]
        public Uri Link { get; set; }

        protected bool Equals(LomadeeCategory other)
        {
            return Id == other.Id && Name == other.Name && Equals(Link, other.Link);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LomadeeCategory) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Link != null ? Link.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
