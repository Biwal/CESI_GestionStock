using FontAwesome.UWP;
using Negosud.ViewModels;
using System;

namespace Negosud.Items
{
    class MenuItem : ViewModelBase
    {
        public string Title { get; set; }

        public FontAwesomeIcon Icon { get; set; }

        public Type NavigateTo { get; set; }
    }
}
