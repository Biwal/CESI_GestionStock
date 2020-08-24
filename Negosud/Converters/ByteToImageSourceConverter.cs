using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Negosud.Converters
{
    public class ByteToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            byte[] imageBytes = (byte[])value;

            Task<ImageSource> task;
            if (imageBytes != null)
            {
                task = GetImageSource(imageBytes);
            } else
            {
                task = GetDefaultImageSource("/Assets/Product/Vin.png");
            }

            return new TaskCompletionNotifier<ImageSource>(task);
        }

        private async Task<ImageSource> GetDefaultImageSource(string url)
        {
            return new BitmapImage(new Uri("ms-appx:///Assets/Products/Vin.png", UriKind.RelativeOrAbsolute));
        }

        private async Task<ImageSource> GetImageSource(Byte[] bytes)
        {
            try
            {
                using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
                {
                    using (DataWriter writer = new DataWriter(ms.GetOutputStreamAt(0)))
                    {
                        writer.WriteBytes(bytes);
                        await writer.StoreAsync();
                    }

                    var image = new BitmapImage();
                    await image.SetSourceAsync(ms);
                    Debug.WriteLine("Return Image");
                    return image;
                }
            } catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
                return null;
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            return null;
        }
    }

    public sealed class TaskCompletionNotifier<TResult> : INotifyPropertyChanged
    {
        public TaskCompletionNotifier(Task<TResult> task)
        {
            Task = task;
            if (!task.IsCompleted)
            {
                var scheduler = (SynchronizationContext.Current == null) ? TaskScheduler.Current : TaskScheduler.FromCurrentSynchronizationContext();
                task.ContinueWith(t =>
                {
                    var propertyChanged = PropertyChanged;
                    if (propertyChanged != null)
                    {
                        propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
                        if (t.IsCanceled)
                        {
                            propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
                        }
                        else if (t.IsFaulted)
                        {
                            propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
                            propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
                        }
                        else
                        {
                            propertyChanged(this, new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
                            propertyChanged(this, new PropertyChangedEventArgs("Result"));
                        }
                    }
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                scheduler);
            }
        }

        public Task<TResult> Task { get; private set; }

        public TResult Result { get { return (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default(TResult); } }

        public bool IsCompleted { get { return Task.IsCompleted; } }

        public bool IsSuccessfullyCompleted { get { return Task.Status == TaskStatus.RanToCompletion; } }

        public bool IsCanceled { get { return Task.IsCanceled; } }

        public bool IsFaulted { get { return Task.IsFaulted; } }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
