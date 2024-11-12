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

            // Get the parent container of the selected element
            var parent = _selectedElement.Parent as UIElement;

            // Ensure that the parent is a container that can hold child elements
            if (parent == null) return;

            // Get the parent container as a Panel (e.g., Canvas, Grid, etc.)
            if (parent is FixedPage parentPage)
            {
                // Get the index of the selected element
                int currentIndex = parentPage.Children.IndexOf(_selectedElement);

                // Check if the element is not already at the back (i.e., if it's not the first element)
                if (currentIndex > 0)
                {
                    // Remove the selected element from its current position
                    parentPage.Children.Remove(_selectedElement);

                    // Insert the selected element just before the previous element (i.e., move it one step back)
                    parentPage.Children.Insert(currentIndex - 1, _selectedElement);
                }
            }
        }
    }
}
