using Negosud.ViewModels.Family;
using Windows.UI.Xaml.Controls;

namespace Negosud.Views.Family
{
    public sealed partial class AddFamilyDialog : ContentDialog
    {
        public AddFamilyDialog()
        {
            DataContext = new AddFamilyViewModel();
            this.InitializeComponent();
        }
    }
}
