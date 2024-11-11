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
    public class ToggleUnderlineCommand : CommandBase
    {
        private ProjectStore _projectStore;
        private TextBlock _selectedElement;

        public ToggleUnderlineCommand(TextBlock selectedElement, ProjectStore projectStore)
        {
            _selectedElement = selectedElement;
            _projectStore = projectStore;
        }

        public override void Execute(object? parameter)
        {
            _projectStore.SaveState();

            if (_selectedElement.TextDecorations == null || !_selectedElement.TextDecorations.Contains(TextDecorations.Underline[0]))
            {
                _selectedElement.TextDecorations = new TextDecorationCollection(TextDecorations.Underline);
            }
            else
            {
                var textDecorations = new TextDecorationCollection(_selectedElement.TextDecorations);
                textDecorations.Clear();
                _selectedElement.TextDecorations = textDecorations;
            }
        }

    }
}
