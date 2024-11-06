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

        public Project(SelectedElementStore selectedElementStore)
        {
            _selectedElementStore = selectedElementStore;

            CreateEmptyDocument();
        }

        private void CreateEmptyDocument()
        {
            Document = new FixedDocument();
            AddBlankPage();
        }
        public void AddTestElements()
        {
            AddNewTextField("Hello World!");
            AddNewImageField("Pack://application:,,,/Assets/coconut.jpg");
        }

        public PageContent AddBlankPage()
        {
            PageContent pageContent = new PageContent();
            FixedPage fixedPage = new FixedPage();

            fixedPage.Width = ISOA4.Width;
            fixedPage.Height = ISOA4.Height;
            pageContent.Child = fixedPage;
            Document.Pages.Add(pageContent);

            return pageContent;
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
            textBlock.TextWrapping = TextWrapping.WrapWithOverflow;
            textBlock.ClipToBounds = true;
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

        public void Dispose()
        {
            _selectedElementStore.Select(null);
        }
    }
}
