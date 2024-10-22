using ProjektLavor.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ProjektLavor.Models
{
    public class Project : IDisposable
    {
        private SelectedElementStore _selectedElementStore;

        public FixedDocument Document { get; set; }

        private bool IsDragging = false;
        private Adorner DraggedElementResizeAdorner;
        private Point PointerOffsetInElement;

        public Project(SelectedElementStore selectedElementStore)
        {
            _selectedElementStore = selectedElementStore;
            _selectedElementStore.PreviewSelectedElementChanged += _selectedElementStore_PreviewSelectedElementChanged;
            
            Document = GetTestDocument();
        }

        private void _selectedElementStore_PreviewSelectedElementChanged(object sender, PreviewSelectedElementChangedEventArgs e)
        {
            if (e.LastValue == null) return;

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(e.LastValue);
            adornerLayer.Remove(DraggedElementResizeAdorner);
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
        private FixedDocument CreateEmptyProject()
        {
            FixedDocument fixedDocument = new FixedDocument();
            fixedDocument.DocumentPaginator.PageSize = new Size(796.8, 1123.2);

            PageContent pageContent = new PageContent();
            FixedPage fixedPage = new FixedPage();

            pageContent.Child = fixedPage;
            fixedDocument.Pages.Add(pageContent);

            fixedPage.MouseDown += FixedPage_MouseDown;
            fixedPage.MouseUp += FixedPage_MouseUp;
            fixedPage.MouseMove += FixedPage_MouseMove;

            return fixedDocument;
        }
        //private void AlignItems(UIElementCollection elements, double width, double height)
        //{
        //    int marginX = 0, marginY = 0;
        //    double y = 0;
        //    Size availableSpace = new Size(width - 2 * marginX, height - 2 * marginY);
        //    foreach (FrameworkElement element in elements)
        //    {
        //        //element.Width = availableSpace.Width;
        //        element.Measure(availableSpace);
        //        FixedPage.SetTop(element, y);
        //        y += element.DesiredSize.Height;
        //    }
        //}
        public void AddNewTextField(string text)
        {
            if (Document == null || Document.Pages.Count <= 0) return;

            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            Document.Pages.Last().Child.Children.Add(textBlock);
        }
        public void AddNewImageField(string path)
        {
            if (Document == null || Document.Pages.Count <= 0) return;

            Image image = new Image();
            image.Source = new BitmapImage(new Uri(path));
            Document.Pages.Last().Child.Children.Add(image);
        }

        private void FixedPage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (e.OriginalSource.GetType() == typeof(FixedPage))
            {
                _selectedElementStore.Select(null);
                return;
            }

            FixedPage fixedPage = (FixedPage)sender;
            FrameworkElement element = (FrameworkElement)e.OriginalSource;
            PointerOffsetInElement = e.GetPosition(element);
            IsDragging = true;
            _selectedElementStore.Select(element);

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(_selectedElementStore.SelectedElement);
            DraggedElementResizeAdorner = new SimpleCircleAdorner(_selectedElementStore.SelectedElement);
            DraggedElementResizeAdorner.Cursor = Cursors.SizeAll;
            adornerLayer.Add(DraggedElementResizeAdorner);
        }

        private void FixedPage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            IsDragging = false;
            if(_selectedElementStore.SelectedElement == null) return;

            //AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(_selectedElementStore.SelectedElement);
            //adornerLayer.Remove(DraggedElementResizeAdorner);

            //_selectedElementStore.SelectedElement = null;
        }
        private void FixedPage_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            if (!IsDragging || _selectedElementStore.SelectedElement == null) return;
            
            FixedPage fixedPage = (FixedPage)sender;
            Point cPos = e.GetPosition(fixedPage);

            _selectedElementStore.SelectedElement.Margin = new Thickness(cPos.X - PointerOffsetInElement.X, cPos.Y - PointerOffsetInElement.Y, 0, 0);
        }

        public void Dispose()
        {
            _selectedElementStore.PreviewSelectedElementChanged -= _selectedElementStore_PreviewSelectedElementChanged;

            foreach (PageContent pageContent in Document.Pages)
            {
                pageContent.Child.MouseDown -= FixedPage_MouseDown;
                pageContent.Child.MouseUp -= FixedPage_MouseUp;
                pageContent.Child.MouseMove -= FixedPage_MouseMove;
            }
        }
    }
}
