using ProjektLavor.Commands;
using ProjektLavor.Services;
using ProjektLavor.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Xceed.Wpf.AvalonDock.Controls;

namespace ProjektLavor.ViewModels
{
    public class TemplateBrowserViewModel : ViewModelBase
    {
        private ProjectStore _projectStore;
        private INavigationService _navigationService;

        public ObservableCollection<KeyValuePair<Image, PageContent>> Items { get; set; }

        private KeyValuePair<Image, PageContent> _selectedItem;
        public KeyValuePair<Image, PageContent> SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public ICommand CloseModalCommand { get; set; }
        public ICommand OkTemplateModalCommand { get; set; }

        public TemplateBrowserViewModel(ProjectStore projectStore, INavigationService navigationService, ObservableCollection<KeyValuePair<Image, PageContent>> items = null)
        {
            _projectStore = projectStore;
            _navigationService = navigationService;

            Items = items ?? new ObservableCollection<KeyValuePair<Image, PageContent>>();

            CloseModalCommand = new NavigateCommand(navigationService);
            OkTemplateModalCommand = new OkTemplateModalCommand(this);

            OnPropertyChanged(nameof(Items));
        }

        public void AddPage()
        {
            _projectStore.NewPage();

            var newPage = _projectStore.CurrentProject.Document.Pages[_projectStore.CurrentProject.Document.Pages.Count - 1];
            var newPageChildren = newPage.Child.Children;
            var childrenToAdd = SelectedItem.Value.Child.Children;
            var childrenToAddCopy = new List<UIElement>(childrenToAdd.Cast<UIElement>());

            newPageChildren.Clear();
            foreach (var child in childrenToAddCopy)
            {
                if (child is AdornerDecorator imageParent)
                {
                    ((Image)imageParent.Child).Tag = Guid.NewGuid();
                }
                childrenToAdd.Remove(child);
                newPageChildren.Add(child);
            }
            childrenToAddCopy.Clear();

            _projectStore.ClearUndoRedoStacks();

            _navigationService.Navigate();
        }
    }
}
