using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ProjektLavor
{
    public class ResizeAdorner : Adorner
    {
        VisualCollection AdornerVisuals;
        Thumb thumbTopLeft;
        Thumb thumbTopRight;
        Thumb thumbBottomLeft;
        Thumb thumbBottomRight;

        public ResizeAdorner(UIElement adornedElement) : base(adornedElement)
        {
            AdornerVisuals = new VisualCollection(this);
            thumbTopLeft = new Thumb { Background = Brushes.Coral, Width = 30, Height = 30 };
            thumbTopRight = new Thumb { Background = Brushes.Coral, Width = 30, Height = 30 };
            thumbBottomLeft = new Thumb { Background = Brushes.Coral, Width = 30, Height = 30 };
            thumbBottomRight = new Thumb { Background = Brushes.Coral, Width = 30, Height = 30 };

            thumbTopLeft.Cursor = Cursors.SizeNWSE;
            thumbTopRight.Cursor = Cursors.SizeNESW;
            thumbBottomLeft.Cursor = Cursors.SizeNESW;
            thumbBottomRight.Cursor = Cursors.SizeNWSE;
            AdornerVisuals.Add(thumbTopLeft);
            AdornerVisuals.Add(thumbTopRight);
            AdornerVisuals.Add(thumbBottomLeft);
            AdornerVisuals.Add(thumbBottomRight);

            thumbTopLeft.DragDelta += ThumbTopLeft_DragDelta;
            thumbTopRight.DragDelta += ThumbTopRight_DragDelta;
            thumbBottomLeft.DragDelta += ThumbBottomLeft_DragDelta;
            thumbBottomRight.DragDelta += ThumbBottomRight_DragDelta;
        }



        private void ThumbTopLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)AdornedElement;
            if (element == null) return;

            double newWidth = element.Width;
            double newHeight = element.Height;
            if (double.IsNaN(newWidth) || newWidth <= 0) newWidth = element.ActualWidth;
            if (double.IsNaN(newHeight) || newHeight <= 0) newHeight = element.ActualHeight;

            newWidth    -= e.HorizontalChange;
            newHeight   -= e.VerticalChange;
            double top = FixedPage.GetTop(element) + e.VerticalChange;
            double left = FixedPage.GetLeft(element) + e.HorizontalChange;

            if (newWidth < 10) newWidth = 10;
            if (newHeight < 10) newHeight = 10;

            element.Width = newWidth;
            element.Height = newHeight;
            element.Measure(((FrameworkElement)element.Parent).DesiredSize);
            FixedPage.SetLeft(element, left);
            FixedPage.SetTop(element, top);
            element.UpdateLayout();
        }
        private void ThumbTopRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)AdornedElement;
            if (element == null) return;

            double newWidth = element.Width;
            double newHeight = element.Height;
            if (double.IsNaN(newWidth) || newWidth <= 0) newWidth = element.ActualWidth;
            if (double.IsNaN(newHeight) || newHeight <= 0) newHeight = element.ActualHeight;

            newWidth    += e.HorizontalChange;
            newHeight   -= e.VerticalChange;
            double top = FixedPage.GetTop(element) + e.VerticalChange;
            double left = FixedPage.GetLeft(element);

            if (newWidth < 10) newWidth = 10;
            if (newHeight < 10) newHeight = 10;

            element.Width = newWidth;
            element.Height = newHeight;
            element.Measure(((FrameworkElement)element.Parent).DesiredSize);
            FixedPage.SetLeft(element, left);
            FixedPage.SetTop(element, top);
            element.UpdateLayout();
        }
        private void ThumbBottomLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)AdornedElement;
            if (element == null) return;

            double newWidth = element.Width;
            double newHeight = element.Height;
            if (double.IsNaN(newWidth) || newWidth <= 0) newWidth = element.ActualWidth;
            if (double.IsNaN(newHeight) || newHeight <= 0) newHeight = element.ActualHeight;

            newWidth    -= e.HorizontalChange;
            newHeight   += e.VerticalChange;
            double top = FixedPage.GetTop(element);
            double left = FixedPage.GetLeft(element) + e.HorizontalChange;

            if (newWidth < 10) newWidth = 10;
            if (newHeight < 10) newHeight = 10;

            element.Width = newWidth;
            element.Height = newHeight;
            element.Measure(((FrameworkElement)element.Parent).DesiredSize);
            FixedPage.SetLeft(element, left);
            FixedPage.SetTop(element, top);
            element.UpdateLayout();
        }
        private void ThumbBottomRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)AdornedElement;
            if (element == null) return;

            double newWidth = element.Width;
            double newHeight = element.Height;
            if (double.IsNaN(newWidth) || newWidth <= 0) newWidth = element.ActualWidth;
            if (double.IsNaN(newHeight) || newHeight <= 0) newHeight = element.ActualHeight;

            newWidth    += e.HorizontalChange;
            newHeight   += e.VerticalChange;
            double top = FixedPage.GetTop(element);
            double left = FixedPage.GetLeft(element);
            
            if (newWidth < 10) newWidth = 10;
            if (newHeight < 10) newHeight = 10;

            element.Width = newWidth;
            element.Height = newHeight;
            element.Measure(((FrameworkElement)element.Parent).DesiredSize);
            FixedPage.SetLeft(element, left);
            FixedPage.SetTop(element, top);
            element.UpdateLayout();
        }

        protected override int VisualChildrenCount => AdornerVisuals.Count;
        protected override Visual GetVisualChild(int index)
        {
            return AdornerVisuals[index];
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            thumbTopLeft.Arrange(new Rect(-5, -5, 10, 10));
            thumbTopRight.Arrange(new Rect(AdornedElement.RenderSize.Width - 5, -5, 10, 10));
            thumbBottomLeft.Arrange(new Rect(-5, AdornedElement.RenderSize.Height - 5, 10, 10));
            thumbBottomRight.Arrange(new Rect(AdornedElement.RenderSize.Width - 5, AdornedElement.RenderSize.Height - 5, 10, 10));

            return finalSize;
        }
        protected override Size MeasureOverride(Size constraint)
        {
            foreach (Visual child in AdornerVisuals)
            {
                if (child is UIElement element)
                {
                    element.Measure(constraint);
                }
            }
            return base.MeasureOverride(constraint);
        }

    }
}
