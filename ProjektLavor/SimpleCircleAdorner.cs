using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Documents;

namespace ProjektLavor
{
    public class SimpleCircleAdorner : Adorner
    {
        // Be sure to call the base class constructor.
        public SimpleCircleAdorner(UIElement adornedElement)
          : base(adornedElement)
        {
        }

        // A common way to implement an adorner's rendering behavior is to override the OnRender
        // method, which is called by the layout system as part of a rendering pass.
        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

            // Some arbitrary drawing implements.
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
            renderBrush.Opacity = 0.2;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
            double renderRadius = 5.0;

            double width = AdornedElement.RenderSize.Width;
            double height = AdornedElement.RenderSize.Height;
            Point topLeft = new Point(0, 0);
            Point topRight = new Point(width, 0);
            Point bottomLeft = new Point(0, height);
            Point bottomRight = new Point(width, height);

            // Draw a circle at each corner.
            drawingContext.DrawEllipse(renderBrush, renderPen, topLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, topRight, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, bottomLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, bottomRight, renderRadius, renderRadius);
        }
    }
}
