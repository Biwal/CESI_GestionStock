using System.ComponentModel.DataAnnotations;

namespace Models.Models
{
    public class ClientOrderItem
    {
        public int Id { get; set; }

        [Required]
        public int ClientOrderId { get; set; }

        public ClientOrder ClientOrder { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Product Product { get; set; }

        [Required]
        public int Quantity { get; set; }

        public bool ShouldSerializeProduct()
        {
            return Id != default(int);
        }

        public bool ShouldSerializeClientOrder()
        {
            return Id != default(int);
        }
    }
}
