using Negosud.ViewModels.Client;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Negosud.Views.Client
{
    public sealed partial class ClientOrder : Page
    {
        private ClientOrderViewModel clientOrderViewModel;

        public ClientOrder()
        {
            clientOrderViewModel = new ClientOrderViewModel();
            DataContext = clientOrderViewModel;
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Models.Models.ClientOrder clientOrder = (Models.Models.ClientOrder)e.Parameter;
            if (clientOrder != null)
            {
                clientOrderViewModel.SetClientOrder(clientOrder);
            }
        }
    }
}
