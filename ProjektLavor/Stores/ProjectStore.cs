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

namespace ProjektLavor.Stores
{
    public class ProjectStore
    {
        private Stack<string> _undoStack = new Stack<string>();
        private Stack<string> _redoStack = new Stack<string>();

        private SelectedElementStore _selectedElementStore;

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
        }

        public void NewProject()
        {
            CurrentProject = new Project(_selectedElementStore);
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

        public void SaveState()
        {
            if (CurrentProject?.Document == null) return;

            string documentState = SerializeDocument(CurrentProject.Document);
            _undoStack.Push(documentState);
            _redoStack.Clear();
        }

        private string SerializeDocument(FixedDocument document)
        {
            XDocument xDocument = new XDocument();
            using (XmlWriter writer = xDocument.CreateWriter())
            {
                System.Windows.Markup.XamlWriter.Save(document, writer);
            }
            return xDocument.ToString();
        }

        private FixedDocument DeserializeDocument(string documentState)
        {
            XDocument xDocument = XDocument.Parse(documentState);
            using (XmlReader reader = xDocument.CreateReader())
            {
                return (FixedDocument)System.Windows.Markup.XamlReader.Load(reader);
            }
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                _selectedElementStore.Select(null);

                string currentState = SerializeDocument(CurrentProject.Document);
                _redoStack.Push(currentState);

                string previousState = _undoStack.Pop();
                CurrentProject.Document = DeserializeDocument(previousState);
                OnCurrentProjectChanged();
            }
        }

        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                _selectedElementStore.Select(null);

                string currentState = SerializeDocument(CurrentProject.Document);
                _undoStack.Push(currentState);

                string nextState = _redoStack.Pop();
                CurrentProject.Document = DeserializeDocument(nextState);
                OnCurrentProjectChanged();
            }
        }

        public void ClearUndoRedoStacks()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}
