using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using ProjektLavor.Models;
using ProjektLavor.Stores;

namespace ProjektLavor.Commands
{
    public class DeletePageCommand : CommandBase
    {
        private ProjectStore _projectStore;
        private SelectedElementStore _selectedElementStore;

        public DeletePageCommand(SelectedElementStore selectedElementStore, ProjectStore projectStore)
        {
            _selectedElementStore = selectedElementStore;
            _projectStore = projectStore;
        }

        public override void Execute(object? parameter)
        {
            try
            {
                if (_projectStore.CurrentProject == null) return;
                if (_projectStore.CurrentProject.ActivePage == null) return;
                if (_projectStore.CurrentProject.Document.Pages.Count < 2) return;

                var result = MessageBox.Show("Biztosan törölni kívánja? Ez a művelet nem visszavonható!", "Törlés megerősítése", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes) return;

                var pageToDelete = _projectStore.CurrentProject.ActivePage;
                int newPageCount = _projectStore.CurrentProject.Document.Pages.Count - 1;

                Project newProject = new Project(_selectedElementStore, _projectStore);
                for (int i = 1; i < newPageCount; i++)
                {
                    newProject.AddBlankPage();
                }

                int newIndex = 0;
                for (int i = 0; i < _projectStore.CurrentProject.Document.Pages.Count; i++)
                {
                    if (_projectStore.CurrentProject.Document.Pages[i].Child != pageToDelete)
                    {
                        var newPage = newProject.Document.Pages[newIndex].Child;
                        var oldPage = _projectStore.CurrentProject.Document.Pages[i].Child;
                        newPage.Background = oldPage.Background;

                        var newChildren = newProject.Document.Pages[newIndex].Child.Children;
                        var oldChildren = _projectStore.CurrentProject.Document.Pages[i].Child.Children;
                        var oldChildrenCopy = new List<UIElement>(oldChildren.Cast<UIElement>());

                        newChildren.Clear();
                        foreach (var child in oldChildrenCopy)
                        {
                            oldChildren.Remove(child);
                            newChildren.Add(child);
                        }
                        oldChildrenCopy.Clear();

                        newIndex++;
                    }
                }

                _projectStore.CurrentProject = new Project(_selectedElementStore, _projectStore);
                for (int i = 1; i < newPageCount; i++)
                {
                    _projectStore.NewPage();
                }

                for (int i = 0; i < newPageCount; i++)
                {
                    if (_projectStore.CurrentProject.Document.Pages[i].Child != pageToDelete)
                    {
                        var newPage = _projectStore.CurrentProject.Document.Pages[i].Child;
                        var oldPage = newProject.Document.Pages[i].Child;
                        newPage.Background = oldPage.Background;

                        var newChildren = _projectStore.CurrentProject.Document.Pages[i].Child.Children;
                        var oldChildren = newProject.Document.Pages[i].Child.Children;
                        var oldChildrenCopy = new List<UIElement>(oldChildren.Cast<UIElement>());

                        newChildren.Clear();
                        foreach (var child in oldChildrenCopy)
                        {
                            oldChildren.Remove(child);
                            newChildren.Add(child);
                        }
                        oldChildrenCopy.Clear();
                    }
                }

                _projectStore.ClearUndoRedoStacks();
            }
            catch (Exception ex)
            { }
        }
    }
}
