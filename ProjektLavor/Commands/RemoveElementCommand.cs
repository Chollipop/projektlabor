using ProjektLavor.Stores;
using System;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ProjektLavor.Commands
{
    public class RemoveElementCommand : CommandBase
    {
        public override void Execute(object? parameter)
        {
            if (parameter is not Tuple<FrameworkElement, SelectedElementStore, ProjectStore> tuple) return;

            var (element, selectedElementStore, projectStore) = tuple;

            if (element?.Parent == null) return;
            if (element.Parent is AdornerDecorator) element = (FrameworkElement)element.Parent;
            if (element.Parent.GetType() != typeof(FixedPage)) return;
            FixedPage parent = (FixedPage)element.Parent;

            selectedElementStore.Select(null);

            if (parent.Children.Contains(element))
            {
                projectStore.SaveState();
                parent.Children.Remove(element);
            }
        }
    }
}
