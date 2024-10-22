using ProjektLavor.Commands;
using ProjektLavor.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ProjektLavor.ViewModels
{
    public class EditorViewModel : ViewModelBase
    {
        private ProjectStore _projectStore;
        private SelectedElementStore _selectedElementStore;
        public FixedDocument CurrentDocument => _projectStore.CurrentProject?.Document;
        public bool HasSelectedItem => _selectedElementStore?.SelectedElement != null;

        public ICommand ApplyPropertiesCommand { get; set; }

        public int DocumentZoom { get; set; } = 80;

        public EditorViewModel(ProjectStore projectStore, SelectedElementStore selectedElementStore)
        {
            _projectStore = projectStore;
            _selectedElementStore = selectedElementStore;
            _projectStore.CurrentProjectChanged += _projectStore_CurrentProjectChanged;
            _selectedElementStore.SelectedElementChanged += _selectedElementStore_SelectedElementChanged;

            ApplyPropertiesCommand = new ApplyPropertiesCommand(_selectedElementStore);
        }

        private void _selectedElementStore_SelectedElementChanged()
        {
            OnPropertyChanged(nameof(HasSelectedItem));
        }

        private void _projectStore_CurrentProjectChanged()
        {
            OnPropertyChanged(nameof(CurrentDocument));
        }

        public override void Dispose()
        {
            _projectStore.CurrentProjectChanged -= _projectStore_CurrentProjectChanged;
            _selectedElementStore.SelectedElementChanged -= _selectedElementStore_SelectedElementChanged;
        }
    }
}
