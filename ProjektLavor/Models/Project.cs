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
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProjektLavor.Models
{
    public class Project : IDisposable
    {
        private readonly Size ISOA4 = new Size(796.8, 1123.2);
        private SelectedElementStore _selectedElementStore;

        public FixedDocument Document { get; set; }

        private bool IsDragging = false;
        private Adorner DraggedElementResizeAdorner;
        private Point PointerOffsetInElement;

        public Project(SelectedElementStore selectedElementStore)
        {
            _selectedElementStore = selectedElementStore;
            _selectedElementStore.PreviewSelectedElementChanged += _selectedElementStore_PreviewSelectedElementChanged;

            Document = CreateEmptyDocument();
#if DEBUG
            Document = GetTestDocument();
#endif
        }

        private FixedDocument CreateEmptyDocument()
        {
            FixedDocument fixedDocument = new FixedDocument();
            fixedDocument.DocumentPaginator.PageSize = ISOA4;

            PageContent pageContent = new PageContent();
            FixedPage fixedPage = new FixedPage();

            pageContent.Child = fixedPage;
            fixedDocument.Pages.Add(pageContent);

            fixedPage.MouseDown += FixedPage_MouseDown;
            fixedPage.MouseUp += FixedPage_MouseUp;
            fixedPage.MouseMove += FixedPage_MouseMove;

            return fixedDocument;
        }
        private FixedDocument GetTestDocument()
        {
            FixedDocument fixedDocument = new FixedDocument();
            fixedDocument.DocumentPaginator.PageSize = ISOA4;

            PageContent pageContent = new PageContent();
            FixedPage fixedPage = new FixedPage();

            TextBlock textBlock = GetTextField("Hello World!");
            Image image = GetImageField("Pack://application:,,,/Assets/coconut.jpg");
            fixedPage.Children.Add(textBlock);
            fixedPage.Children.Add(image);

            pageContent.Child = fixedPage;
            fixedDocument.Pages.Add(pageContent);

            fixedPage.MouseDown += FixedPage_MouseDown;
            fixedPage.MouseUp += FixedPage_MouseUp;
            fixedPage.MouseMove += FixedPage_MouseMove;

            return fixedDocument;
        }

        public void AddNewTextField(string text)
        {
            if (Document == null || Document.Pages.Count <= 0) return;

            Document.Pages.Last().Child.Children.Add(GetTextField(text));
        }
        public void AddNewImageField(string path)
        {
            if (Document == null || Document.Pages.Count <= 0) return;

            Document.Pages.Last().Child.Children.Add(GetImageField(path));
        }
        private TextBlock GetTextField(string text)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.Cursor = Cursors.SizeAll;

            return textBlock;
        }
        private Image GetImageField(string path)
        {
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(path));
            image.Stretch = Stretch.Fill;
            image.Cursor = Cursors.SizeAll;

            return image;
        }

        private void _selectedElementStore_PreviewSelectedElementChanged(object sender, PreviewSelectedElementChangedEventArgs e)
        {
            if (e.LastValue == null) return;

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(e.LastValue);
            foreach (Adorner adorner in adornerLayer.GetAdorners(e.LastValue))
            {
                adornerLayer.Remove(adorner);
            }
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

            AdornerLayer.GetAdornerLayer(element).Add(new ResizeAdorner(element));
        }
        private void FixedPage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            IsDragging = false;
        }
        private void FixedPage_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            if (!IsDragging || _selectedElementStore.SelectedElement == null) return;
            
            FixedPage fixedPage = (FixedPage)sender;
            Point cPos = e.GetPosition(fixedPage);

            double x = cPos.X - PointerOffsetInElement.X;
            double y = cPos.Y - PointerOffsetInElement.Y;

            FixedPage.SetLeft(_selectedElementStore.SelectedElement, x);
            FixedPage.SetTop(_selectedElementStore.SelectedElement, y);
        }


        public void Dispose()
        {
            _selectedElementStore.PreviewSelectedElementChanged -= _selectedElementStore_PreviewSelectedElementChanged;
            _selectedElementStore.Select(null);

            foreach (PageContent pageContent in Document.Pages)
            {
                pageContent.Child.MouseDown -= FixedPage_MouseDown;
                pageContent.Child.MouseUp -= FixedPage_MouseUp;
                pageContent.Child.MouseMove -= FixedPage_MouseMove;
            }
        }
    }
}
