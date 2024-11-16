using ProjektLavor.Models;
using ProjektLavor.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace ProjektLavor.Commands
{
    public class RotatePageCommand : CommandBase
    {
        private readonly Size ISOA4 = new Size(796.8, 1123.2);
        private ProjectStore _projectStore;
        private SelectedElementStore _selectedElementStore;

        public RotatePageCommand(SelectedElementStore selectedElementStore, ProjectStore projectStore)
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

                var pageToRotate = _projectStore.CurrentProject.ActivePage;
                int pageToRotateIndex = 0;
                int pageCount = _projectStore.CurrentProject.Document.Pages.Count;
                var alreadyRotatedPages = _projectStore.CurrentProject.Document.Pages
                    .Where(p => p.Child.Width > p.Child.Height);
                var alreadyRotatedPageIndexes = new List<int>();

                for (int i = 0; i < pageCount; i++)
                {
                    foreach (var page in alreadyRotatedPages)
                    {
                        if (_projectStore.CurrentProject.Document.Pages[i].Child == page.Child)
                        {
                            alreadyRotatedPageIndexes.Add(i);
                        }
                    }
                }

                Project newProject;

                if (pageToRotate.Height > pageToRotate.Width && _projectStore.CurrentProject.Document.Pages[0].Child == pageToRotate)
                {
                    pageToRotateIndex = 0;
                    newProject = new Project(_selectedElementStore, _projectStore, true);
                }
                else if (alreadyRotatedPageIndexes.Contains(0))
                {
                    newProject = new Project(_selectedElementStore, _projectStore, true);
                }
                else
                {
                    newProject = new Project(_selectedElementStore, _projectStore);
                }

                for (int i = 1; i < pageCount; i++)
                {
                    if (_projectStore.CurrentProject.Document.Pages[i].Child == pageToRotate)
                    {
                        pageToRotateIndex = i;
                        if (pageToRotate.Height > pageToRotate.Width)
                        {
                            newProject.AddBlankPage(null, true);
                        }
                        else
                        {
                            newProject.AddBlankPage();
                        }
                    }
                    else if (alreadyRotatedPageIndexes.Contains(i))
                    {
                        newProject.AddBlankPage(null, true);
                    }
                    else
                    {
                        newProject.AddBlankPage();
                    }
                }

                double tempChildLeft, tempChildTop;
                for (int i = 0; i < pageCount; i++)
                {
                    var newPage = newProject.Document.Pages[i].Child;
                    var oldPage = _projectStore.CurrentProject.Document.Pages[i].Child;
                    newPage.Background = oldPage.Background;
                    newPage.Tag = oldPage.Tag;

                    var newChildren = newProject.Document.Pages[i].Child.Children;
                    var oldChildren = _projectStore.CurrentProject.Document.Pages[i].Child.Children;
                    var oldChildrenCopy = new List<UIElement>(oldChildren.Cast<UIElement>());

                    newChildren.Clear();
                    foreach (var child in oldChildrenCopy)
                    {
                        if (_projectStore.CurrentProject.Document.Pages[i].Child != pageToRotate)
                        {
                            oldChildren.Remove(child);
                            newChildren.Add(child);
                        }
                        else
                        {
                            tempChildLeft = FixedPage.GetLeft(child);
                            tempChildTop = FixedPage.GetTop(child);

                            oldChildren.Remove(child);
                            newChildren.Add(child);

                            FixedPage.SetLeft(child, tempChildTop);
                            FixedPage.SetTop(child, pageToRotate.Width - tempChildLeft - child.RenderSize.Width);
                        }

                    }
                    oldChildrenCopy.Clear();
                }

                if (pageToRotate.Height > pageToRotate.Width && pageToRotateIndex == 0)
                {
                    _projectStore.CurrentProject = new Project(_selectedElementStore, _projectStore, true);
                }
                else if (alreadyRotatedPageIndexes.Contains(0) && pageToRotateIndex != 0)
                {
                    _projectStore.CurrentProject = new Project(_selectedElementStore, _projectStore, true);
                }
                else
                {
                    _projectStore.CurrentProject = new Project(_selectedElementStore, _projectStore);
                }

                for (int i = 1; i < pageCount; i++)
                {
                    if (pageToRotateIndex == i)
                    {
                        if (pageToRotate.Height > pageToRotate.Width)
                        {
                            _projectStore.NewPage(null, true);
                        }
                        else
                        {
                            _projectStore.NewPage();
                        }
                    }
                    else if (alreadyRotatedPageIndexes.Contains(i))
                    {
                        _projectStore.NewPage(null, true);
                    }
                    else
                    {
                        _projectStore.NewPage();
                    }
                }

                for (int i = 0; i < pageCount; i++)
                {
                    var newPage = _projectStore.CurrentProject.Document.Pages[i].Child;
                    var oldPage = newProject.Document.Pages[i].Child;
                    newPage.Background = oldPage.Background;
                    newPage.Tag = oldPage.Tag;

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

                _projectStore.ClearUndoRedoStacks();
            }
            catch (Exception ex)
            { }
        }
    }
}
