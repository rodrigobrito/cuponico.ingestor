using System.Runtime.Serialization;

namespace Cuponico.Ingestor.Host.Partners.Coupons
{
    [DataContract(Name = "Category", Namespace = "br.com.cuponico.partners.coupons")]
    public class Category
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
