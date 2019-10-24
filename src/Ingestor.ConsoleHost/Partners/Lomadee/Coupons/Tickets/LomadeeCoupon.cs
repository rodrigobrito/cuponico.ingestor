using System;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Ingestor.ConsoleHost.Partners.Lomadee.Coupons.Tickets
{
    public class LomadeeCoupon
    {
        [BsonId]
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("discount")]
        public decimal Discount { get; set; }

        [JsonProperty("store")]
        public LomadeeStore Store { get; set; }

        [JsonProperty("category")]
        public LomadeeCategory Category { get; set; }

        [JsonProperty("vigency")]
        public DateTime Vigency { get; set; }

        [JsonProperty("link")]
        public Uri Link { get; set; }

        [JsonProperty("new")]
        public bool New { get; set; }

        public void UpdateProperties()
        {
            // Update description and remark
            var (description, remark) = ExtractDescriptionAndRemark(Description);
            Description = description;
            Remark = remark;
            // Update discount
            Discount = Discount == 0 ? TryGetDiscountFromDescriptionProperty(this) : Discount;
        }

        private static (string, string) ExtractDescriptionAndRemark(string couponText)
        {
            var description = couponText;
            var remark = string.Empty;
            if (!string.IsNullOrWhiteSpace(couponText))
            {
                var positionToBreak = 0;
                for (var i = 0; i < couponText.Length; i++)
                {
                    if (i > 0 && (i + 1) < couponText.Length)
                    {
                        if (couponText[i - 1] != ' ' &&
                            !char.IsDigit(couponText[i - 1]) &&
                            couponText[i] == '.' &&
                            couponText[i + 1] == ' ')
                        {
                            positionToBreak = i;
                            break;
                        }
                    }
                }

                if (positionToBreak > 0)
                {
                    description = $"{couponText.Substring(0, positionToBreak)}.";
                    if (positionToBreak + 2 <= couponText.Length)
                        remark = couponText.Substring(positionToBreak + 2);
                }
            }
            return (description, remark);
        }

        private static decimal TryGetDiscountFromDescriptionProperty(LomadeeCoupon source)
        {
            var description = source.Description.Replace("R$", string.Empty)
                .Replace(" ", string.Empty).Trim();

            var builder = new StringBuilder();
            foreach (var str in description)
            {
                if (!char.IsDigit(str) && str != ',') break; // When I find any non-decimal characters, I end the operation.
                builder.Append(str == ',' ? '.' : str);
            }

            var number = builder.ToString();
            return string.IsNullOrWhiteSpace(number) ? 0 : decimal.Parse(number);
        }

        protected bool Equals(LomadeeCoupon other)
        {
            return Id == other.Id && 
                   Description == other.Description && 
                   Remark == other.Remark && 
                   Code == other.Code && 
                   Discount == other.Discount && 
                   Equals(Store, other.Store) && 
                   Equals(Category, other.Category) && 
                   Vigency.Equals(other.Vigency) &&
                   Equals(Link, other.Link) && 
                   New == other.New;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LomadeeCoupon) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Remark != null ? Remark.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Code != null ? Code.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Discount.GetHashCode();
                hashCode = (hashCode * 397) ^ (Store != null ? Store.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Category != null ? Category.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Vigency.GetHashCode();
                hashCode = (hashCode * 397) ^ (Link != null ? Link.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ New.GetHashCode();
                return hashCode;
            }
        }
    }
}
