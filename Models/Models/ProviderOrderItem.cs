using System.ComponentModel.DataAnnotations;

namespace Models.Models
{
    public class ProviderOrderItem
    {
        public int Id { get; set; }

        [Required]
        public int ProviderOrderId { get; set; }

        public ProviderOrder ProviderOrder { get; set; }

        [Required]
        public int ProductId { get; set; }

        private Product product;
        public Product Product
        {
            get
            {
                return product;
            }
            set
            {
                product = value;
                if (product != null) ProductId = product.Id;
            }
        }

        [Required]
        public int Quantity { get; set; }

        public bool ShouldSerializeProduct()
        {
            return Id != default(int);
        }

        public bool ShouldSerializeProviderOrder()
        {
            return Id != default(int);
        }
    }
}
