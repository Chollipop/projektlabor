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
                        new List<Image>()
                        {
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Frames/frame4.png"))}
                        }
                    )
                ));
            OpenNewTextBubbleModalCommand = new NavigateCommand(
                new ModalNavigationService<CollectionBrowserViewModel>(
                    modalNavigationStore,
                    () => _projectStore.CurrentProject == null ? null : new CollectionBrowserViewModel(
                        _projectStore,
                        new CloseModalNavigationService(modalNavigationStore),
                        new List<Image>()
                        {
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble1.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble2.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble3.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble4.png"))},
                            new Image(){Source=new BitmapImage(new Uri("Pack://application:,,,/Assets/Bubbles/bubble5.png"))}
                        }
                    )
                ));

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
        }
    }
}
