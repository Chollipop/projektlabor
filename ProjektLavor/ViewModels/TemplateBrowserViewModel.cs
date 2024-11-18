using Microsoft.Win32;
using ProjektLavor.Commands;
using ProjektLavor.Services;
using ProjektLavor.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml;
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

        private FixedPage DeserializeFixedPage(string xaml)
        {
            using (var stringReader = new StringReader(xaml))
            {
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    var fixedPage = (FixedPage)XamlReader.Load(xmlReader);
                    foreach (var child in fixedPage.Children)
                    {
                        if (child is AdornerDecorator decorator)
                        {
                            var image = (Image)decorator.Child;
                            image.Source = new BitmapImage(new Uri("Pack://application:,,,/Assets/Templates/placeholder.png"));
                            image.ContextMenu = _projectStore.CreateImageContextMenu(image);
                        }
                    }
                    return fixedPage;
                }
            }
        }

        public void AddPage()
        {
            UIElementCollection childrenToAdd = null;
            FixedPage loadedPage = new FixedPage() { Background = Brushes.White, Tag = Guid.NewGuid() };

            if (SelectedItem.Value.Child.Tag == "custom_template")
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Template files (*.template)|*.template",
                    DefaultExt = ".template",
                    AddExtension = true
                };

                bool? result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    string filePath = openFileDialog.FileName;
                    string serializedPage = File.ReadAllText(filePath);
                    loadedPage = DeserializeFixedPage(serializedPage);
                    childrenToAdd = loadedPage.Children;
                }
            }
            else
            {
                childrenToAdd = SelectedItem.Value.Child.Children;
            }

            if (childrenToAdd != null)
            {
                _projectStore.NewPage();

                var newPage = _projectStore.CurrentProject.Document.Pages[_projectStore.CurrentProject.Document.Pages.Count - 1];
                var newPageChildren = newPage.Child.Children;
                var childrenToAddCopy = new List<UIElement>(childrenToAdd.Cast<UIElement>());

                newPage.Child.Background = loadedPage.Background;
                newPage.Child.Tag = loadedPage.Tag;

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
            }

            _navigationService.Navigate();
        }
    }
}
