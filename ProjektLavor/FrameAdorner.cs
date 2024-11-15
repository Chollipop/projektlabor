using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ProjektLavor
{
    public class FrameAdorner : Adorner
    {
        public ImageSource ImageSource { get; set; }
        VisualCollection AdornerVisuals;
        Image Image;

        public FrameAdorner(UIElement adornedElement, ImageSource imageSource) : base(adornedElement)
        {
            ImageSource = imageSource;
            AdornerVisuals = new VisualCollection(this);

            Image = new Image() { Source = imageSource };
            Image.IsHitTestVisible = false;
            Image.Focusable = false;
            Image.Stretch = Stretch.Fill;
            Image.StretchDirection = StretchDirection.Both;
            AdornerVisuals.Add(Image);
        }


        protected override int VisualChildrenCount => AdornerVisuals.Count;
        protected override Visual GetVisualChild(int index)
        {
            return AdornerVisuals[index];
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double width = AdornedElement.RenderSize.Width;
            double height = AdornedElement.RenderSize.Height;

            Image.Arrange(new Rect(-(width*.13), -(height * .13), width + (width * .13 * 2), height + (height * .13 * 2)));
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