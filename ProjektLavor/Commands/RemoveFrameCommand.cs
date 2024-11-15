using ProjektLavor.Stores;
using System;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ProjektLavor.Commands
{
    public class RemoveFrameCommand : CommandBase
    {
        public override void Execute(object? parameter)
        {
            if (parameter is not Tuple<FrameworkElement, SelectedElementStore, ProjectStore> tuple) return;

            var (element, selectedElementStore, projectStore) = tuple;

            if (element?.Parent == null) return;
            if (element.Parent.GetType() != typeof(FixedPage)) return;

            FixedPage fixedPage = (FixedPage)element.Parent;

            foreach (var item in AdornerLayer.GetAdornerLayer(element)?.GetAdorners(selectedElementStore.SelectedElement) ?? [])
            {
                if (item is FrameAdorner)
                {
                    projectStore.SaveState();
                    AdornerLayer.GetAdornerLayer(fixedPage)?.Remove(item);
                    selectedElementStore.SelectedElement = null;
                }
            }
        }
    }
}
