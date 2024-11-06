using ProjektLavor.Stores;
using System.Windows.Input;

namespace ProjektLavor.Commands
{
    internal class NewPageCommand : CommandBase
    {
        private ProjectStore _projectStore;

        public NewPageCommand(ProjectStore projectStore)
        {
            _projectStore = projectStore;
        }

        public override void Execute(object? parameter)
        {
            if (_projectStore?.CurrentProject == null) return;

            _projectStore.NewPage();
        }
    }
}