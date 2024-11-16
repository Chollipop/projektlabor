using ProjektLavor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml.Linq;
using System.Xml;
using System.Windows;
using ProjektLavor.Commands;
using System.Windows.Controls;
using System.Reflection.Metadata;
using System.IO;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using System.Windows.Media;

namespace ProjektLavor.Stores
{
    public class ProjectStore
    {
        private Stack<string> _undoStack;
        private Stack<string> _redoStack;

        private SelectedElementStore _selectedElementStore;
        private string _currentProjectFilePath;

        public string CurrentProjectFilePath
        {
            get => _currentProjectFilePath;
            set
            {
                if (_currentProjectFilePath != value)
                {
                    _currentProjectFilePath = value;
                    OnCurrentProjectFilePathChanged();
                }
            }
        }

        public event Action CurrentProjectFilePathChanged;

        private void OnCurrentProjectFilePathChanged()
        {
            CurrentProjectFilePathChanged?.Invoke();
        }

        public delegate void NewPageAddedEventHandler(PageContent pageContent);
        public event NewPageAddedEventHandler NewPageAdded;
        public event Action CurrentProjectChanged;
        private Project _currentProject;
        public Project CurrentProject
        {
            get => _currentProject;
            set
            {
                _currentProject?.Dispose();
                _currentProject = value;
                OnCurrentProjectChanged();
            }
        }

        public ProjectStore(SelectedElementStore selectedElementStore)
        {
            _selectedElementStore = selectedElementStore;
            _undoStack = new Stack<string>();
            _redoStack = new Stack<string>();
        }

        public void NewProject()
        {
            CurrentProject = new Project(_selectedElementStore, this);
#if DEBUG
            CurrentProject.AddTestElements();
#endif
        }
        public void CloseProject()
        {
            CurrentProject = null;
        }
        public void NewPage(PageContent newPage = null, bool rotate = false)
        {
            PageContent _newPage = _currentProject.AddBlankPage(newPage, rotate);
            NewPageAdded?.Invoke(_newPage);
        }

        private void OnCurrentProjectChanged()
        {
            CurrentProjectChanged?.Invoke();
        }

        public void SaveProject(string filePath)
        {
            try
            {
                if (CurrentProject?.Document == null) return;

                string documentState = SerializeDocument(CurrentProject.Document);
                System.IO.File.WriteAllText(filePath, documentState);
                CurrentProjectFilePath = filePath;
            }
            catch (Exception e)
            { }
        }

        public void LoadProject(string filePath)
        {
            try
            {
                if (!System.IO.File.Exists(filePath)) return;

                string documentState = System.IO.File.ReadAllText(filePath);
                FixedDocument deserializedDocument = DeserializeDocument(documentState);

                var alreadyRotatedPages = deserializedDocument.Pages
                    .Where(p => p.Child.Width > p.Child.Height);
                var alreadyRotatedPageIndexes = new List<int>();

                for (int i = 0; i < deserializedDocument.Pages.Count; i++)
                {
                    foreach (var page in alreadyRotatedPages)
                    {
                        if (deserializedDocument.Pages[i].Child == page.Child)
                        {
                            alreadyRotatedPageIndexes.Add(i);
                        }
                    }
                }

                if (CurrentProject == null)
                {
                    if (alreadyRotatedPageIndexes.Contains(0))
                    {
                        CurrentProject = new Project(_selectedElementStore, this, true);
                    }
                    else
                    {
                        CurrentProject = new Project(_selectedElementStore, this);
                    }

                    for (int i = 1; i < deserializedDocument.Pages.Count; i++)
                    {
                        if (alreadyRotatedPageIndexes.Contains(i))
                        {
                            NewPage(null, true);
                        }
                        else
                        {
                            NewPage();
                        }
                    }
                }

                for (int i = 0; i < deserializedDocument.Pages.Count; i++)
                {
                    var currentPage = CurrentProject.Document.Pages[i].Child;
                    var deserializedPage = deserializedDocument.Pages[i].Child;
                    currentPage.Background = deserializedPage.Background;
                    currentPage.Tag = deserializedPage.Tag;

                    var currentChildren = CurrentProject.Document.Pages[i].Child.Children;
                    var deserializedChildren = deserializedDocument.Pages[i].Child.Children;
                    var deserializedChildrenCopy = new List<UIElement>(deserializedChildren.Cast<UIElement>());

                    currentChildren.Clear();
                    foreach (var child in deserializedChildrenCopy)
                    {
                        deserializedChildren.Remove(child);
                        currentChildren.Add(child);
                    }
                    deserializedChildrenCopy.Clear();
                }
                CurrentProjectFilePath = filePath;

                OnCurrentProjectChanged();
            }
            catch (Exception e)
            { }
        }

