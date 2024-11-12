using ProjektLavor.Stores;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ProjektLavor.Commands
{
    public class MoveElementForwardCommand : CommandBase
    {
        private ProjectStore _projectStore;
        private FrameworkElement _selectedElement;

        public MoveElementForwardCommand(FrameworkElement selectedElement, ProjectStore projectStore)
        {
            _selectedElement = selectedElement;
            _projectStore = projectStore;
        }

        public override void Execute(object? parameter)
        {
            if (_selectedElement == null) return;

            // Get the parent container of the selected element
            var parent = _selectedElement.Parent as UIElement;

            // Ensure that the parent is a container that can hold child elements
            if (parent == null) return;

            // Get the parent container as a Panel (e.g., Canvas, Grid, etc.)
            if (parent is FixedPage parentPage)
            {
                // Remove the selected element from the parent container
                parentPage.Children.Remove(_selectedElement);

                // Reinsert the selected element at the last position (bring to front)
                parentPage.Children.Add(_selectedElement);
            }
        }
    }
}
