using ProjektLavor.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace ProjektLavor.ViewModels
{
    public class EditorViewModel : ViewModelBase
    {
        private ProjectStore _projectStore;
        public FixedDocument CurrentDocument => _projectStore.CurrentProject?.Document;

        public int DocumentZoom { get; set; } = 80;

        public EditorViewModel(ProjectStore projectStore)
        {
            _projectStore = projectStore;
            _projectStore.CurrentProjectChanged += _projectStore_CurrentProjectChanged;
        }

        private void _projectStore_CurrentProjectChanged()
        {
            OnPropertyChanged(nameof(CurrentDocument));
        }
    }
}
