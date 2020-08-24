using Negosud.ViewModels.Provider;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Negosud.Views.Provider
{
    public sealed partial class ProviderOrder : Page
    {
        private ProviderOrderViewModel providerOrderViewModel;

        public ProviderOrder()
        {
            providerOrderViewModel = new ProviderOrderViewModel();
            DataContext = providerOrderViewModel;
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Models.Models.ProviderOrder providerOrder = (Models.Models.ProviderOrder)e.Parameter;
            if(providerOrder != null)
            {
                providerOrderViewModel.SetProviderOrder(providerOrder);
            }
        }
    }
}
