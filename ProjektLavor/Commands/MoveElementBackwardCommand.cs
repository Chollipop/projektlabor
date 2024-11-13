using ProjektLavor.Stores;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ProjektLavor.Commands
{
    public class MoveElementBackwardCommand : CommandBase
    {
        private ProjectStore _projectStore;
        private FrameworkElement _selectedElement;

        public MoveElementBackwardCommand(FrameworkElement selectedElement, ProjectStore projectStore)
        {
            _selectedElement = selectedElement;
            _projectStore = projectStore;
        }

        public override void Execute(object? parameter)
        {
            if (_selectedElement == null) return;

            _projectStore.SaveState();

            // Get the parent container of the selected element
            var parent = _selectedElement.Parent as UIElement;

            // Ensure that the parent is a container that can hold child elements
            if (parent == null) return;

            // Get the parent container as a Panel (e.g., Canvas, Grid, etc.)
            if (parent is FixedPage parentPage)
            {
                // Remove the selected element from the parent container
                parentPage.Children.Remove(_selectedElement);

                // Reinsert the selected element at the first position (send to back)
                parentPage.Children.Insert(0, _selectedElement);
            }
        }
    }
}
