using Models.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Models.Models
{
    public class ProviderOrder
    {
        public int Id { get; set; }

        [Required]
        public int ProviderId { get; set; }

        private Provider provider;
        public Provider Provider
        {
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
        public OrderStatus? CurrentStatus { get; set; }

        [NotMapped]
        public double Price
        {
            get
            {
                double price = 0;

                foreach (ProviderOrderItem providerOrderItem in ProviderOrderItems)
                {
                    if (providerOrderItem.Product != null)
                    {
                        price += providerOrderItem.Quantity * providerOrderItem.Product.PackedPrice;
                    }
                }

                return price;
            }
        }

        public ObservableCollection<ProviderOrderItem> ProviderOrderItems { get; set; } = new ObservableCollection<ProviderOrderItem>();

        public ProviderOrder()
        {
            CreatedAt = DateTime.Now;
        }

        [NotMapped]
        [JsonIgnore]
        public string FormattedProductNumber {
            get {
                int packs = 0;
                int products = 0;
                
                foreach(ProviderOrderItem providerOrderItem in ProviderOrderItems)
                {
                    if(providerOrderItem.Product != null)
                    {
                        packs += providerOrderItem.Quantity;
                        products += providerOrderItem.Quantity * providerOrderItem.Product.PackedQuantity;
                    }
                }

                return packs + " packs (" + products + " bouteilles)";
            }
        }

        [NotMapped]
        [JsonIgnore]
        public string FormattedPrice
        {
            get
            {
                double price = 0;

                foreach (ProviderOrderItem providerOrderItem in ProviderOrderItems)
                {
                    if (providerOrderItem.Product != null)
                    {
                        price += providerOrderItem.Quantity * providerOrderItem.Product.PackedPrice;
                    }
                }

                return price+"€";
            }
        }

        public bool ShouldSerializeProvider()
        {
            return Id != default(int);
        }
    }
}
