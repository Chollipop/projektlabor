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

            AlignItems(
                fixedPage.Children,
                fixedDocument.DocumentPaginator.PageSize.Width,
                fixedDocument.DocumentPaginator.PageSize.Height
            );

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

        public void Dispose()
        {
        }
    }
}
