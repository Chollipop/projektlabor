using ProjektLavor.Commands;
using ProjektLavor.Services;
using ProjektLavor.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ProjektLavor.ViewModels
{
    public class CollectionBrowserViewModel : ViewModelBase
    {
        public List<Image> Items { get; set; }
        public Image? SelectedImage { get; set; }

        public ICommand CloseModalCommand { get; set; }
        public ICommand OkCollectionModalCommand { get; set; }

        public CollectionBrowserViewModel(ProjectStore projectStore, INavigationService navigationService, SelectedElementStore selectedElementStore, List<Image>? items = null, bool isFrame = false)
        {
            Items = items ?? new List<Image>();

            CloseModalCommand = new NavigateCommand(navigationService);
            OkCollectionModalCommand = new OkCollectionModalCommand(this, navigationService, projectStore, isFrame ? selectedElementStore : null);

            OnPropertyChanged(nameof(Items));
        }
    }
}
