using ProjektLavor.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektLavor.Commands
{
    public class ApplyPropertiesCommand : CommandBase
    {
        private SelectedElementStore _selectedElementStore;

        public ApplyPropertiesCommand(SelectedElementStore selectedElementStore)
        {
            _selectedElementStore = selectedElementStore;
        }

        public override void Execute(object? parameter)
        {
            _selectedElementStore.SelectedElement = null;
        }
    }
}
