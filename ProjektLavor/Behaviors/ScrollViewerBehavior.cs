using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Controls;

namespace ProjektLavor.Behaviors
{
    public static class ScrollViewerBehavior
    {
        public static readonly DependencyProperty ScrollChangedCommandProperty =
            DependencyProperty.RegisterAttached(
                "ScrollChangedCommand",
                typeof(ICommand),
                typeof(ScrollViewerBehavior),
                new PropertyMetadata(null, OnScrollChangedCommandChanged));

        public static ICommand GetScrollChangedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ScrollChangedCommandProperty);
        }

        public static void SetScrollChangedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ScrollChangedCommandProperty, value);
        }

        private static void OnScrollChangedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DocumentViewer viewer)
            {
                viewer.Loaded += Viewer_Loaded;
            }
        }

        private static void Viewer_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is DocumentViewer viewer)
            {
                var scrollViewer = FindScrollViewer(viewer);

                if (scrollViewer != null)
                {
                    scrollViewer.ScrollChanged += (s, e) => OnScrollViewerScrollChanged(viewer, e);
                }
            }
        }

        private static void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var command = GetScrollChangedCommand(sender as DocumentViewer);

            if (command != null && command.CanExecute(e))
            {
                command.Execute(e);
            }
        }

        private static ScrollViewer FindScrollViewer(DependencyObject d)
        {
            if (d is ScrollViewer)
            {
                return (ScrollViewer)d;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                var child = VisualTreeHelper.GetChild(d, i);
                var result = FindScrollViewer(child);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
