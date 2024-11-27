using Microsoft.Win32;
using ProjektLavor.Stores;
using System;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xaml;
using System.Xml;
using System.Xml.Linq;
using static ProjektLavor.Stores.ProjectStore;

namespace ProjektLavor.Commands
{
    public class SavePageAsTemplateCommand : CommandBase
    {
        private ProjectStore _projectStore;

        public SavePageAsTemplateCommand(ProjectStore projectStore)
        {
            _projectStore = projectStore;
        }

        public override void Execute(object? parameter)
        {
            if (_projectStore.CurrentProject == null) return;
            if (_projectStore.CurrentProject.Document == null) return;
            if (_projectStore.CurrentProject.ActivePage == null) return;

            try
            {
                FixedPage pageToSave = _projectStore.CurrentProject.ActivePage;

                foreach (var element in pageToSave.Children)
                {
                    if (element is FrameworkElement frameworkElement)
                    {
                        if (double.IsNaN(frameworkElement.Width))
                        {
                            frameworkElement.Width = frameworkElement.ActualWidth;
                        }
                        if (double.IsNaN(frameworkElement.Height))
                        {
                            frameworkElement.Height = frameworkElement.ActualHeight;
                        }
                    }
                }

                string serializedPage = SerializeFixedPage(pageToSave);

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Template files (*.template)|*.template",
                    DefaultExt = ".template",
                    AddExtension = true
                };

                bool? result = saveFileDialog.ShowDialog();
                if (result == true)
                {
                    string filePath = saveFileDialog.FileName;
                    if (!filePath.EndsWith(".template"))
                    {
                        filePath += ".template";
                    }
                    File.WriteAllText(filePath, serializedPage);
                }
            }
            catch (Exception e)
            { }
        }

        public string SerializeFixedPage(FixedPage fixedPage)
        {
            RemoveContextMenu(fixedPage);

            XDocument xDocument = new XDocument(new XElement("Document"));

            using (XmlWriter xmlWriter = xDocument.Root.CreateWriter())
            {
                System.Windows.Markup.XamlWriter.Save(fixedPage, xmlWriter);
            }

            foreach (FrameworkElement e in fixedPage.Children)
            {
                FrameworkElement element = e;
                if (element is AdornerDecorator) element = (FrameworkElement)((AdornerDecorator)element).Child;

                var adornerLayer = AdornerLayer.GetAdornerLayer(element);
                if (adornerLayer != null)
                {
                    var adorners = adornerLayer.GetAdorners(element);
                    if (adorners != null)
                    {
                        foreach (var adorner in adorners)
                        {
                            if (adorner is FrameAdorner frameAdorner)
                            {
                                var frameAdornerState = new FrameAdornerState
                                {
                                    AdornedElement = ((Image)element).Tag?.ToString() ?? string.Empty,
                                    SourceUri = frameAdorner.ImageSource.ToString()
                                };

                                XElement adornerElement = new XElement("FrameAdornerState",
                                    new XElement("AdornedElement", frameAdornerState.AdornedElement),
                                    new XElement("SourceUri", frameAdornerState.SourceUri)
                                );

                                xDocument.Root.Add(adornerElement);
                            }
                        }
                    }
                }
            }

            RecreateContextMenu(fixedPage);
            return xDocument.ToString();
        }

        private void RecreateContextMenu(FixedPage fixedPage)
        {
            foreach (var element in fixedPage.Children)
            {
                if (element is TextBlock textBlock)
                {
                    textBlock.ContextMenu = _projectStore.CreateTextBlockContextMenu(textBlock);
                }
                if (element is AdornerDecorator decorator)
                {
                    Image image = (Image)decorator.Child;
                    image.ContextMenu = _projectStore.CreateImageContextMenu(image, image.Tag?.ToString() == "ignore_wizard");
                }
            }
        }

        private void RemoveContextMenu(FixedPage fixedPage)
        {
            foreach (var element in fixedPage.Children)
            {
                if (element is TextBlock textBlock)
                {
                    textBlock.ContextMenu = null;
                }
                if (element is AdornerDecorator decorator)
                {
                    ((Image)decorator.Child).ContextMenu = null;
                }
            }
        }
    }
}
