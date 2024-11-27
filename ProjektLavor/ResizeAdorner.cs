using ProjektLavor.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ProjektLavor
{
    public class ResizeAdorner : Adorner
    {
        private ProjectStore _projectStore;
        private bool _isStateSaved;
        VisualCollection AdornerVisuals;

        Thumb thumbTopLeft;
        Thumb thumbTopRight;
        Thumb thumbBottomLeft;
        Thumb thumbBottomRight;
        Rectangle thumbBounds;

        public ResizeAdorner(ProjectStore projectStore, UIElement adornedElement) : base(adornedElement)
        {
            _projectStore = projectStore;
            AdornerVisuals = new VisualCollection(this);
            thumbBounds = new Rectangle() { Stroke = Brushes.Gray, StrokeThickness = 2, StrokeDashArray = { 3, 2 } };
            thumbTopLeft = new Thumb { Background = Brushes.Coral, Width = 30, Height = 30, Cursor = Cursors.SizeNWSE, Tag = "TopLeft" };
            thumbTopRight = new Thumb { Background = Brushes.Coral, Width = 30, Height = 30, Cursor = Cursors.SizeNESW, Tag = "TopRight" };
            thumbBottomLeft = new Thumb { Background = Brushes.Coral, Width = 30, Height = 30, Cursor = Cursors.SizeNESW, Tag = "BottomLeft" };
            thumbBottomRight = new Thumb { Background = Brushes.Coral, Width = 30, Height = 30, Cursor = Cursors.SizeNWSE, Tag = "BottomRight" };

            AdornerVisuals.Add(thumbBounds);
            AdornerVisuals.Add(thumbTopLeft);
            AdornerVisuals.Add(thumbTopRight);
            AdornerVisuals.Add(thumbBottomLeft);
            AdornerVisuals.Add(thumbBottomRight);

            thumbTopLeft.DragStarted += Thumb_DragStarted;
            thumbTopRight.DragStarted += Thumb_DragStarted;
            thumbBottomLeft.DragStarted += Thumb_DragStarted;
            thumbBottomRight.DragStarted += Thumb_DragStarted;

            thumbTopLeft.DragDelta += Thumb_DragDelta;
            thumbTopRight.DragDelta += Thumb_DragDelta;
            thumbBottomLeft.DragDelta += Thumb_DragDelta;
            thumbBottomRight.DragDelta += Thumb_DragDelta;

            thumbTopLeft.DragCompleted += Thumb_DragCompleted;
            thumbTopRight.DragCompleted += Thumb_DragCompleted;
            thumbBottomLeft.DragCompleted += Thumb_DragCompleted;
            thumbBottomRight.DragCompleted += Thumb_DragCompleted;
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            if (!_isStateSaved)
            {
                _projectStore.SaveState();
                _isStateSaved = true;
            }
        }
        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            _isStateSaved = false;
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)AdornedElement;
            if (element == null) return;
            FrameworkElement elementToPosition = element;
            if (element is Image) elementToPosition = (FrameworkElement)element.Parent;

            if (double.IsNaN(element.Width) || element.Width <= 0) element.Width = element.ActualWidth;
            if (double.IsNaN(element.Height) || element.Height <= 0) element.Height = element.ActualHeight;

            TransformGroup? transformGroup = element.RenderTransform as TransformGroup;
            ScaleTransform? scaleTransform = null;
            RotateTransform? rotateTransform = null;
            foreach (var transform in transformGroup?.Children ?? [])
            {
                if (transform is ScaleTransform st)
                    scaleTransform = st;
                else if (transform is RotateTransform rt)
                    rotateTransform = rt;
            }

            if (rotateTransform == null) rotateTransform = new RotateTransform();
            Matrix rotateMatrix = rotateTransform.Value;
            Matrix inverseMatrix = rotateMatrix;
            inverseMatrix.Invert();
            Point delta = new Point(e.HorizontalChange, e.VerticalChange);
            delta = inverseMatrix.Transform(delta);

            double newHorizontalChange = delta.X;
            double newVerticalChange = delta.Y;

            double newWidth = element.Width;
            double newHeight = element.Height;
            double left = FixedPage.GetLeft(elementToPosition);
            double top = FixedPage.GetTop(elementToPosition);

            Thumb? thumb = sender as Thumb;
            string? thumbTag = thumb?.Tag as string;
            switch (thumbTag)
            {
                case "TopLeft":
                    newWidth -= newHorizontalChange;
                    newHeight -= newVerticalChange;
                    left += newHorizontalChange;
                    top += newVerticalChange;
                    break;
                case "TopRight":
                    newWidth += newHorizontalChange;
                    newHeight -= newVerticalChange;
                    top += newVerticalChange;
                    break;
                case "BottomLeft":
                    newWidth -= newHorizontalChange;
                    newHeight += newVerticalChange;
                    left += newHorizontalChange;
                    break;
                case "BottomRight":
                    newWidth += newHorizontalChange;
                    newHeight += newVerticalChange;
                    break;
            }

            if (newWidth < 10) newWidth = 10;
            if (newHeight < 10) newHeight = 10;
            if (scaleTransform != null)
            {
                newWidth /= Math.Abs(scaleTransform.ScaleX);
                newHeight /= Math.Abs(scaleTransform.ScaleY);
            }

            if (scaleTransform != null)
            {
                if (scaleTransform.ScaleX < 0)
                {
                    switch (thumbTag)
                    {
                        case "TopLeft":
                            left -= 2 * newHorizontalChange;
                            break;
                        case "BottomLeft":
                            left -= 2 * newHorizontalChange;
                            break;
                    }
                }
                if (scaleTransform.ScaleY < 0)
                {
                    switch (thumbTag)
                    {
                        case "TopLeft":
                            top -= 2 * newVerticalChange;
                            break;
                        case "TopRight":
                            top -= 2 * newVerticalChange;
                            break;
                    }
                }
            }

            element.Width = newWidth;
            element.Height = newHeight;
            FixedPage.SetLeft(elementToPosition, left);
            FixedPage.SetTop(elementToPosition, top);
            elementToPosition.UpdateLayout();
            element.UpdateLayout();
        }

        protected override int VisualChildrenCount => AdornerVisuals.Count;
        protected override Visual GetVisualChild(int index)
        {
            return AdornerVisuals[index];
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            thumbBounds.Arrange(new Rect(-2.5, -2.5, AdornedElement.RenderSize.Width + 5, AdornedElement.RenderSize.Height + 5));
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
