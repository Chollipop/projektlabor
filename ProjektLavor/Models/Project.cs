using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProjektLavor.Models
{
    public class Project : IDisposable
    {
        public FixedDocument Document { get; set; }

        private bool IsDragging = false;
        private FrameworkElement DraggedElement;
        private Point PointerOffsetInElement;

        public Project()
        {
            Document = GetTestDocument();
        }

        private FixedDocument GetTestDocument()
        {
            FixedDocument fixedDocument = new FixedDocument();
            fixedDocument.DocumentPaginator.PageSize = new Size(796.8, 1123.2);

            PageContent pageContent = new PageContent();
            FixedPage fixedPage = new FixedPage();
            TextBlock textBlock = new TextBlock();
            Image image = new Image();

            textBlock.Text = "Hello World!";
            image.Source = new BitmapImage(new Uri("Pack://application:,,,/Assets/coconut.jpg"));
            fixedPage.Children.Add(textBlock);
            fixedPage.Children.Add(image);

            pageContent.Child = fixedPage;
            fixedDocument.Pages.Add(pageContent);

            fixedPage.MouseDown += FixedPage_MouseDown;
            fixedPage.MouseUp += FixedPage_MouseUp;
            fixedPage.MouseMove += FixedPage_MouseMove;

            //AlignItems(
            //    fixedPage.Children,
            //    fixedDocument.DocumentPaginator.PageSize.Width,
            //    fixedDocument.DocumentPaginator.PageSize.Height
            //);

            return fixedDocument;
        }
        private void AlignItems(UIElementCollection elements, double width, double height)
        {
            int marginX = 0, marginY = 0;
            double y = 0;
            Size availableSpace = new Size(width - 2 * marginX, height - 2 * marginY);
            foreach (FrameworkElement element in elements)
            {
                //element.Width = availableSpace.Width;
                element.Measure(availableSpace);
                FixedPage.SetTop(element, y);
                y += element.DesiredSize.Height;
            }
        }

        private void FixedPage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (e.OriginalSource.GetType() == typeof(FixedPage)) return;

            FixedPage fixedPage = (FixedPage)sender;
            FrameworkElement element = (FrameworkElement)e.OriginalSource;
            PointerOffsetInElement = e.GetPosition(element);
            IsDragging = true;
            DraggedElement = element;
        }
        private void FixedPage_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IsDragging = false;
            DraggedElement = null;
        }
        private void FixedPage_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            e.Handled = true;
            if (!IsDragging || DraggedElement == null) return;
            
            FixedPage fixedPage = (FixedPage)sender;
            Point cPos = e.GetPosition(fixedPage);

            DraggedElement.Margin = new Thickness(cPos.X - PointerOffsetInElement.X, cPos.Y - PointerOffsetInElement.Y, 0, 0);
        }

        public void Dispose()
        {
            foreach (PageContent pageContent in Document.Pages)
            {
                pageContent.Child.MouseDown -= FixedPage_MouseDown;
                pageContent.Child.MouseUp -= FixedPage_MouseUp;
                pageContent.Child.MouseMove -= FixedPage_MouseMove;
            }
        }
    }
}
