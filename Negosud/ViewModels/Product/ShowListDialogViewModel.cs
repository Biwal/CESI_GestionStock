using Negosud.Views.Product;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Negosud.ViewModels.Product
{
	class ShowListDialogViewModel : ViewModelBase
	{
		public ObservableCollection<Models.Models.Product> Products { get; set; } = new ObservableCollection<Models.Models.Product>();
		public Models.Models.Product SelectedProduct { get; set; }

		public string Title { get; set; }
		
		public ShowListDialogViewModel(string title = null, List<Models.Models.Product> products = null)
		{
			if (title != null) Title = title;
			if (products != null) Products = new ObservableCollection<Models.Models.Product>(products);
			OnPropertyChanged("Title");
			OnPropertyChanged("Products");
		}

		private void refreshItem(Models.Models.Product product)
		{
			int index = Products.IndexOf(product);
			Products.RemoveAt(index);
			Products.Insert(index, product);
		}
	}
}
