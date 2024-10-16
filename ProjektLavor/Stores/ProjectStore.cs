using ProjektLavor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektLavor.Stores
{
    public class ProjectStore
    {
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
        internal void NewProject()
        {
            CurrentProject = new Project();
        }
        internal void CloseProject()
        {
            CurrentProject = null;
        }


        private void OnCurrentProjectChanged()
        {
            CurrentProjectChanged?.Invoke();
        }
    }
}
