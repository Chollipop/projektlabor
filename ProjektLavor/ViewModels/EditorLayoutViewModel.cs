using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektLavor.ViewModels
{
    public class EditorLayoutViewModel : ViewModelBase
    {
        public ToolbarViewModel ToolbarViewModel { get; }
        public EditorViewModel EditorViewModel { get; }

        public EditorLayoutViewModel(ToolbarViewModel toolbarViewModel, EditorViewModel editorViewModel)
        {
            ToolbarViewModel = toolbarViewModel;
            EditorViewModel = editorViewModel;
        }
    }
}
