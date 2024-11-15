using ProjektLavor.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjektLavor.Commands
{
    public class ScrollChangedCommand : CommandBase
    {
        private ProjectStore _projectStore;

        public ScrollChangedCommand(ProjectStore projectStore)
        {
            _projectStore = projectStore;
        }

        public override void Execute(object? parameter)
        {
            _projectStore.AddAdorners();
        }
    }
}
