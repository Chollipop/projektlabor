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
        public ICommand OpenNewTextElementModalCommand { get; set; }
        public ICommand NewImageElementCommand { get; set; }

        public ToolbarViewModel(IServiceProvider serviceProvider)
        {
            ModalNavigationStore modalNavigationStore = serviceProvider.GetService<ModalNavigationStore>();
            ProjectStore projectStore = serviceProvider.GetRequiredService<ProjectStore>();

            //OpenNewTextElementModalCommand = new NavigateCommand(newTextElementNavigationService);
            OpenNewTextElementModalCommand = new NavigateCommand(
                new ModalNavigationService<NewTextElementViewModel>(
                    modalNavigationStore,
                    () => new NewTextElementViewModel(
                        projectStore,
                        new CloseModalNavigationService(modalNavigationStore)
                    )
                ));
            NewImageElementCommand = new NewImageElementCommand(projectStore);
        }
    }
}
