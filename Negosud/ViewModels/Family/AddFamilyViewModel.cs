using Negosud.Services;

namespace Negosud.ViewModels.Family
{
    public class AddFamilyViewModel
    {

        public Models.Models.Family Family { get; set; } = new Models.Models.Family();
        public DelegateCommand EditFamilyCommand { get; set; }

        public AddFamilyViewModel()
        {
            EditFamilyCommand = new DelegateCommand(executeEditFamily, canExecuteEditFamily);
        }

        private bool canExecuteEditFamily(object obj)
        {
            return Family.Name != null;
        }

        private async void executeEditFamily(object obj)
        {
            if (Family.Id == default)
            {
                await RestClient.Instance.Post<Models.Models.Family>(Family);
            }
            else
            {
                await RestClient.Instance.Post<Models.Models.Family>(Family);
            }
        }
    }
}
