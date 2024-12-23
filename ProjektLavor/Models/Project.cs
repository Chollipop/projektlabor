﻿using ProjektLavor.Stores;
using ProjektLavor.Commands;
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
using Xceed.Wpf.AvalonDock.Controls;

namespace ProjektLavor.Models
{
    public class Project : IDisposable
    {
        private readonly Size ISOA4 = new Size(796.8, 1123.2);
        private SelectedElementStore _selectedElementStore;
        private ProjectStore _projectStore;

        public FixedDocument Document { get; set; }
        public FixedPage ActivePage { get; private set; }


        public Project(SelectedElementStore selectedElementStore, ProjectStore projectStore, bool startWithRotatedPage = false)
        {
            _selectedElementStore = selectedElementStore;
            _projectStore = projectStore;

            CreateEmptyDocument(startWithRotatedPage);
        }

        private void CreateEmptyDocument(bool startWithRotatedPage = false)
        {
            Document = new FixedDocument();
            Document.Cursor = Cursors.Arrow;
            AddBlankPage(null, startWithRotatedPage);
        }
        public void AddTestElements()
        {
            AddNewTextField("Hello World!");
            AddNewImageField("Pack://application:,,,/Assets/coconut.jpg");
        }

        public PageContent AddBlankPage(PageContent newPage = null, bool rotate = false)
        {
            if (newPage != null)
            {
                Document.Pages.Add(newPage);
                return newPage;
            }
            PageContent pageContent = new PageContent();
            FixedPage fixedPage = new FixedPage();

            if (rotate)
            {
                fixedPage.Width = ISOA4.Height;
                fixedPage.Height = ISOA4.Width;
            }
            else
            {
                fixedPage.Width = ISOA4.Width;
                fixedPage.Height = ISOA4.Height;
            }

            fixedPage.Tag = Guid.NewGuid().ToString();

            pageContent.Child = fixedPage;

            Document.Pages.Add(pageContent);

            return pageContent;
        }

        public void AddNewTextField(string text)
        {
            if (Document == null || Document.Pages.Count <= 0) return;
            if (ActivePage == null)
            {
                Document.Pages.Last().Child.Children.Add(GetTextField(text));
            }
            else
            {
                ActivePage.Children.Add(GetTextField(text));
            }
        }
        public void AddNewImageField(string path)
        {
            AddNewImageField(new BitmapImage(new Uri(path)));
        }
        public void AddNewImageField(ImageSource source, bool IgnoreWizard = false)
        {
            if (Document == null || Document.Pages.Count <= 0) return;

            _projectStore.SaveState();

            FixedPage page = Document.Pages.Last().Child;
            if (ActivePage != null) page = ActivePage;

            AdornerDecorator adornerDecorator = new AdornerDecorator();
            Image image = GetImageField(source, IgnoreWizard);
            adornerDecorator.Child = image;
            
            image.Tag = Guid.NewGuid().ToString();
            if (IgnoreWizard) image.Tag = "ignore_wizard";
            
            var newImageField = page.Children.Add(adornerDecorator);
        }

        private TextBlock GetTextField(string text)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.TextWrapping = TextWrapping.WrapWithOverflow;
            textBlock.ClipToBounds = true;
            textBlock.Cursor = Cursors.SizeAll;

            textBlock.ContextMenu = new ContextMenu();

            MenuItem removeElementMenuItem = new MenuItem();
            removeElementMenuItem.Header = "Törlés";
            removeElementMenuItem.Command = new RemoveElementCommand();
            removeElementMenuItem.CommandParameter = Tuple.Create((FrameworkElement)textBlock, _selectedElementStore, _projectStore);
            textBlock.ContextMenu.Items.Add(removeElementMenuItem);

            return textBlock;
        }

        private Image GetImageField(ImageSource source, bool IgnoreWizard = false)
        {
            Image image = new Image();
            image.Source = source;
            image.Stretch = Stretch.Fill;
            image.Cursor = Cursors.SizeAll;

            image.ContextMenu = new ContextMenu();

            MenuItem changeImageMenuItem = new MenuItem();
            changeImageMenuItem.Header = "Kép módosítása";
            changeImageMenuItem.Command = new ChangeImageSourceCommand();
            changeImageMenuItem.CommandParameter = Tuple.Create(image, _projectStore);
            
            MenuItem removeElementMenuItem = new MenuItem();
            removeElementMenuItem.Header = "Törlés";
            removeElementMenuItem.Command = new RemoveElementCommand();
            removeElementMenuItem.CommandParameter = Tuple.Create((FrameworkElement)image, _selectedElementStore, _projectStore);
            
            MenuItem removeFrameMenuItem = new MenuItem();
            removeFrameMenuItem.Header = "Keret törlése";
            removeFrameMenuItem.Command = new RemoveFrameCommand();
            removeFrameMenuItem.CommandParameter = Tuple.Create((FrameworkElement)image, _selectedElementStore, _projectStore);

            image.ContextMenu.Items.Add(removeElementMenuItem);
            if (!IgnoreWizard)
            {
                image.ContextMenu.Items.Add(changeImageMenuItem);
                image.ContextMenu.Items.Add(removeFrameMenuItem);
            }

            return image;
        }

        public void SetActivePage(FixedPage page)
        {
            ActivePage = page;
            OnActivePageChanged();
        }

        public event Action ActivePageChanged;

        private void OnActivePageChanged()
        {
            ActivePageChanged?.Invoke();
        }

        public void Dispose()
        {
            _selectedElementStore.Select(null);
            ActivePage = null;
            OnActivePageChanged();
        }
    }
}
