using Models.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;

namespace Models.Models
{
    public class ClientOrder
    {
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }

        private Client client;
        public Client Client
        {
            get
            {
                return client;
            }
            set
            {
                client = value;
                if (client != null) ClientId = client.Id;
            }
        }

        [Required]
        public DateTime CreatedAt { get; set; }

        private OrderStatus status;

        [Required]
        public OrderStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                if (CurrentStatus == null) CurrentStatus = value;
            }
        }

        [NotMapped]
        [JsonIgnore]
        public int FormattedProductNumber
        {
            get
            {
                int products = 0;

                ClientOrderItems.ToList().ForEach(ci => products += ci.Quantity);
               
                return products;
            }
        }

        [NotMapped]
        [JsonIgnore]
        public string FormattedPrice
        {
            get
            {
                double price = 0;

                ClientOrderItems.ToList().ForEach(ci => {
                    if (ci.Product != null) { price += ci.Quantity * ci.Product.Price; } 
                });

                return price + "€";
            }
        }

        [NotMapped]
        [JsonIgnore]
        public OrderStatus? CurrentStatus { get; set; }

        [NotMapped]
        [JsonIgnore]
        public double Price {
            get 
            {
                double price = 0;
                ClientOrderItems.ToList().ForEach(co =>
                {
                    if (co.Product != null) price += co.Quantity * co.Product.Price;
                });

                return price;
            }
        }

        public ObservableCollection<ClientOrderItem> ClientOrderItems { get; set; } = new ObservableCollection<ClientOrderItem>();

        public ClientOrder()
        {
            CreatedAt = DateTime.Now;
        }

        public bool ShouldSerializeClient()
        {
            return Id != default(int);
        }
    }
}
