using Negosud.Services;
using Negosud.Views.Product;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;

namespace Negosud.ViewModels.Product
{
    class IndexViewModel : ViewModelBase
    {
        public ObservableCollection<Models.Models.Product> Products { get; set; }

        public DelegateCommand AddProductCommand { get; set; }
        public DelegateCommand EditProductCommand { get; set; }

        private Models.Models.Product selectedProduct;

        public Models.Models.Product SelectedProduct
        {
            get { return selectedProduct; }
            set { selectedProduct = value; OnPropertyChanged("SelectedProduct"); }
        }

        public IndexViewModel()
        {
            AddProductCommand = new DelegateCommand(executeAddProduct);
            EditProductCommand = new DelegateCommand(executeEditProduct);
            loadProducts();
        }

        public async void executeEditProduct(object obj)
        {
            EditProductDialog editProductDialog = new EditProductDialog(SelectedProduct);
            Models.Models.Product copiedProduct = SelectedProduct;
            ContentDialogResult result = await editProductDialog.ShowAsync();
            if(result == ContentDialogResult.Primary) refreshItem(copiedProduct);
        }

        private void refreshItem(Models.Models.Product product)
        {
            int index = Products.IndexOf(product);
            Products.RemoveAt(index);
            Products.Insert(index, product);
        }

        private async void executeAddProduct(object obj)
        {
            EditProductDialog dialog = new EditProductDialog();
            ContentDialogResult result = await dialog.ShowAsync();
            if(result == ContentDialogResult.Primary) loadProducts();
        }

        private async void loadProducts()
        {
            Products = new ObservableCollection<Models.Models.Product>((await RestClient.Instance.GetAll<Models.Models.Product>("product")).model);
            SelectedProduct = Products.FirstOrDefault();
            OnPropertyChanged("Products");
        }
    }
}
