using Microsoft.Win32;
using ProjektLavor.Stores;
using System;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;

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
            {
                MessageBox.Show(e.Message);
            }
        }

        public string SerializeFixedPage(FixedPage fixedPage)
        {
            RemoveContextMenu(fixedPage);
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter))
                {
                    XamlWriter.Save(fixedPage, xmlWriter);
                    return stringWriter.ToString();
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
