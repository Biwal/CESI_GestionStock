using Negosud.ViewModels.Product;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Negosud.Views.Product
{
    public sealed partial class Index : Page
    {
        public Index()
        {
            DataContext = new IndexViewModel();
            this.InitializeComponent();
        }

        private void windowSizeChanged(object sender, RoutedEventArgs e)
        {
            ListProducts.MaxHeight = ((Frame)Window.Current.Content).ActualHeight - 48;
        }
    }
}
