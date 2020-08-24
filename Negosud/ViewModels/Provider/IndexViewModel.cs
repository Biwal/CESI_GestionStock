using Models.Utils;
using Negosud.consts;
using Negosud.Services;
using Negosud.Views.Product;
using Negosud.Views.Provider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Negosud.ViewModels.Provider
{
    public class IndexViewModel : ResponseViewModelBase
    {
        private Models.Models.Provider selectedProvider;

        public Models.Models.Provider SelectedProvider
        {
            get { return selectedProvider; }
            set { selectedProvider = value; OnPropertyChanged("SelectedProvider"); loadProviderOrdersByProvider(); }
        }

        private List<Models.Models.Product> productsUpdated;
        private List<Models.Models.Product> failedProducts;

        public bool ShowProductsVisibility
        {
            get
            {
                return (productsUpdated != null && productsUpdated.Count > 0);
            }
        }
        public bool ShowFailedProductsVisibility
        {
            get
            {
                return (failedProducts != null && failedProducts.Count > 0);
            }

        }

        public ObservableCollection<Models.Models.Provider> Providers { get; set; }
        public ObservableCollection<Models.Models.ProviderOrder> ProviderOrders { get; set; } = new ObservableCollection<Models.Models.ProviderOrder>();

        public List<OrderStatus> AllStatus { get; set; } = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().ToList();

        public OrderStatus SelectedStatus { get; set; }

        public DelegateCommand EditOrderCommand { get; set; } = new DelegateCommand(ExecuteEditOrder);
        public DelegateCommand StatusChangedCommand { get; set; }
        public DelegateCommand AddProviderOrderCommand { get; set; } = new DelegateCommand(ExecuteAddOrder);
        public DelegateCommand RegularizeStockCommand { get; set; }
        public DelegateCommand ShowUpdatedProductsCommand { get; set; }
        public DelegateCommand ShowFailedProductsCommand { get; set; }

        public IndexViewModel()
        {
            StatusChangedCommand = new DelegateCommand(executeStatusChanged);
            RegularizeStockCommand = new DelegateCommand(executeRegularizeStock);
            ShowUpdatedProductsCommand = new DelegateCommand(ExecuteShowUpdatedProducts);
            ShowFailedProductsCommand = new DelegateCommand(ExecuteShowFailedProducts);
            loadProviders();
            loadProviderOrdersByProvider();
        }

        private async void executeRegularizeStock(object obj)
        {
            List<Models.Models.Product> productsToUpdate = (await RestClient.Instance.Get<Models.Models.Product>("product?toUpdate=true")).model;

            if (productsToUpdate.Count == 0)
            {
                updateResponseMessage("Aucun produit ne nécessite une régularisation du stock", Colors.VALID_COLOR);
                return;
            }

            int providerOrdersAdded = 0;
            int providerOrdersUpdated = 0;

            foreach (Models.Models.Product product in productsToUpdate)
            {
                int quantityToAdd = (int)Math.Floor((double)(product.MaxStockAvailable - product.Quantity) / product.PackedQuantity);

                Models.Models.ProviderOrder providerOrder = await getOrCreateOrderFromProduct(product);
                Models.Models.ProviderOrderItem providerOrderItem = getOrCreateFromProvider(providerOrder, product);
                if (providerOrderItem.Id != default)
                {
                    if (providerOrderItem.Quantity < quantityToAdd)
                    {
                        providerOrderItem.Quantity += (quantityToAdd - providerOrderItem.Quantity);
                    }
                }
                else
                {
                    providerOrderItem.Quantity = quantityToAdd;
                    providerOrder.ProviderOrderItems.Add(providerOrderItem);
                }

                if (providerOrder.Id == default)
                {
                    bool res = await RestClient.Instance.Post(providerOrder);
                    if (res) providerOrdersAdded++;
                }
                else
                {
                    bool res = await RestClient.Instance.Put(providerOrder, providerOrder.Id);
                    if (res) providerOrdersUpdated++;
                }
            }

            if (providerOrdersAdded > 0 || providerOrdersUpdated > 0)
            {
                string plurOrSingEnd = providerOrdersAdded > 1 ? "s" : "";
                string femPlurOrSingEnd = providerOrdersAdded > 1 ? "es" : "e";

                updateResponseMessage(
                    (providerOrdersAdded > 0 ? providerOrdersAdded + " commande" + plurOrSingEnd + " ajoutée" + plurOrSingEnd : "")
                    + (providerOrdersUpdated > 0 ? providerOrdersUpdated + " commande" + plurOrSingEnd + " mis" + femPlurOrSingEnd + " à jour" : ""), Colors.VALID_COLOR);
            }
            else updateResponseMessage("Aucune commande n'a pu être ajoutée ou mise à jour", Colors.INVALID_COLOR);
            loadProviderOrdersByProvider();
        }

        private async Task<Models.Models.ProviderOrder> getOrCreateOrderFromProduct(Models.Models.Product product)
        {
            Models.Models.ProviderOrder providerOrder;
            List<Models.Models.ProviderOrder> providerOrders = (await RestClient.Instance.Get<Models.Models.ProviderOrder>("providerorder?lastProviderOrder=true&providerId=" + product.ProviderId)).model;
            if (providerOrders.Count == 0)
            {
                providerOrder = new Models.Models.ProviderOrder { Provider = product.Provider, Status = Models.Utils.OrderStatus.NONE };
            }
            else providerOrder = providerOrders[0];

            return providerOrder;
        }

        private Models.Models.ProviderOrderItem getOrCreateFromProvider(Models.Models.ProviderOrder providerOrder, Models.Models.Product product)
        {
            foreach (Models.Models.ProviderOrderItem providerOrderItem in providerOrder.ProviderOrderItems)
            {
                if (providerOrderItem.Product.Id == product.Id)
                {
                    return providerOrderItem;
                }
            }

            return new Models.Models.ProviderOrderItem { ProviderOrder = providerOrder, Product = product };
        }

        private static async void ExecuteAddOrder(object obj)
        {
            ScreenManager.Instance.ShowScreen(typeof(ProviderOrder));
        }

        private static async void ExecuteEditOrder(object obj)
        {
            ScreenManager.Instance.ShowScreen(typeof(ProviderOrder), (Models.Models.ProviderOrder) obj);
        }

        private async void executeStatusChanged(object obj)
        {
            Debug.WriteLine("Status changed ???");

            if (obj != null && obj is Models.Models.ProviderOrder)
            {
                Debug.WriteLine(((Models.Models.ProviderOrder)obj).Price);
                Models.Models.ProviderOrder providerOrder = (Models.Models.ProviderOrder)obj;
                Debug.WriteLine("CurrentStatus " + providerOrder.CurrentStatus + " NewStatus " + providerOrder.Status);
                if(providerOrder.CurrentStatus != providerOrder.Status)
                {
                    bool res = await RestClient.Instance.Put<Models.Models.ProviderOrder>(providerOrder, providerOrder.Id);
                    if (res) {
                        providerOrder.CurrentStatus = providerOrder.Status;
                        refreshItem(providerOrder);
                        (List<Models.Models.Product>, List<Models.Models.Product>) products = await updateStock(providerOrder);
                        List<Models.Models.Product> updatedProducts = products.Item1;
                        List<Models.Models.Product> failedProducts = products.Item2;
                        if (updatedProducts.Count == providerOrder.ProviderOrderItems.Count)
                        {
                            this.productsUpdated = updatedProducts;
                            OnPropertyChanged("ShowProductsVisibility");
                            updateResponseMessage("Les produits ont bien été réapprovisionnés", Colors.VALID_COLOR);
                        } else
                        {
                            this.failedProducts = failedProducts;
                            OnPropertyChanged("ShowFailedProductsVisibility");
                            updateResponseMessage((providerOrder.ProviderOrderItems.Count - updatedProducts.Count) + " produits n'ont pas été mis à jour", Colors.INVALID_COLOR);
                        }
                    }
                }
            }
        }

        private async void ExecuteShowUpdatedProducts(object obj)
        {
            ShowListDialog showListDialog = new ShowListDialog("Liste des produits mis à jour", productsUpdated);
            await showListDialog.ShowAsync();
        }

        private async void ExecuteShowFailedProducts(object obj)
        {
            ShowListDialog showListDialog = new ShowListDialog("Liste des produits dont la mise à jour n'a pas abouti", failedProducts);
            await showListDialog.ShowAsync();
        }

        private async Task<(List<Models.Models.Product>, List<Models.Models.Product>)> updateStock(Models.Models.ProviderOrder providerOrder)
        {
            List<Models.Models.Product> updatedProducts = new List<Models.Models.Product>();
            List<Models.Models.Product> failedProducts = new List<Models.Models.Product>();
            foreach (Models.Models.ProviderOrderItem providerOrderItem in providerOrder.ProviderOrderItems) {
                providerOrderItem.Product.Quantity += providerOrderItem.Quantity * providerOrderItem.Product.PackedQuantity;
                bool res = await RestClient.Instance.Put<Models.Models.Product>(providerOrderItem.Product, providerOrderItem.Product.Id);
                if (res) updatedProducts.Add(providerOrderItem.Product);
                else failedProducts.Add(providerOrderItem.Product);
            }

            return (updatedProducts, failedProducts);
        }

        private void refreshItem(Models.Models.ProviderOrder providerOrder)
        {
            int index = ProviderOrders.IndexOf(providerOrder);
            if(index >= 0)
            {
                ProviderOrders.RemoveAt(index);
                ProviderOrders.Insert(index, providerOrder);
            }
        }

        private async void loadProviderOrdersByProvider()
        {
            ProviderOrders = new ObservableCollection<Models.Models.ProviderOrder>((await RestClient.Instance.GetAll<Models.Models.ProviderOrder> ("providerorder" + (SelectedProvider != null ? "?providerId="+SelectedProvider.Id : ""))).model);          
            OnPropertyChanged("ProviderOrders");
        }

        private async void loadProviders()
        {
            Providers = new ObservableCollection<Models.Models.Provider>((await RestClient.Instance.GetAll<Models.Models.Provider>("provider")).model);
            OnPropertyChanged("Providers");
        }
    }
}