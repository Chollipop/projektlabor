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

namespace ProjektLavor.ViewModels
{
    public class ToolbarViewModel : ViewModelBase
    {
        public ICommand ChangeColorCommand { get; set; }

        public ICommand NewImageElementCommand { get; set; }
        public ICommand OpenNewFrameModalCommand { get; set; }
        public ICommand OpenNewTextElementModalCommand { get; set; }
        public ICommand OpenNewTextBubbleModalCommand { get; set; }

        public ToolbarViewModel(IServiceProvider serviceProvider)
        {
            ModalNavigationStore modalNavigationStore = serviceProvider.GetRequiredService<ModalNavigationStore>();
            ProjectStore projectStore = serviceProvider.GetRequiredService<ProjectStore>();

            //OpenNewTextElementModalCommand = new NavigateCommand(newTextElementNavigationService);
            OpenNewTextElementModalCommand = new NavigateCommand(
                new ModalNavigationService<TextElementInputViewModel>(
                    modalNavigationStore,
                    () => projectStore.CurrentProject == null ? null : new TextElementInputViewModel(
                        projectStore,
                        new CloseModalNavigationService(modalNavigationStore)
                    )
                ));
            NewImageElementCommand = new NewImageElementCommand(projectStore);
        }
    }
}
