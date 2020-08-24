using Models.Models;
using Negosud.consts;
using Negosud.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Negosud.ViewModels.Provider
{
    class ProviderOrderViewModel : ResponseViewModelBase
    {
        public ProviderOrder ProviderOrder { get; set; } = new ProviderOrder();

        public string ButtonTitle
        {
            get
            {
                return ProviderOrder.Id == default ? "Passer commande" : "Editer la commande";
            }
        }

        public ObservableCollection<Models.Models.Provider> Providers { get; set; }

        public bool SelectedProviderVisibility
        {
            get {
                return ProviderOrder.Id == default;
            }
        }

        public bool TextProviderVisibility { get => !SelectedProviderVisibility; }

        public DelegateCommand AddClientCommand { get; set; }
        public DelegateCommand PassOrderCommand { get; set; }
        public DelegateCommand ReduceQuantityCommand { get; set; }
        public DelegateCommand AddQuantityCommand { get; set; }
        public DelegateCommand SelectedProviderChanged { get; set; }
        public DelegateCommand UpdateOrderQuantityCommand { get; set; }

        public ProviderOrderViewModel()
        {
            PassOrderCommand = new DelegateCommand(ExecutePassOrder);
            ReduceQuantityCommand = new DelegateCommand(ExecuteReduceQuantity);
            AddQuantityCommand = new DelegateCommand(ExecuteAddQuantity);
            SelectedProviderChanged = new DelegateCommand(ExecuteProviderChange);
            UpdateOrderQuantityCommand = new DelegateCommand(ExecuteUpdateOrderQuantity);
            if(ProviderOrder.Id == default(int)) loadProviders();
        }

        public void SetProviderOrder(ProviderOrder providerOrder)
        {
            ProviderOrder = providerOrder;
            OnPropertyChanged("ProviderOrder");
            OnPropertyChanged("ButtonTitle");
        }

        private void ExecuteUpdateOrderQuantity(object obj)
        {
            ProviderOrderItem providerOrderItem = (ProviderOrderItem)obj;
            refreshItem(providerOrderItem);
        } 

        private void ExecuteAddQuantity(object obj)
        {
            ProviderOrderItem providerOrderItem = (ProviderOrderItem)obj;
            providerOrderItem.Quantity++;
            refreshItem(providerOrderItem);
        }

        private void ExecuteReduceQuantity(object obj)
        {
            ProviderOrderItem providerOrderItem = (ProviderOrderItem)obj;
            if (providerOrderItem.Quantity - 1 >= 0) providerOrderItem.Quantity--;
            refreshItem(providerOrderItem);
        }

        private void ExecuteProviderChange(object obj)
        {
            if(ProviderOrder.Provider != null)
            {
                loadProductsByProvider();
            }
        }

        private void refreshItem(ProviderOrderItem clientOrderProductItem)
        {
            int index = ProviderOrder.ProviderOrderItems.IndexOf(clientOrderProductItem);
            if(index >= 0) {
                ProviderOrder.ProviderOrderItems.RemoveAt(index);
                ProviderOrder.ProviderOrderItems.Insert(index, clientOrderProductItem);
                OnPropertyChanged("ProviderOrder");
            }
        }

        private async void ExecutePassOrder(object obj)
        {

            if (ProviderOrder.Provider == null)
            {
                updateResponseMessage("Il faut choisir un fournisseur pour passer commande", Colors.INVALID_COLOR);
                return;
            }

            List<ProviderOrderItem> providerOrderProducts = getProviderOrderItemsAvailable();

            if (providerOrderProducts.Count == 0)
            {
                updateResponseMessage("Il faut au minimum un produit pour passer commande", Colors.INVALID_COLOR);
                return;
            }

            bool result = true;
            ProviderOrder.ProviderOrderItems = new ObservableCollection<Models.Models.ProviderOrderItem>(providerOrderProducts);

            if (ProviderOrder.Id != default)
            {
                result = await RestClient.Instance.Put<ProviderOrder>(ProviderOrder, ProviderOrder.Id);
            }
            else
            {
                result = await RestClient.Instance.Post<ProviderOrder>(ProviderOrder);
            }

            Action a = () => ScreenManager.Instance.ShowScreen(typeof(Views.Provider.Index));

            if (result)
            {
                updateResponseMessage("La commande fournisseur a bien été passée", Colors.VALID_COLOR, a);
            }
            else
            {
                updateResponseMessage("La commande fournisseur n'a pas pu aboutir", Colors.INVALID_COLOR, a);
            }
        }

        private List<ProviderOrderItem> getProviderOrderItemsAvailable()
        {
            List<ProviderOrderItem> providerOrderProducts = new List<ProviderOrderItem>();
            foreach (ProviderOrderItem providerOrderProductItem in ProviderOrder.ProviderOrderItems)
            {
                if (providerOrderProductItem.Quantity > 0)
                {
                    providerOrderProductItem.Product = null;
                    providerOrderProducts.Add(providerOrderProductItem);
                }
            }

            return providerOrderProducts;
        }

        private async void loadProductsByProvider()
        {
            if (ProviderOrder.Id != default(int)) return;
            List<Models.Models.Product> productsByProvider = (await RestClient.Instance.GetAll<Models.Models.Product>("product?providerId=" + ProviderOrder.Provider.Id)).model;
            ProviderOrder.ProviderOrderItems.Clear();
            foreach (Models.Models.Product product in productsByProvider)
            {
                if(!providerOrderItemsHas(product))
                {
                    ProviderOrder.ProviderOrderItems.Add(
                       new ProviderOrderItem
                       {
                           Product = product,
                           Quantity = 0,
                           ProductId = product.Id,
                           ProviderOrderId = ProviderOrder.Id
                       }
                    );
                }
            }

            OnPropertyChanged("ProviderOrder");
        }

        private bool providerOrderItemsHas(Models.Models.Product product)
        {
            foreach(ProviderOrderItem providerOrderItem in ProviderOrder.ProviderOrderItems)
            {
                if(providerOrderItem.Product.Id == product.Id)
                {
                    return true;
                }
            }

            return false;
        }

        private async void loadProviders()
        {
  
            Providers = new ObservableCollection<Models.Models.Provider>((await RestClient.Instance.GetAll<Models.Models.Provider>("provider")).model);
            ProviderOrder.Provider = Providers.FirstOrDefault();
            loadProductsByProvider();
            OnPropertyChanged("Providers");
        }
    }
}