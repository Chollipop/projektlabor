using ProjektLavor.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjektLavor.Commands
{
    public class RedoCommand : CommandBase
    {
        private ProjectStore _projectStore;

        public RedoCommand(ProjectStore projectStore)
        {
            _projectStore = projectStore;
        }

        public override void Execute(object? parameter)
        {
            _projectStore.Redo();
        }
    }
}
