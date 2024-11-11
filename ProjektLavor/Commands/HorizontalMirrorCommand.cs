using ProjektLavor.Stores;
using System;
using System.Windows;
using System.Windows.Media;

namespace ProjektLavor.Commands
{
    public class HorizontalMirrorCommand : CommandBase
    {
        private ProjectStore _projectStore;
        private FrameworkElement _selectedElement;

        public HorizontalMirrorCommand(FrameworkElement selectedElement, ProjectStore projectStore)
        {
            _selectedElement = selectedElement;
            _projectStore = projectStore;
        }

        public override void Execute(object? parameter)
        {
            _projectStore.SaveState();

            if (_selectedElement != null)
            {
                var transformGroup = _selectedElement.RenderTransform as TransformGroup;
                if (transformGroup == null)
                {
                    transformGroup = new TransformGroup();
                    _selectedElement.RenderTransform = transformGroup;
                }

                var scaleTransform = transformGroup.Children.OfType<ScaleTransform>().FirstOrDefault();
                if (scaleTransform == null)
                {
                    scaleTransform = new ScaleTransform();
                    transformGroup.Children.Add(scaleTransform);
                }

                scaleTransform.CenterX = _selectedElement.ActualWidth / 2;
                scaleTransform.CenterY = _selectedElement.ActualHeight / 2;
                scaleTransform.ScaleY *= -1;
            }
        }
    }
}
