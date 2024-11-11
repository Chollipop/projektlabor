using ProjektLavor.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ProjektLavor.Commands
{
    public class ToggleBoldCommand : CommandBase
    {
        private ProjectStore _projectStore;
        private TextBlock _selectedElement;

        public ToggleBoldCommand(TextBlock selectedElement, ProjectStore projectStore)
        {
            _selectedElement = selectedElement;
            _projectStore = projectStore;
        }

        public override void Execute(object? parameter)
        {
            _projectStore.SaveState();

            _selectedElement.FontWeight = _selectedElement.FontWeight == FontWeights.Bold ? FontWeights.Normal : FontWeights.Bold;
        }
    }
}
