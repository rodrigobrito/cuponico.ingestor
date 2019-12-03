namespace Cuponico.Ingestor.Host.Domain.Advertiser.Coupons
{
    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FriendlyName { get; set; }

        public static Category GetDefaultCategory()
        {
            return new Category
            {
                Id = Defaults.CategoryId.ToString(),
                Name = Defaults.CategoryName,
                FriendlyName = Defaults.CategoryFriendlyName
            };
        }
    }
}