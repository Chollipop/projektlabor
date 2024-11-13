using ProjektLavor.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace ProjektLavor.Commands
{
    public class NewProjectCommand : CommandBase
    {
        private ProjectStore _projectStore;

        public NewProjectCommand(ProjectStore projectStore)
        {
            _projectStore = projectStore;
        }

        public override void Execute(object? parameter)
        {
            _projectStore.NewProject();
            _projectStore.CurrentProjectFilePath = null;
            _projectStore.ClearUndoRedoStacks();
        }
    }
}
