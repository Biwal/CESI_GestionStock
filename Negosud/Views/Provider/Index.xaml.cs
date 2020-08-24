using Negosud.ViewModels.Provider;
using Windows.UI.Xaml.Controls;

namespace Negosud.Views.Provider
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
