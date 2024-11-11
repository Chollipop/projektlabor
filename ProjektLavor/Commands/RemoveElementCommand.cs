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
    public class RemoveElementCommand : CommandBase
    {
        private readonly SelectedElementStore _selectedElementStore;
        private FrameworkElement _element;

        public RemoveElementCommand(FrameworkElement element, SelectedElementStore selectedElementStore)
        {
            _element = element;
            _selectedElementStore = selectedElementStore;
        }

        public override void Execute(object? parameter)
        {
            if (_element?.Parent == null) return;
            if (_element.Parent.GetType() != typeof(FixedPage)) return;
            FixedPage parent = (FixedPage)_element.Parent;

            _selectedElementStore.Select(null);
            parent.Children.Remove(_element);
        }
    }
}
