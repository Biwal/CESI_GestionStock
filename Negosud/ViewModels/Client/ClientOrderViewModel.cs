using Negosud.Views.Client;
using System;
using System.Collections.ObjectModel;
using Negosud.Services;
using System.Collections.Generic;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Models.Models;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Negosud.ViewModels.Client
{
    public class ClientOrderViewModel : ResponseViewModelBase
    {
        public Models.Models.ClientOrder ClientOrder { get; set; } = new Models.Models.ClientOrder();

        public string ButtonTitle
        {
            get
            {
                return ClientOrder.Id == default ? "Passer commande" : "Editer la commande";
            }
        }

        public ObservableCollection<Models.Models.Client> Clients { get; set; }

        public DelegateCommand AddClientCommand { get; set; }
        public DelegateCommand PassOrderCommand { get; set; }
        public DelegateCommand ReduceQuantityCommand { get; set; }
        public DelegateCommand AddQuantityCommand { get; set; }
        public DelegateCommand UpdateOrderQuantityCommand { get; set; }

        public bool SelectedClientVisibility
        {
            get
            {
                return ClientOrder.Id == default;
            }
        }

        public bool ReverseSelectedClientVisibility { get => !SelectedClientVisibility; }

        public ClientOrderViewModel()
        {
            PassOrderCommand = new DelegateCommand(ExecutePassOrder);
            ReduceQuantityCommand = new DelegateCommand(ExecuteReduceQuantity);
            AddQuantityCommand = new DelegateCommand(ExecuteAddQuantity);
            PassOrderCommand = new DelegateCommand(ExecutePassOrder);
            AddClientCommand = new DelegateCommand(ExecuteAddClientClick);
            UpdateOrderQuantityCommand = new DelegateCommand(ExecuteUpdateOrderQuantity);
            loadClients();
            loadProductItems();
        }

        private void ExecuteUpdateOrderQuantity(object obj)
        {
            ClientOrderItem providerOrderItem = (ClientOrderItem)obj;
            refreshItem(providerOrderItem);
        }

        private async void loadProductItems() {
            if (ClientOrder.Id != default) return;
            List<Models.Models.Product> products = (await RestClient.Instance.GetAll<Models.Models.Product>("product")).model;
            foreach(Models.Models.Product product in products)
            {
                if(!clientOrderItemsHas(product))
                {
                    ClientOrder.ClientOrderItems.Add(
                        new Models.Models.ClientOrderItem
                        {
                            Product = product,
                            Quantity = 0,
                            ProductId = product.Id
                        }
                    );
                }
            }

            OnPropertyChanged("ClientOrder");
        }

        public void SetClientOrder(Models.Models.ClientOrder clientOrder)
        {
            ClientOrder = clientOrder;
            OnPropertyChanged("ClientOrder");
            OnPropertyChanged("ButtonTitle");
        }

        private bool clientOrderItemsHas(Models.Models.Product product)
        {
            foreach (ClientOrderItem clientOrderItem in ClientOrder.ClientOrderItems)
            {
                if (clientOrderItem.Product.Id == product.Id)
                {
                    return true;
                }
            }

            return false;
        }

        private void ExecuteAddQuantity(object obj) {
            ClientOrderItem clientOrderProductItem = (ClientOrderItem)obj;
            clientOrderProductItem.Quantity++;
            refreshItem(clientOrderProductItem);
        }

        private void ExecuteReduceQuantity(object obj)
        {
            Models.Models.ClientOrderItem clientOrderProductItem = (Models.Models.ClientOrderItem) obj;
            if (clientOrderProductItem.Quantity - 1 >= 0) clientOrderProductItem.Quantity--;
            refreshItem(clientOrderProductItem);
        }

        private void refreshItem(ClientOrderItem clientOrderProductItem)
        {
            int index = ClientOrder.ClientOrderItems.IndexOf(clientOrderProductItem);
            if(index >= 0)
            {
                ClientOrder.ClientOrderItems.RemoveAt(index);
                ClientOrder.ClientOrderItems.Insert(index, clientOrderProductItem);
                OnPropertyChanged("ClientOrder");
            }
        }

        private async void ExecutePassOrder(object obj)
        {
            if (ClientOrder.Client == null)
            {
                updateResponseMessage("Il faut choisir un client pour passer commande", new SolidColorBrush(Color.FromArgb(255, 255, 77, 77)));
                return;
            }

            List<Models.Models.ClientOrderItem> clientOrderProducts = getClientOrderItemsAvailable();

            if (clientOrderProducts.Count == 0)
            {
                updateResponseMessage("Il faut au minimum un produit pour passer commande", new SolidColorBrush(Color.FromArgb(255, 255, 77, 77)));
                return;
            }

            bool result = true;
            ClientOrder.ClientOrderItems = new ObservableCollection<Models.Models.ClientOrderItem>(clientOrderProducts);

            Debug.WriteLine(JsonConvert.SerializeObject(ClientOrder));

            if (ClientOrder.Id != default)
            {
                result = await RestClient.Instance.Put<Models.Models.ClientOrder>(ClientOrder, ClientOrder.Id);
            }
            else
            {
                result = await RestClient.Instance.Post<Models.Models.ClientOrder>(ClientOrder);
            }

            Action a = () => ScreenManager.Instance.ShowScreen(typeof(Views.Client.Index));

            if (result)
                updateResponseMessage("La vente a bien été enregistrée", new SolidColorBrush(Color.FromArgb(255, 0, 204, 68)), a);
            else
                updateResponseMessage("La vente n'a pas pu aboutir", new SolidColorBrush(Color.FromArgb(255, 255, 77, 77)), a);
        }

        private List<Models.Models.ClientOrderItem> getClientOrderItemsAvailable()
        {
            List<Models.Models.ClientOrderItem> clientOrderProducts = new List<Models.Models.ClientOrderItem>();
            foreach (Models.Models.ClientOrderItem clientOrderProductItem in ClientOrder.ClientOrderItems)
            {
                if (clientOrderProductItem.Quantity > 0)
                {
                    clientOrderProducts.Add(clientOrderProductItem);
                }
            }

            return clientOrderProducts;
        }

        private async void ExecuteAddClientClick(object obj)
        {
            AddClientDialog dialog = new AddClientDialog();
            await dialog.ShowAsync();
            loadClients();
        }

        private async void loadClients()
        {
            Clients = new ObservableCollection<Models.Models.Client>((await RestClient.Instance.GetAll<Models.Models.Client>("client")).model);
            if (Clients.Count > 0) ClientOrder.Client = Clients[0];
            OnPropertyChanged("ClientOrder");
            OnPropertyChanged("Clients");
        }
    }
}