        public void SaveState()
        {
            try
            {
                if (CurrentProject?.Document == null) return;

                string documentState = SerializeDocument(CurrentProject.Document);
                _undoStack.Push(documentState);
                _redoStack.Clear();
            }
            catch (Exception e)
            { }
        }

        private void RemoveContextMenu(FixedDocument document)
        {
            foreach (var page in document.Pages)
            {
                if (page.Child is FixedPage fixedPage)
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

        // Helper class for FrameAdorner state
        [Serializable]
        public class FrameAdornerState
        {
            public string AdornedElement { get; set; }
            public string SourceUri { get; set; }
        }

        private string SerializeDocument(FixedDocument document)
        {
            try
            {
                RemoveContextMenu(document);

                XDocument xDocument = new XDocument(new XElement("Document"));

                using (XmlWriter writer = xDocument.Root.CreateWriter())
                {
                    System.Windows.Markup.XamlWriter.Save(document, writer);
                }

                // Serialize FrameAdorner states
                foreach (var page in document.Pages)
                {
                    if (page.Child is FixedPage fixedPage)
                    {
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
                    }
                }

                RecreateContextMenu(document);
                return xDocument.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private List<Tuple<string, Image, BitmapImage>> adorners = new List<Tuple<string, Image, BitmapImage>>();

        public void AddAdorners()
        {
            if (adorners.Count == 0) return;

            foreach (var adorner in adorners)
            {
                foreach (var page in CurrentProject.Document.Pages)
                {
                    if (page.Child is FixedPage fixedPage)
                    {
                        if (fixedPage.Tag?.ToString() == adorner.Item1)
                        {

                            bool hasFrameAdorner = false;
                            foreach (var item in AdornerLayer.GetAdornerLayer(adorner.Item2)?.GetAdorners(adorner.Item2) ?? [])
                            {
                                if (item is FrameAdorner)
                                {
                                    hasFrameAdorner = true;
                                    break;
                                }
                            }
                            if(!hasFrameAdorner)
                            {
                                AdornerLayer.GetAdornerLayer(adorner.Item2)?.Add(new FrameAdorner(adorner.Item2, adorner.Item3));
                            }
                        }
                    }
                }
            }
        }

        public void ClearAdorners()
        {
            adorners.Clear();
        }

        private FixedDocument DeserializeDocument(string documentState)
        {
            try
            {
                XDocument xDocument = XDocument.Parse(documentState);
                XElement fixedDocumentElement = (XElement)xDocument.Root.FirstNode;

                FixedDocument document;
                using (XmlReader reader = fixedDocumentElement.CreateReader())
                {
                    document = (FixedDocument)System.Windows.Markup.XamlReader.Load(reader);
                }

                 ClearAdorners();
                // Deserialize FrameAdorner states
                foreach (var adornerElement in xDocument.Root.Elements("FrameAdornerState"))
                {
                    var frameAdornerState = new FrameAdornerState
                    {
                        AdornedElement = adornerElement.Element("AdornedElement").Value,
                        SourceUri = adornerElement.Element("SourceUri").Value
                    };

                    foreach (var page in document.Pages)
                    {
                        if (page.Child is FixedPage fixedPage)
                        {
                            foreach (FrameworkElement e in fixedPage.Children)
                            {
                                FrameworkElement element = e;
                                if (element is AdornerDecorator) element = (FrameworkElement)((AdornerDecorator)element).Child;

                                if (element is Image image && image.Tag.ToString() == frameAdornerState.AdornedElement)
                                {
                                    adorners.Add(new Tuple<string, Image, BitmapImage>(fixedPage.Tag.ToString(), image, new BitmapImage(new Uri(frameAdornerState.SourceUri))));
                                }
                            }
                        }
                    }
                }

                RecreateContextMenu(document);
                return document;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private void RecreateContextMenu(FixedDocument document)
        {
            foreach (var page in document.Pages)
            {
                if (page.Child is FixedPage fixedPage)
                {
                    foreach (var element in fixedPage.Children)
                    {
                        if (element is TextBlock textBlock)
                        {
                            textBlock.ContextMenu = CreateTextBlockContextMenu(textBlock);
                        }
                        if (element is AdornerDecorator decorator)
                        {
                            Image image = (Image)decorator.Child;
                            image.ContextMenu = CreateImageContextMenu(image);
                        }
                    }
                }
            }
        }

        private ContextMenu CreateTextBlockContextMenu(TextBlock textBlock)
        {
            ContextMenu contextMenu = new ContextMenu();

            MenuItem removeElementMenuItem = new MenuItem();
            removeElementMenuItem.Header = "Törlés";
            removeElementMenuItem.Command = new RemoveElementCommand();
            removeElementMenuItem.CommandParameter = Tuple.Create((FrameworkElement)textBlock, _selectedElementStore, this);

            contextMenu.Items.Add(removeElementMenuItem);

            return contextMenu;
        }

        public ContextMenu CreateImageContextMenu(Image image)
        {
            ContextMenu contextMenu = new ContextMenu();

            MenuItem changeImageMenuItem = new MenuItem();
            changeImageMenuItem.Header = "Kép módosítása";
            changeImageMenuItem.Command = new ChangeImageSourceCommand();
            changeImageMenuItem.CommandParameter = Tuple.Create(image, this);

            MenuItem removeElementMenuItem = new MenuItem();
            removeElementMenuItem.Header = "Törlés";
            removeElementMenuItem.Command = new RemoveElementCommand();
            removeElementMenuItem.CommandParameter = Tuple.Create((FrameworkElement)image, _selectedElementStore, this);

            MenuItem removeFrameMenuItem = new MenuItem();
            removeFrameMenuItem.Header = "Keret törlése";
            removeFrameMenuItem.Command = new RemoveFrameCommand();
            removeFrameMenuItem.CommandParameter = Tuple.Create((FrameworkElement)image, _selectedElementStore, this);

            contextMenu.Items.Add(changeImageMenuItem);
            contextMenu.Items.Add(removeElementMenuItem);
            contextMenu.Items.Add(removeFrameMenuItem);

            return contextMenu;
        }

        public void Undo()
        {
            try
            {
                if (_undoStack.Count > 0)
                {
                    _selectedElementStore.Select(null);

                    string currentState = SerializeDocument(CurrentProject.Document);
                    _redoStack.Push(currentState);

                    string previousState = _undoStack.Pop();
                    FixedDocument deserializedDocument = DeserializeDocument(previousState);

                    for (int i = 0; i < deserializedDocument.Pages.Count; i++)
                    {
                        var currentPage = CurrentProject.Document.Pages[i].Child;
                        var deserializedPage = deserializedDocument.Pages[i].Child;
                        currentPage.Background = deserializedPage.Background;

                        var currentChildren = CurrentProject.Document.Pages[i].Child.Children;
                        var deserializedChildren = deserializedDocument.Pages[i].Child.Children;
                        var deserializedChildrenCopy = new List<UIElement>(deserializedChildren.Cast<UIElement>());

                        currentChildren.Clear();
                        foreach (var child in deserializedChildrenCopy)
                        {
                            deserializedChildren.Remove(child);
                            currentChildren.Add(child);
                        }
                        deserializedChildrenCopy.Clear();
                    }

                    OnCurrentProjectChanged();

                    AddAdorners();
                }
            }
            catch (Exception e)
            { }
        }

        public void Redo()
        {
            try
            {
                if (_redoStack.Count > 0)
                {
                    _selectedElementStore.Select(null);

                    string currentState = SerializeDocument(CurrentProject.Document);
                    _undoStack.Push(currentState);

                    string nextState = _redoStack.Pop();
                    FixedDocument deserializedDocument = DeserializeDocument(nextState);

                    for (int i = 0; i < deserializedDocument.Pages.Count; i++)
                    {
                        var currentPage = CurrentProject.Document.Pages[i].Child;
                        var deserializedPage = deserializedDocument.Pages[i].Child;
                        currentPage.Background = deserializedPage.Background;

                        var currentChildren = CurrentProject.Document.Pages[i].Child.Children;
                        var deserializedChildren = deserializedDocument.Pages[i].Child.Children;
                        var deserializedChildrenCopy = new List<UIElement>(deserializedChildren.Cast<UIElement>());

                        currentChildren.Clear();
                        foreach (var child in deserializedChildrenCopy)
                        {
                            deserializedChildren.Remove(child);
                            currentChildren.Add(child);
                        }
                        deserializedChildrenCopy.Clear();
                    }

                    OnCurrentProjectChanged();

                    AddAdorners();
                }
            }
            catch (Exception e)
            { }
        }

        public void ClearUndoRedoStacks()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}
