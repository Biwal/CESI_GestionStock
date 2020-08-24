using Models.Models;
using Negosud.consts;
using Negosud.Services;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml.Media;

namespace Negosud.ViewModels
{
    class HomeViewModel : ViewModelBase
    {
        public Brush ProductAlertColor
        {
            get
            {
                return ProductAlertVisibility ? Colors.ALERT_INVALID_COLOR : Colors.ALERT_VALID_COLOR;
            }
        }
        public Brush ClientAlertColor
        {
            get
            {
                return ClientAlertVisibility ? Colors.ALERT_INVALID_COLOR : Colors.ALERT_VALID_COLOR;
            }
        }
        public Brush ProviderAlertColor
        {
            get
            {
                return ProviderAlertVisibility ? Colors.ALERT_INVALID_COLOR : Colors.ALERT_VALID_COLOR;
            }
        }
        public bool ClientAlertVisibility
        {
            get {
                return ClientOrdersNotFinalized > 0;
            }
        }
        public bool ReverseClientAlertVisibility
        {
            get
            {
                return !ClientAlertVisibility;
            }
        }
        public bool ProviderAlertVisibility
        {
            get
            {
                return ProviderOrdersNotFinalized > 0;
            }
        }
        public bool ReverseProviderAlertVisibility
        {
            get
            {
                return !ProviderAlertVisibility;
            }
        }
        public bool ProductAlertVisibility
        {
            get
            {
                return ProductsUnderMinimalQuantity > 0;
            }
        }
        public bool ReverseProductAlertVisibility
        {
            get
            {
                return !ProductAlertVisibility;
            }
        }

        public int ProductsUnderMinimalQuantity { get; set; }
        public int ClientOrdersNotFinalized { get; set; }
        public int ProviderOrdersNotFinalized { get; set; }
        public int ProductsCount { get; set; }
        public int ClientsCount { get; set; }
        public int ProvidersCount { get; set; }
        public int ProviderOrdersCount { get; set; }
        public int ClientOrdersCount { get; set; }
        public string ClientOrdersTotal { get; set; }
        public string ProviderOrdersTotal { get; set; }
        public string Gains { get; set; }
        public Brush GainsColor { get; set; }

        public HomeViewModel()
        {
            loadData();
        }

        private async void loadData()
        {
            List<Models.Models.Product> products = (await RestClient.Instance.Get<Models.Models.Product>("product")).model;
            ProductsCount = products.Count;

            ClientsCount = (await RestClient.Instance.Get<Models.Models.Product>("client")).model.Count;
            ProvidersCount = (await RestClient.Instance.Get<Models.Models.Product>("provider")).model.Count;

            List<ProviderOrder> providerOrders = (await RestClient.Instance.Get<ProviderOrder>("providerorder")).model.FindAll(e => e.Status != Models.Utils.OrderStatus.ANNULE);
            List<ClientOrder> clientOrders = (await RestClient.Instance.Get<ClientOrder>("clientorder")).model.FindAll(e => e.Status != Models.Utils.OrderStatus.ANNULE);

            ProviderOrdersCount = providerOrders.Count;
            ClientOrdersCount = clientOrders.Count;

            foreach (Models.Models.Product product in products)
            {
                if (product.Quantity <= product.MinStockAvailable) ProductsUnderMinimalQuantity++;
            }

            double providerOrdersTotal = 0;
            foreach (ProviderOrder providerOrder in providerOrders)
            {
                providerOrdersTotal += providerOrder.Price;
                if (providerOrder.Status != Models.Utils.OrderStatus.OK) ProviderOrdersNotFinalized++;
            }
         

            double clientOrdersTotal = 0;
            foreach (ClientOrder clientOrder in clientOrders)
            {
                clientOrdersTotal += clientOrder.Price;
                if (clientOrder.Status != Models.Utils.OrderStatus.OK) ClientOrdersNotFinalized++;
            }
            Gains = (clientOrdersTotal - providerOrdersTotal) + "€";
            GainsColor = (clientOrdersTotal - providerOrdersTotal) > 0 ? Colors.VALID_COLOR : Colors.INVALID_COLOR;
            ProviderOrdersTotal = providerOrdersTotal + "€";
            ClientOrdersTotal = clientOrdersTotal + "€";

            changeProperties(new string[] { "ProductsUnderMinimalQuantity", "ClientOrdersNotFinalized", "ProviderOrdersNotFinalized", "ProductsCount",
            "ClientsCount", "ProvidersCount", "ProviderOrdersCount", "ClientOrdersCount", "ClientOrdersTotal", "ProviderOrdersTotal", "ClientAlertVisibility",
            "ReverseClientAlertVisibility", "ProviderAlertVisibility", "ReverseProviderAlertVisibility", "NoAlertVisibility", "ProductAlertColor",
            "ClientAlertColor", "ProviderAlertColor", "ProductAlertVisibility", "ReverseProductAlertVisibility", "Gains", "GainsColor"});
        }

        private void changeProperties(string[] fields) {
            foreach(string field in fields)
            {
                OnPropertyChanged(field);
            }
        }
    }
}
