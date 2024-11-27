using Microsoft.Win32;
using ProjektLavor.Commands;
using ProjektLavor.Services;
using ProjektLavor.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
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
using System.Xml.Linq;
using Xceed.Wpf.AvalonDock.Controls;
using static ProjektLavor.Stores.ProjectStore;

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
            try
            {
                XDocument xDocument = XDocument.Parse(xaml);
                XElement fixedPageElement = (XElement)xDocument.Root.FirstNode;
                FixedPage fixedPage;

                using (XmlReader reader = fixedPageElement.CreateReader())
                {
                    fixedPage = (FixedPage)System.Windows.Markup.XamlReader.Load(reader);
                }

                foreach (var child in fixedPage.Children)
                {
                    if (child is AdornerDecorator decorator)
                    {
                        var image = (Image)decorator.Child;
                        bool isIgnoreWizard = image.Tag?.ToString() == "ignore_wizard";
                        if (!isIgnoreWizard) image.Source = new BitmapImage(new Uri("Pack://application:,,,/Assets/Templates/placeholder.png"));
                        image.ContextMenu = _projectStore.CreateImageContextMenu(image, isIgnoreWizard);
                    }
                }

                foreach (var adornerElement in xDocument.Root.Elements("FrameAdornerState"))
                {
                    var frameAdornerState = new FrameAdornerState
                    {
                        AdornedElement = adornerElement.Element("AdornedElement").Value,
                        SourceUri = adornerElement.Element("SourceUri").Value
                    };

                    foreach (FrameworkElement e in fixedPage.Children)
                    {
                        FrameworkElement element = e;
                        if (element is AdornerDecorator) element = (FrameworkElement)((AdornerDecorator)element).Child;

                        if (element is Image image && image.Tag.ToString() == frameAdornerState.AdornedElement)
                        {
                            _projectStore.adorners.Add(new Tuple<string, Image, BitmapImage>(fixedPage.Tag.ToString(), image, new BitmapImage(new Uri(frameAdornerState.SourceUri))));
                        }
                    }
                }

                return fixedPage;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public void AddPage()
        {
            UIElementCollection childrenToAdd = null;
            FixedPage loadedPage = new FixedPage() { Background = Brushes.White, Tag = Guid.NewGuid() };

            if (SelectedItem.Value.Child.Tag.ToString() == "custom_template")
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Sablonfájlok (*.template)|*.template",
                    DefaultExt = ".template",
                    AddExtension = true
                };

                bool? result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    string filePath = openFileDialog.FileName;
                    string serializedPage = File.ReadAllText(filePath);
                    loadedPage = DeserializeFixedPage(serializedPage);

                    if (loadedPage == null) return;

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
