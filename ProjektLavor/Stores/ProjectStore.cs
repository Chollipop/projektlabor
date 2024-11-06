using ProjektLavor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ProjektLavor.Stores
{
    public class ProjectStore
    {
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
    }
}
