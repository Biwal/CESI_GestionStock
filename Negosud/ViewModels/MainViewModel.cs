using FontAwesome.UWP;
using Negosud.Items;
using Negosud.Views;
using Negosud.Views.Client;
using Negosud.Views.Product;
using Negosud.Views.Provider;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Negosud.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        public ObservableCollection<MenuItem> MenuItems { get; set; }

        private MenuItem selectedMenuItem;

        public MenuItem SelectedMenuItem
        {
            get { return selectedMenuItem; }
            set { selectedMenuItem = value; OnPropertyChanged("SelectedMenuItem"); }
        }

        public MainViewModel()
        {
            MenuItems = new ObservableCollection<MenuItem>(getMenuItems());
            SelectedMenuItem = MenuItems.FirstOrDefault();
        }

        private List<MenuItem> getMenuItems()
        {
            List<MenuItem> menuItems = new List<MenuItem>();
            menuItems.Add(new MenuItem() { Title = "Accueil", Icon = FontAwesomeIcon.Home, NavigateTo = typeof(Home) });
            menuItems.Add(new MenuItem() { Title = "Stock", Icon = FontAwesomeIcon.Inbox, NavigateTo = typeof(Views.Product.Index) });
            menuItems.Add(new MenuItem() { Title = "Ventes", Icon = FontAwesomeIcon.Opencart, NavigateTo = typeof(Views.Client.Index) });
            menuItems.Add(new MenuItem() { Title = "Commandes", Icon = FontAwesomeIcon.ShoppingCart, NavigateTo = typeof(Views.Provider.Index) });

            return menuItems;
        }
    }
}
