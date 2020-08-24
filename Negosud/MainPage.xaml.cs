using Negosud.Items;
using Negosud.Services;
using Negosud.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Negosud
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            DataContext = new MainViewModel();
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            ScreenManager.Instance.FrameContent = FrameContent;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            MenuGrid_Tapped(null, null);
        }

        private void MenuGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (NavigationPane.IsPaneOpen)
                NavigationPane.IsPaneOpen = !NavigationPane.IsPaneOpen;

            MenuItem menu = LeftMenu.SelectedItem as MenuItem;
            if (menu != null)
            {
                if (menu.NavigateTo != null)
                {
                    ScreenManager.Instance.ShowScreen(menu.NavigateTo);
                }
           
            }
        }
    }
}
