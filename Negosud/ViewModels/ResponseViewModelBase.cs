using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Media;

namespace Negosud.ViewModels
{
    public class ResponseViewModelBase : ViewModelBase
    {
        public string ResponseMessage { get; set; }
        public Brush ResponseMessageColor { get; set; }

        protected void updateResponseMessage(string msg, Brush color)
        {
            setResponseMessage(msg, color);
            Task.Delay(2000).ContinueWith(t => resetResponseMessage());
        }

        protected void updateResponseMessage(string msg, Brush color, Action runnable)
        {
            setResponseMessage(msg, color);
            Task.Delay(2000).ContinueWith(t => {
                resetResponseMessage();
                runnable.Invoke();
            });
        }

        private void setResponseMessage(string msg, Brush color)
        {
            ResponseMessage = msg;
            ResponseMessageColor = color;

            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                OnPropertyChanged("ResponseMessage");
                OnPropertyChanged("ResponseMessageColor");
            });
        }

        private void resetResponseMessage()
        {
            setResponseMessage("", null);
        }
    }
}
