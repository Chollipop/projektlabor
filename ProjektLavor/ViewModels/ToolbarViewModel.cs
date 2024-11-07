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
using System.Windows.Controls;

namespace ProjektLavor.ViewModels
{
    public class ToolbarViewModel : ViewModelBase
    {
        private ProjectStore _projectStore;

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

                foreach (PageContent pageContent in _projectStore.CurrentProject.Document.Pages)
                {
                    pageContent.Child.Background = new SolidColorBrush(value.Value);
                }
                //OnPropertyChanged(nameof(SelectedCustomColor));
            }
        }

        public ToolbarViewModel(IServiceProvider serviceProvider)
        {
            ModalNavigationStore modalNavigationStore = serviceProvider.GetRequiredService<ModalNavigationStore>();
            _projectStore = serviceProvider.GetRequiredService<ProjectStore>();

            ChangeColorCommand = new ChangeColorCommand(_projectStore);

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
