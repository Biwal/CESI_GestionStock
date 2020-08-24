using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Negosud.Services
{
    public class ScreenManager
    {
        private static ScreenManager _instance;
        public static ScreenManager Instance {
            get {
                if(_instance == null)
                {
                    _instance = new ScreenManager();
                }

                return _instance;
            }
            set {
                _instance = value;
            }
        }

        public Frame FrameContent { get; set; }
        
        public void ShowScreen(Type clazz, object parameter = null)
        {
            if(FrameContent != null)
            {
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { FrameContent.Navigate(clazz, parameter); });
            }    
        }
    }
}
