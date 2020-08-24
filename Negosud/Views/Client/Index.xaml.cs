using Negosud.ViewModels.Client;
using Windows.UI.Xaml.Controls;

namespace Negosud.Views.Client
{
    public sealed partial class Index : Page
    {
        public Index()
        {
            DataContext = new IndexViewModel();
            this.InitializeComponent();
        }
    }
}
