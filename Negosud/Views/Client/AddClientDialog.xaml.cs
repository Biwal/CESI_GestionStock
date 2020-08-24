using Negosud.Services;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Negosud.Views.Client
{
    public sealed partial class AddClientDialog : ContentDialog
    {
        public AddClientDialog()
        {
            this.InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();
            Models.Models.Client client = new Models.Models.Client
            {
                Firstname =  (Firstname.Text != "") ? Firstname.Text : null,
                Lastname = (Lastname.Text != "") ? Lastname.Text : null,
                Phone = (Phone.Text != "") ? Phone.Text : null,
                Email = (Email.Text != "") ? Email.Text : null,
            };
           
            args.Cancel = await ValidateForm(client);

            if (!args.Cancel) { 
                bool Response = await RestClient.Instance.Post<Models.Models.Client>(client); 
            }

            deferral.Complete();
        }

        private async Task<bool> ValidateForm(Models.Models.Client client)
        {
            if (client.Firstname == null || client.Lastname == null)
            {
                var dialog = new MessageDialog("Veuillez indiquer les champs obligatoires !");
                await dialog.ShowAsync();
                return true;
            }

            return false;
        }

        private void Integer_OnBeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if (args.NewText == "") return;
            bool result = int.TryParse(args.NewText, out int intResult);
            if (result == false || args.NewText.Contains(" "))
                args.Cancel = true;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}

