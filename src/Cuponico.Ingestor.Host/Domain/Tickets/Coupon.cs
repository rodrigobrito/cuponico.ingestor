using System;

namespace Cuponico.Ingestor.Host.Domain.Tickets
{
    public class Coupon
    {
        public long CouponId { get; set; }
        public string Description { get; set; }
        public string FriendlyDescription { get; set; }
        public string Remark { get; set; }
        public string Code { get; set; }
        public decimal Discount { get; set; }
        public Store Store { get; set; }
        public Category Category { get; set; }
        public DateTime Validity { get; set; }
        public Uri CouponLink { get; set; }
        public bool New { get; set; }
        public bool IsPercentage { get; set; }
        public bool Shipping { get; set; }
        public DateTime ChangedDate { get; set; } = DateTime.Now.ToUniversalTime();
        protected bool Equals(Coupon other)
        {
            return CouponId == other.CouponId &&
                   Description == other.Description &&
                   FriendlyDescription == other.FriendlyDescription &&
                   Remark == other.Remark &&
                   Code == other.Code &&
                   Discount == other.Discount &&
                   Equals(Store, other.Store) &&
                   Equals(Category, other.Category) &&
                   Validity.Equals(other.Validity) &&
                   Equals(CouponLink, other.CouponLink) &&
                   New == other.New &&
                   IsPercentage == other.IsPercentage &&
                   Shipping == other.Shipping;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Coupon)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = CouponId.GetHashCode();
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FriendlyDescription != null ? FriendlyDescription.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Remark != null ? Remark.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Code != null ? Code.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Discount.GetHashCode();
                hashCode = (hashCode * 397) ^ (Store != null ? Store.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Category != null ? Category.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Validity.GetHashCode();
                hashCode = (hashCode * 397) ^ (CouponLink != null ? CouponLink.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ New.GetHashCode();
                hashCode = (hashCode * 397) ^ IsPercentage.GetHashCode();
                hashCode = (hashCode * 397) ^ Shipping.GetHashCode();
                return hashCode;
            }
        }
    }
}