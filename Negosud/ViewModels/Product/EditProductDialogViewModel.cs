using Microsoft.Toolkit.Uwp.UI.Controls;
using Negosud.Services;
using Negosud.Views.Family;
using Negosud.Views.Provider;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Negosud.ViewModels.Product
{
    class EditProductDialogViewModel: ViewModelBase
    {
		public Models.Models.Product Product { get; set; } = new Models.Models.Product();

        public ObservableCollection<Models.Models.Family> Families { get; set; }
		public ObservableCollection<Models.Models.Provider> Providers { get; set; }

		public DelegateCommand AddProductCommand { get; set; }
		public DelegateCommand AddProviderCommand { get; set; }
		public DelegateCommand AddFamilyCommand { get; set; }
		public DelegateCommand AddPhotoCommand { get; set; }

		public EditProductDialogViewModel(Models.Models.Product product = null)
		{
			AddProductCommand = new DelegateCommand(ExecuteAddProduct);
			AddProviderCommand = new DelegateCommand(ExecuteAddProvider);
			AddFamilyCommand = new DelegateCommand(ExecuteAddFamily);
			AddPhotoCommand = new DelegateCommand(ExecuteAddPhoto);
			loadFamilies();
			loadProviders();

			if (product != null) setupProduct(product);
		}

		public void UpdateRangeSelector(RangeSelector rs)
		{
			rs.RangeMin = Product.MinStockAvailable;
			rs.RangeMax = Product.MaxStockAvailable;
		}

		private void setupProduct(Models.Models.Product product)
		{
			Product = product;
			OnPropertyChanged("Product");
		}

		public void RangeSelectorValuesChanged(RangeSelector rs)
		{
			Product.MinStockAvailable = (int)rs.RangeMin;
			Product.MaxStockAvailable = (int)rs.RangeMax;
		}

		private async void ExecuteAddPhoto(object sender)
		{
			FileOpenPicker picker = new FileOpenPicker();
			picker.ViewMode = PickerViewMode.Thumbnail;
			picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
			picker.FileTypeFilter.Add(".jpg");
			picker.FileTypeFilter.Add(".jpeg");
			picker.FileTypeFilter.Add(".png");
			StorageFile file = await picker.PickSingleFileAsync();
			if (file != null)
			{
				byte[] bytes = await ConvertImageToByte(file);
				Product.Image = bytes;
				OnPropertyChanged("Product");
			}
		}

		private async void ExecuteAddProvider(object obj)
		{
			ContentDialog contentDialog = (ContentDialog)obj;
			contentDialog.Hide();
			var dialog = new AddProviderDialog();
			await dialog.ShowAsync();
			loadProviders();
			await contentDialog.ShowAsync();
		}

		private async void ExecuteAddFamily(object obj)
		{
			ContentDialog contentDialog = (ContentDialog)obj;
			contentDialog.Hide();
			var dialog = new AddFamilyDialog();
			await dialog.ShowAsync();
			loadFamilies();
			await contentDialog.ShowAsync();
		}

		private async void ExecuteAddProduct(object obj)
		{
			Debug.WriteLine(JsonConvert.SerializeObject(Product));
			bool validated = await validateForm();
			if (validated)
			{
				if (Product.Id == default(int))
				{
					bool response = await RestClient.Instance.Post<Models.Models.Product>(Product);
				}
				else
				{
					Debug.WriteLine(JsonConvert.SerializeObject(Product));
					bool response = await RestClient.Instance.Put<Models.Models.Product>(Product, Product.Id);
					Debug.WriteLine("Response " + response);
				}
			}
		}

		private async Task<bool> validateForm()
		{
			if (Product.Name == null || Product.Provider == null || Product.Family == null || Product.Price == 0 || (Product.MinStockAvailable < 0 && Product.MaxStockAvailable < Product.MinStockAvailable))
			{
				var dialog = new MessageDialog("Veuillez indiquer les champs obligatoires !");
				await dialog.ShowAsync();
				return false;
			}
			return true;
		}

		private async void loadProviders()
		{
			Providers = new ObservableCollection<Models.Models.Provider>((await RestClient.Instance.GetAll<Models.Models.Provider>("provider")).model);
			if (Providers.Count > 0) Product.Provider = Providers[0];
			OnPropertyChanged("Providers");
		}

		private async void loadFamilies()
		{
			Families = new ObservableCollection<Models.Models.Family>((await RestClient.Instance.GetAll<Models.Models.Family>("family")).model);
			if (Families.Count > 0) Product.Family = Families[0];
			OnPropertyChanged("Families");
		}

		private async Task<byte[]> ConvertImageToByte(StorageFile file)
		{
			using (var inputStream = await file.OpenSequentialReadAsync())
			{
				Stream readStream = inputStream.AsStreamForRead();
				byte[] byteArray = new byte[readStream.Length];
				await readStream.ReadAsync(byteArray, 0, byteArray.Length);
				return byteArray;
			}

		}
	}
}
