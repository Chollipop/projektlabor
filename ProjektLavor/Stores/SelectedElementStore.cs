using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjektLavor.Stores
{
    public class SelectedElementStore
    {
        public delegate void PreviewSelectedElementChangedEventHandler(object sender, PreviewSelectedElementChangedEventArgs e);
        public event PreviewSelectedElementChangedEventHandler PreviewSelectedElementChanged;
        public event Action SelectedElementChanged;

        private FrameworkElement _selectedElement;
        public FrameworkElement SelectedElement
        {
            get => _selectedElement;
            set
            {
                PreviewSelectedElementChanged?.Invoke(this, new PreviewSelectedElementChangedEventArgs(_selectedElement, value));
                _selectedElement = value;
                SelectedElementChanged?.Invoke();
            }
        }

        public void Select(FrameworkElement frameworkElement)
        {
            SelectedElement = frameworkElement;
        }
    }


    public class PreviewSelectedElementChangedEventArgs : EventArgs
    {
        public FrameworkElement LastValue { get; private set; }
        public FrameworkElement NewValue { get; private set; }

        public PreviewSelectedElementChangedEventArgs(FrameworkElement lastElement, FrameworkElement newValue)
        {
            LastValue = lastElement;
            NewValue = newValue;
        }
    }
}
