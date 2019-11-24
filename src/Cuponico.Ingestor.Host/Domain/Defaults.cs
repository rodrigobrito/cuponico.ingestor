using System;
using Elevar.Utils;

namespace Cuponico.Ingestor.Host.Domain
{
    public class Defaults
    {
        #region Category

        public static Guid CategoryId => Guid.Parse("79e81181-d9bb-43e7-8be6-812be8ef237e");
        public static string CategoryName => "Chegando agora";
        public static string CategoryFriendlyName => CategoryName.ToFriendlyName();
        public static DateTime CategoryCreatedDate => new DateTime(2019, 07, 01);
        public static DateTime CategoryChangedDate => DateTime.UtcNow;

        #endregion

        #region Stores

        public static Guid StoreId => Guid.Parse("15bb3f66-95ce-4d05-84b8-fd0c1071eae6");
        public static string StoreName => "Cuponico";
        public static string StoreFriendlyName => "cuponico";
        public static DateTime StoreCreatedDate => new DateTime(2019, 07, 01);
        public static DateTime StoreChangedDate => DateTime.UtcNow;
        public static Uri StoreImageUrl => new Uri(@"https://cuponico.com.br/assets/img/coupon.png");
        public static Uri StoreStoreUrl => new Uri(@"https://cuponico.com.br");

        #endregion
    }
}
