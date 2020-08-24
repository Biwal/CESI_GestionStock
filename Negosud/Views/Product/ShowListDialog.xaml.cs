using Negosud.ViewModels.Product;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Negosud.Views.Product
{
    public sealed partial class ShowListDialog : ContentDialog
    {
        public ShowListDialog(string title, List<Models.Models.Product> products)
        {
            DataContext = new ShowListDialogViewModel(title, products);
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
