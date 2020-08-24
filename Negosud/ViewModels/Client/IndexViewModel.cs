using Models.Utils;
using Negosud.consts;
using Negosud.Services;
using Negosud.Views.Product;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Negosud.ViewModels.Client
{
    public class IndexViewModel : ResponseViewModelBase
    {
        private Models.Models.Client selectedClient;

        public Models.Models.Client SelectedClient
        {
            get { return selectedClient; }
            set { selectedClient = value; OnPropertyChanged("SelectedClient"); loadClientOrdersByClient(); }
        }

        private List<Models.Models.Product> productsUpdated;
        private List<Models.Models.Product> failedProducts;
        
        public bool ShowProductsVisibility
        {
            get {
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

        public ObservableCollection<Models.Models.Client> Clients { get; set; }
        public ObservableCollection<Models.Models.ClientOrder> ClientOrders { get; set; } = new ObservableCollection<Models.Models.ClientOrder>();

        public List<OrderStatus> AllStatus { get; set; } = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().ToList();

        public OrderStatus SelectedStatus { get; set; }

        public DelegateCommand EditOrderCommand { get; set; } = new DelegateCommand(ExecuteEditOrder);
        public DelegateCommand StatusChangedCommand { get; set; }
        public DelegateCommand AddProviderOrderCommand { get; set; } = new DelegateCommand(ExecuteAddOrder);
        public DelegateCommand ShowUpdatedProductsCommand { get; set; }
        public DelegateCommand ShowFailedProductsCommand { get; set; }
        
        public IndexViewModel()
        {
            StatusChangedCommand = new DelegateCommand(ExecuteStatusChanged);
            ShowUpdatedProductsCommand = new DelegateCommand(ExecuteShowUpdatedProducts);
            ShowFailedProductsCommand = new DelegateCommand(ExecuteShowFailedProducts);
            loadClients();
            loadClientOrdersByClient();
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

        private static async void ExecuteAddOrder(object obj)
        {
            ScreenManager.Instance.ShowScreen(typeof(Views.Client.ClientOrder));
        }

        private static async void ExecuteEditOrder(object obj)
        {
            ScreenManager.Instance.ShowScreen(typeof(Views.Client.ClientOrder), (Models.Models.ClientOrder) obj);
        }

        private async void ExecuteStatusChanged(object obj)
        {
            if (obj != null && obj.GetType() == typeof(Models.Models.ClientOrder))
            {
                Models.Models.ClientOrder clientOrder = (Models.Models.ClientOrder)obj;
                if(clientOrder.CurrentStatus != clientOrder.Status)
                {
                    bool res = await RestClient.Instance.Put<Models.Models.ClientOrder>(clientOrder, clientOrder.Id);
                    if (res) {
                        clientOrder.CurrentStatus = clientOrder.Status;
                        refreshItem(clientOrder);
                        (List<Models.Models.Product>, List<Models.Models.Product>) products = await updateStock(clientOrder);
                        List<Models.Models.Product> updatedProducts = products.Item1;
                        List<Models.Models.Product> failedProducts = products.Item2;

                        if (updatedProducts.Count == clientOrder.ClientOrderItems.Count)
                        {
                            this.productsUpdated = updatedProducts;
                            OnPropertyChanged("ShowProductsVisibility"); 
                            updateResponseMessage("Tous les produits ont été mis à jour", Colors.VALID_COLOR);
                        }
                        else 
                        {
                            this.failedProducts = failedProducts;
                            OnPropertyChanged("ShowFailedProductsVisibility");
                            updateResponseMessage((clientOrder.ClientOrderItems.Count - updatedProducts.Count) + " produits n'ont pas été mis à jour", Colors.INVALID_COLOR);
                        }
                    }
                }
            }
        }

        private void refreshItem(Models.Models.ClientOrder clientOrder)
        {
            int index = ClientOrders.IndexOf(clientOrder);
            if(index > 0) {
                ClientOrders.RemoveAt(index);
                ClientOrders.Insert(index, clientOrder);
            }
        }

        private async Task<(List<Models.Models.Product>, List<Models.Models.Product>)> updateStock(Models.Models.ClientOrder clientOrder)
        {
            List<Models.Models.Product> updatedProducts = new List<Models.Models.Product>();
            List<Models.Models.Product> failedProducts = new List<Models.Models.Product>();
            foreach (Models.Models.ClientOrderItem clientOrderItem in clientOrder.ClientOrderItems)
            {
                clientOrderItem.Product.Quantity -= clientOrderItem.Quantity;
                bool res = await RestClient.Instance.Put(clientOrderItem.Product, clientOrderItem.Product.Id);
                if (res) updatedProducts.Add(clientOrderItem.Product);
                else failedProducts.Add(clientOrderItem.Product);
            }

            return (updatedProducts, failedProducts);
        }

        private async void loadClientOrdersByClient()
        {
            ClientOrders = new ObservableCollection<Models.Models.ClientOrder>((await RestClient.Instance.GetAll<Models.Models.ClientOrder> ("clientorder" + (SelectedClient != null ? "?clientId="+ SelectedClient.Id : ""))).model);
            OnPropertyChanged("ClientOrders");
        }

        private async void loadClients()
        {
            Clients = new ObservableCollection<Models.Models.Client>((await RestClient.Instance.GetAll<Models.Models.Client>("client")).model);
            OnPropertyChanged("Clients");
        }
    }
}