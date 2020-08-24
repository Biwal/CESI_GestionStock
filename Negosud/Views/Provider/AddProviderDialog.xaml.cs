using Negosud.ViewModels.Provider;
using Windows.UI.Xaml.Controls;

namespace Negosud.Views.Provider
{
    public sealed partial class AddProviderDialog : ContentDialog
    {
        public AddProviderDialog()
        {
            DataContext = new AddProviderViewModel();
            this.InitializeComponent();
        }
    }
}
