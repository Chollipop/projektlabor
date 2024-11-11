using ProjektLavor.ViewModels;
using ProjektLavor.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Extensions.DependencyInjection;
using ProjektLavor.Stores;

namespace ProjektLavor.ViewModels
{
    public class NavigationBarViewModel : ViewModelBase
    {
        public ICommand NewProjectCommand { get; }
        public ICommand OpenProjectCommand { get; }
        public ICommand SaveProjectCommand { get; }
        public ICommand SaveAsProjectCommand { get; }
        public ICommand CloseProjectCommand { get; }

        public ICommand NewPageCommand { get; }

        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }

        public ICommand ExitApplicationCommand { get; }

        public NavigationBarViewModel(IServiceProvider _serviceProvider)
        {
            ProjectStore projectStore = _serviceProvider.GetRequiredService<ProjectStore>();
            NewProjectCommand = new NewProjectCommand(projectStore);
            //OpenProjectCommand = new OpenProjectCommand();
            //SaveProjectCommand = new SaveProjectCommand();
            //SaveAsProjectCommand = new SaveAsProjectCommand();
            CloseProjectCommand = new CloseProjectCommand(projectStore);

            NewPageCommand = new NewPageCommand(projectStore);

            UndoCommand = new UndoCommand(projectStore);
            RedoCommand = new RedoCommand(projectStore);

            ExitApplicationCommand = new ExitApplicationCommand();
        }

    }
}
