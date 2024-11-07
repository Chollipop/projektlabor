using ProjektLavor.Stores;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ProjektLavor.Commands
{
    public class ChangeColorCommand : CommandBase
    {
        private ProjectStore _projectStore;
        private SelectedElementStore _selectedElementStore;

        public ChangeColorCommand(ProjectStore projectStore, SelectedElementStore selectedElementStore)
        {
            _projectStore = projectStore;
            _selectedElementStore = selectedElementStore;
        }

        public override void Execute(object? parameter)
        {
            if (parameter == null || parameter.GetType() != typeof(Button)) return;

            if (_projectStore?.CurrentProject?.Document == null) return;

            var activePage = _projectStore.CurrentProject.ActivePage;
            var selectedElement = _selectedElementStore.SelectedElement;

            if (selectedElement != null && selectedElement.GetType() == typeof(TextBlock))
            {
                ((TextBlock)selectedElement).Foreground = ((Button)parameter).Background;
            }
            else if (activePage != null)
            {
                activePage.Background = ((Button)parameter).Background;
            }
        }
    }
}
