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
