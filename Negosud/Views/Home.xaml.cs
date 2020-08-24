using Negosud.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Negosud.Views
{
    public sealed partial class Home : Page
    {
        public Home()
        {
            DataContext = new HomeViewModel();
            this.InitializeComponent();
        }
     }
}
