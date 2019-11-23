namespace Cuponico.Ingestor.Host.Domain
{
    public static class CuponicoEvents
    {
        public static string AffiliateStoreCreated { get; set; }
        public static string AffiliateStoreChanged { get; set; }
        public static string AffiliateStoreCanceled { get; set; }
        public static string AffiliateCategoryCreated { get; set; }
        public static string AffiliateCategoryChanged { get; set; }
        public static string AffiliateCategoryCanceled { get; set; }
        public static string AffiliateCouponCreated { get; set; }
        public static string AffiliateCouponChanged { get; set; }
        public static string AffiliateCouponCanceled { get; set; }
    }
}