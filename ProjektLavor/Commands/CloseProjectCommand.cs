using ProjektLavor.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektLavor.Commands
{
    public class CloseProjectCommand : CommandBase
    {
        private ProjectStore _projectStore;

        public CloseProjectCommand(ProjectStore projectStore)
        {
            _projectStore = projectStore;
        }

        public override void Execute(object? parameter)
        {
            _projectStore.ClearAdorners();
            _projectStore.CloseProject();
            _projectStore.CurrentProjectFilePath = null;
            _projectStore.ClearUndoRedoStacks();
        }
    }
}
