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

        //public FlowDocument CurrentDocument { get; set; }
        public FlowDocument CurrentDocument => _projectStore.CurrentProject?.Document;

        public EditorViewModel(ProjectStore projectStore)
        {
            _projectStore = projectStore;
            _projectStore.CurrentProjectChanged += _projectStore_CurrentProjectChanged;

            //CurrentDocument = new FlowDocument();
            //CurrentDocument.PageHeight = 1123.2; //A4 page height
            //CurrentDocument.PageWidth = 796.8; //A4 page width
            //CurrentDocument.Background = Brushes.White;
        }

        private void _projectStore_CurrentProjectChanged()
        {
            OnPropertyChanged(nameof(CurrentDocument));
        }
    }
}
