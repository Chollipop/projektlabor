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
        public ICommand ExportProjectCommand { get; }
        public ICommand PrintProjectCommand { get; }
        public ICommand DeletePageCommand { get; }
        public ICommand RotatePageCommand { get; }

        public NavigationBarViewModel(IServiceProvider _serviceProvider)
        {
            SelectedElementStore selectedElementStore = _serviceProvider.GetRequiredService<SelectedElementStore>();
            ProjectStore projectStore = _serviceProvider.GetRequiredService<ProjectStore>();
            NewProjectCommand = new NewProjectCommand(projectStore);
            OpenProjectCommand = new OpenProjectCommand(projectStore);
            SaveProjectCommand = new SaveProjectCommand(projectStore);
            SaveAsProjectCommand = new SaveAsProjectCommand(projectStore);
            CloseProjectCommand = new CloseProjectCommand(projectStore);

            NewPageCommand = new NewPageCommand(projectStore);

            UndoCommand = new UndoCommand(projectStore);
            RedoCommand = new RedoCommand(projectStore);

            ExportProjectCommand = new ExportProjectCommand(projectStore);
            PrintProjectCommand = new PrintProjectCommand(projectStore);

            ExitApplicationCommand = new ExitApplicationCommand();

            DeletePageCommand = new DeletePageCommand(selectedElementStore, projectStore);
            RotatePageCommand = new RotatePageCommand(selectedElementStore, projectStore);
        }
    }
}
