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

            foreach (var item in AdornerLayer.GetAdornerLayer(element)?.GetAdorners(element) ?? [])
            {
                if (item is FrameAdorner)
                {
                    projectStore.SaveState();
                    AdornerLayer.GetAdornerLayer(element)?.Remove(item);
                    selectedElementStore.SelectedElement = null;
                }
            }
        }
    }
}
