using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Models.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int MinStockAvailable { get; set; }

        [Required]
        public int MaxStockAvailable { get; set; }

        [Required]
        public int PackedQuantity { get; set; } = 6;

        [Required]
        public int PackedPrice { get; set; }

        public byte[] Image { get; set; }

        [Required]
        public int FamilyId { get; set; }

        [Required]
        public int ProviderId { get; set; }

        private Provider provider;

        public Provider Provider {
            get
            {
                return provider;
            }
            set
            {
                provider = value;
                if (provider != null) ProviderId = provider.Id;
            }
        }

        private Family family;

        public Family Family
        {
            get
            {
                return family;
            }
            set
            {
                family = value;
                if(family != null) FamilyId = family.Id;
            }
        }

        public bool Validate()
        {
            return true;
        }

        public string FormattedPrice
        {
            get
            {
                return Price + "€";
            }
        }

        public bool ShouldSerializeFamily()
        {
            return Id != default(int);
        }

        public bool ShouldSerializeProvider()
        {
            return Id != default(int);
        }
    }
}
