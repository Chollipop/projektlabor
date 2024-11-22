using System;
using ProjektLavor.Commands;
using ProjektLavor.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ProjektLavor.Stores;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Reflection.Metadata;
using System.Windows.Controls;
using System.Data.Common;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Markup;
using System.Windows;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace ProjektLavor.ViewModels
{
    public class ToolbarViewModel : ViewModelBase
    {
        private ProjectStore _projectStore;
        private SelectedElementStore _selectedElementStore;

        public ICommand ChangeColorCommand { get; set; }
        public ICommand RunWizardCommand { get; set; }
        public ICommand NewImageElementCommand { get; set; }
        public ICommand OpenNewFrameModalCommand { get; set; }
        public ICommand OpenNewTextElementModalCommand { get; set; }
        public ICommand OpenNewTextBubbleModalCommand { get; set; }
        public ICommand OpenNewTemplateModalCommand { get; set; }
        public ICommand CustomColorCommand { get; set; }

        private Nullable<Color> _selectedCustomColor;
        public Nullable<Color> SelectedCustomColor
        {
            get => _selectedCustomColor;
            set
            {
                _selectedCustomColor = value;
                if (_projectStore?.CurrentProject?.Document == null || value == null) return;

                var activePage = _projectStore.CurrentProject.ActivePage;
                var selectedElement = _selectedElementStore.SelectedElement;

                if (selectedElement != null && selectedElement.GetType() == typeof(TextBlock))
                {
                    ((TextBlock)selectedElement).Foreground = new SolidColorBrush(value.Value);
                }
                else if (activePage != null)
                {
                    activePage.Background = new SolidColorBrush(value.Value);
                }
            }
        }

        public ToolbarViewModel(IServiceProvider serviceProvider)
        {
            ModalNavigationStore modalNavigationStore = serviceProvider.GetRequiredService<ModalNavigationStore>();
            _projectStore = serviceProvider.GetRequiredService<ProjectStore>();
            _selectedElementStore = serviceProvider.GetRequiredService<SelectedElementStore>();

            ChangeColorCommand = new ChangeColorCommand(_projectStore, _selectedElementStore);

            OpenNewFrameModalCommand = new NavigateCommand(
                new ModalNavigationService<CollectionBrowserViewModel>(
                    modalNavigationStore,
                    () => _projectStore.CurrentProject == null ? null : new CollectionBrowserViewModel(
                        _projectStore,
                        new CloseModalNavigationService(modalNavigationStore),
                        _selectedElementStore,
                        new List<Image>()
                        {
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame4.png"))},
                        }, true
                    )
                ));
            OpenNewTextBubbleModalCommand = new NavigateCommand(
                new ModalNavigationService<CollectionBrowserViewModel>(
                    modalNavigationStore,
                    () => _projectStore.CurrentProject == null ? null : new CollectionBrowserViewModel(
                        _projectStore,
                        new CloseModalNavigationService(modalNavigationStore),
                        _selectedElementStore,
                        new List<Image>()
                        {
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble1.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                        }
                    )
                ));

            OpenNewTemplateModalCommand = new NavigateCommand(new ModalNavigationService<TemplateBrowserViewModel>(modalNavigationStore,
                () => _projectStore.CurrentProject == null ? null : new TemplateBrowserViewModel(
                _projectStore,
                new CloseModalNavigationService(modalNavigationStore),
                new ObservableCollection<KeyValuePair<Image, PageContent>>()
                {
                    new KeyValuePair<Image, PageContent>(new Image() { Source = new BitmapImage(new Uri("Pack://application:,,,/Assets/Templates/template1.png")) }, new PageContent() { Child = DeserializeFixedPage(template1Xml) }),
                    new KeyValuePair<Image, PageContent>(new Image() { Source = new BitmapImage(new Uri("Pack://application:,,,/Assets/Templates/template2.png")) }, new PageContent() { Child = DeserializeFixedPage(template2Xml) }),
                    new KeyValuePair<Image, PageContent>(new Image() { Source = new BitmapImage(new Uri("Pack://application:,,,/Assets/Templates/template3.png")) }, new PageContent() { Child = DeserializeFixedPage(template3Xml) }),
                    new KeyValuePair<Image, PageContent>(new Image() { Source = new BitmapImage(new Uri("Pack://application:,,,/Assets/Templates/template4.png")) }, new PageContent() { Child = DeserializeFixedPage(template4Xml) }),
                    new KeyValuePair<Image, PageContent>(new Image() { Source = new BitmapImage(new Uri("Pack://application:,,,/Assets/Templates/plus-symbol-button.png")) }, new PageContent() { Child = new FixedPage() { Tag = "custom_template" } })
                })));

            //OpenNewTextElementModalCommand = new NavigateCommand(newTextElementNavigationService);
            OpenNewTextElementModalCommand = new NavigateCommand(
                new ModalNavigationService<TextElementInputViewModel>(
                    modalNavigationStore,
                    () => _projectStore.CurrentProject == null ? null : new TextElementInputViewModel(
                        _projectStore,
                        new CloseModalNavigationService(modalNavigationStore)
                    )
                ));
            NewImageElementCommand = new NewImageElementCommand(_projectStore);
            RunWizardCommand = new RunWizardCommand(_projectStore);
        }

        public FixedPage DeserializeFixedPage(string xaml)
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
                            bool isIgnoreWizard = image.Tag?.ToString() == "ignore_wizard";
                            if (!isIgnoreWizard) image.Source = new BitmapImage(new Uri("Pack://application:,,,/Assets/Templates/placeholder.png"));
                            image.ContextMenu = _projectStore.CreateImageContextMenu(image, isIgnoreWizard);
                        }
                    }
                    return fixedPage;
                }
            }
        }

        private string template1Xml = "<FixedPage xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" Width=\"796.8\" Height=\"1123.2\"><AdornerDecorator FixedPage.Left=\"400\" FixedPage.Top=\"30\"><Image Stretch=\"Fill\" Cursor=\"SizeAll\" /></AdornerDecorator><AdornerDecorator FixedPage.Left=\"30\" FixedPage.Top=\"318\"><Image Stretch=\"Fill\" Cursor=\"SizeAll\" /></AdornerDecorator><AdornerDecorator FixedPage.Left=\"400\" FixedPage.Top=\"606\"><Image Stretch=\"Fill\" Cursor=\"SizeAll\" /></AdornerDecorator><AdornerDecorator FixedPage.Left=\"30\" FixedPage.Top=\"894\"><Image Stretch=\"Fill\" Cursor=\"SizeAll\" /></AdornerDecorator></FixedPage>";
        private string template2Xml = "<FixedPage xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" Width=\"796.8\" Height=\"1123.2\"><AdornerDecorator FixedPage.Left=\"18.95818793223816\" FixedPage.Top=\"11.374912759342749\"><Image Stretch=\"Fill\" Width=\"350\" Height=\"730\" Cursor=\"SizeAll\" ContextMenu=\"{x:Null}\" /></AdornerDecorator><AdornerDecorator FixedPage.Left=\"423.3995304866444\" FixedPage.Top=\"11.374912759342664\"><Image Stretch=\"Fill\" Width=\"350\" Height=\"730\" Cursor=\"SizeAll\" ContextMenu=\"{x:Null}\" /></AdornerDecorator><AdornerDecorator FixedPage.Left=\"26.541463105132834\" FixedPage.Top=\"767.1746716578897\"><Image Stretch=\"Fill\" Width=\"740.1593282886074\" Height=\"338.05449775796\" Cursor=\"SizeAll\" ContextMenu=\"{x:Null}\" /></AdornerDecorator></FixedPage>";
        private string template3Xml = "<FixedPage xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" Width=\"796.8\" Height=\"1123.2\"><AdornerDecorator FixedPage.Left=\"25\" FixedPage.Top=\"20\"><Image Stretch=\"Fill\" Cursor=\"SizeAll\" ContextMenu=\"{x:Null}\" /></AdornerDecorator><AdornerDecorator FixedPage.Left=\"25\" FixedPage.Top=\"465\"><Image Stretch=\"Fill\" Cursor=\"SizeAll\" ContextMenu=\"{x:Null}\" /></AdornerDecorator><AdornerDecorator FixedPage.Left=\"24.013704714168398\" FixedPage.Top=\"245\"><Image Stretch=\"Fill\" Cursor=\"SizeAll\" ContextMenu=\"{x:Null}\" /></AdornerDecorator><AdornerDecorator FixedPage.Left=\"419.607892900196\" FixedPage.Top=\"12.638791954825138\"><Image Stretch=\"Fill\" Width=\"350\" Height=\"650\" Cursor=\"SizeAll\" ContextMenu=\"{x:Null}\" /></AdornerDecorator><AdornerDecorator FixedPage.Left=\"29.069221496097725\" FixedPage.Top=\"709.0362286656939\"><Image Stretch=\"Fill\" Width=\"740.1593282886076\" Height=\"379.7625112088831\" Cursor=\"SizeAll\" ContextMenu=\"{x:Null}\" /></AdornerDecorator></FixedPage>";
        private string template4Xml = "<FixedPage xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" Width=\"796.8\" Height=\"1123.2\"><AdornerDecorator FixedPage.Left=\"44.235771841887754\" FixedPage.Top=\"165.56817460821026\"><Image Stretch=\"Fill\" Cursor=\"SizeAll\" ContextMenu=\"{x:Null}\" /></AdornerDecorator><AdornerDecorator FixedPage.Left=\"385.4831546221683\" FixedPage.Top=\"743.1609669437217\"><Image Stretch=\"Fill\" Cursor=\"SizeAll\" ContextMenu=\"{x:Null}\" /></AdornerDecorator><AdornerDecorator FixedPage.Left=\"25.277583909650048\" FixedPage.Top=\"528.3015037116935\"><Image Stretch=\"Fill\" Width=\"315\" Height=\"580\" Cursor=\"SizeAll\" ContextMenu=\"{x:Null}\" /></AdornerDecorator><AdornerDecorator FixedPage.Left=\"457.5242687646719\" FixedPage.Top=\"12.638791954825365\"><Image Stretch=\"Fill\" Width=\"314.2320394109979\" Height=\"579.4554240951212\" Cursor=\"SizeAll\" ContextMenu=\"{x:Null}\" /></AdornerDecorator></FixedPage>";
    }
}
