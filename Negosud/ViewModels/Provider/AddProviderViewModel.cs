using Negosud.Services;

namespace Negosud.ViewModels.Provider
{
    class AddProviderViewModel
    {
        public Models.Models.Provider Provider { get; set; } = new Models.Models.Provider();

        public DelegateCommand EditProviderCommand { get;set; }

        public AddProviderViewModel()
        {
            EditProviderCommand = new DelegateCommand(executeEditProvider, canExecuteEditProvider);
        }

        private bool canExecuteEditProvider(object obj)
        {
            return Provider.Lastname != null && Provider.Firstname != null && Provider.Address != null;
        }

        private async void executeEditProvider(object obj)
        {
            if (Provider.Id == default)
            {
                await RestClient.Instance.Post<Models.Models.Provider>(Provider);
            } else
            {
                await RestClient.Instance.Post<Models.Models.Provider>(Provider);
            }
        }
    }
}
