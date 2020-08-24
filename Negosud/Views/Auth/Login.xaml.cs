using Negosud.ViewModels.Auth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Negosud.Views.Auth
{
    public sealed partial class Login : Page
    {
        public Login()
        {
            DataContext = new AuthViewModel();
            this.InitializeComponent();
        }

        private void textBoxGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox source = sender as TextBox;
            if (source != null && source.Text.Contains(" "))
            {
                source.Text = "";
            }
        }
    }
}
