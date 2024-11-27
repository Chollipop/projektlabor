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

            FrameworkElement? parent = _selectedElement.Parent as FrameworkElement;
            FrameworkElement element = _selectedElement;

            if (parent == null) return;
            if (parent is AdornerDecorator)
            {
                element = parent;
                parent = (FrameworkElement)parent.Parent;
            }

            if (parent is FixedPage parentPage)
            {
                parentPage.Children.Remove(element);
                parentPage.Children.Insert(0, element);
            }
        }
    }
}
