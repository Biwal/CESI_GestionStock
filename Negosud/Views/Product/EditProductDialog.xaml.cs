using Microsoft.Toolkit.Uwp.UI.Controls;
using Negosud.ViewModels.Product;
using Windows.UI.Xaml.Controls;

namespace Negosud.Views.Product
{
    public sealed partial class EditProductDialog : ContentDialog
    {
        private EditProductDialogViewModel viewModel;
        private Models.Models.Product product;

        public EditProductDialog(Models.Models.Product product = null)
        {
            this.product = product;
            this.Initialize();
        }

        private void Initialize()
        {
            viewModel = new EditProductDialogViewModel(product);
            DataContext = viewModel;
            this.InitializeComponent();
            if(product != null) viewModel.UpdateRangeSelector(StockAvailable);
        }

        private void Double_OnBeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if (args.NewText == "") return;
            bool result = double.TryParse(args.NewText, out double doubleResult);
            if (result == false || args.NewText.Contains(" "))
                args.Cancel = true;

        }

        private void Integer_OnBeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if (args.NewText == "") return;
            bool result = int.TryParse(args.NewText, out int intResult);
            if (result == false || args.NewText.Contains(" "))
                args.Cancel = true;
        }

        private void rangeSelectorValueChanged(object obj, RangeChangedEventArgs e)
        {
            viewModel.RangeSelectorValuesChanged(StockAvailable);
        }
    }
}

