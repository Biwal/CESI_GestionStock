using Negosud.Services;
using System;
using System.Diagnostics;
using System.Threading;
using Windows.UI.Xaml;

namespace Negosud.ViewModels.Auth
{
    class AuthViewModel : ViewModelBase
    {
        private Timer timer;

        public string Username { get; set; } = "Nom d'utilisateur";
        public string Password { get; set; } = "Mot de passe";
        public string ErrorMessage { get; set; }

      
        public double CanvasHeight { get; set; } = 450;
        public double CanvasWidth { get; set; } = 850;

        public DelegateCommand LoginCommand { get; set; }
        public DelegateCommand SizeChanged { get; set; }

        public AuthViewModel()
        {
            LoginCommand = new DelegateCommand(ExecuteLogin);
            SizeChanged = new DelegateCommand(ExecuteSizeChanged);
        }

        private void ExecuteSizeChanged(object obj)
        {
            SizeChangedEventArgs args = (SizeChangedEventArgs)obj;
            CanvasHeight = args.NewSize.Height / 4;
            CanvasWidth = args.NewSize.Width;
            OnPropertyChanged("CanvasWidth");
            OnPropertyChanged("CanvasHeight");
        }

        private async void ExecuteLogin(object obj)
        {
           /* bool isAuth = await RestClient.Instance.Authenticate(Username, Password);
            if (isAuth)
            {
                switchToHomeScreen();
            }
            else
            {
                ErrorMessage = "Nom d'utilisateur ou mot de passe erroné";
                OnPropertyChanged("ErrorMessage");
            }*/
        }

        private void switchToHomeScreen()
        {
            Timer timer = new Timer(test, null, 0, 1);
        }

        private async void test(object state)
        {
            double height = CanvasHeight;
            height += 20;
            if (height >= 1100)
            {
                timer.Dispose();
                //
            }

            CanvasHeight = height;
            OnPropertyChanged("CanvasHeight");
        }
    }
}
