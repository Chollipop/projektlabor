using ProjektLavor.Commands;
using ProjektLavor.Services;
using ProjektLavor.Stores;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjektLavor.ViewModels
{
    public class TextElementInputViewModel : ViewModelBase
    {
        public ICommand SaveButtonCommand { get; set; }
        public string TextInput { get; set; }

        public TextElementInputViewModel(ProjectStore projectStore, INavigationService navigationService, TextBlock? selectedElement = null)
        {
            TextInput = selectedElement?.Text ?? string.Empty;
            if(selectedElement != null)
            {
                SaveButtonCommand = new EditTextElementCommand(this, selectedElement, navigationService);
            }
            else
            {
                SaveButtonCommand = new NewTextElementCommand(this, projectStore, navigationService);
            }
        }
    }
}
