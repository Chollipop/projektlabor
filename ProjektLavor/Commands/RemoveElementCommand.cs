using ProjektLavor.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace ProjektLavor.Commands
{
    internal class RemoveElementCommand : CommandBase
    {
        private readonly SelectedElementStore _selectedElementStore;

        public RemoveElementCommand(SelectedElementStore selectedElementStore)
        {
            _selectedElementStore = selectedElementStore;
        }

        public override void Execute(object? parameter)
        {
            if (parameter == null || !typeof(FrameworkElement).IsAssignableFrom(parameter.GetType())) return;
            FrameworkElement element = (FrameworkElement)parameter;

            if (element.Parent == null) return;
            if (element.Parent.GetType() != typeof(FixedPage)) return;
            FixedPage parent = (FixedPage)element.Parent;

            _selectedElementStore.Select(null);
            parent.Children.Remove(element);
        }
    }
}
