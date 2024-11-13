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
        public void NewPage()
        {
            PageContent newPage = _currentProject.AddBlankPage();
            NewPageAdded?.Invoke(newPage);
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

                if (CurrentProject == null)
                {
                    CurrentProject = new Project(_selectedElementStore, this);
                    for (int i = 1; i < deserializedDocument.Pages.Count; i++)
                    {
                        this.NewPage();
                    }
                }

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

        public string SerializeDocument(FixedDocument document)
        {
            try
            {
            RemoveContextMenu(document);

            XDocument xDocument = new XDocument();
            using (XmlWriter writer = xDocument.CreateWriter())
            {
                System.Windows.Markup.XamlWriter.Save(document, writer);
            }

            RecreateContextMenu(document);
            return xDocument.ToString();
            }
            catch (Exception e)
            {
                return null;
            }
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
                        if (element is Image image)
                        {
                            image.ContextMenu = null;
                        }
                    }
                }
            }
        }

        public FixedDocument DeserializeDocument(string documentState)
        {
            try
            {
                XDocument xDocument = XDocument.Parse(documentState);
                using (XmlReader reader = xDocument.CreateReader())
                {
                    FixedDocument document = (FixedDocument)System.Windows.Markup.XamlReader.Load(reader);
                    RecreateContextMenu(document);
                    return document;
                }
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
                        if (element is Image image)
                        {
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

        private ContextMenu CreateImageContextMenu(Image image)
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

            contextMenu.Items.Add(changeImageMenuItem);
            contextMenu.Items.Add(removeElementMenuItem);

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
