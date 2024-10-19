using ProjektLavor.Commands;
using ProjektLavor.Services;
using ProjektLavor.Stores;
using System.Windows.Input;

namespace ProjektLavor.ViewModels
{
    public class NewTextElementViewModel : ViewModelBase
    {
        public ICommand NewTextElementCommand { get; set; }
        public string TextInput { get; set; }

        public NewTextElementViewModel(ProjectStore projectStore, INavigationService navigationService)
        {
            NewTextElementCommand = new NewTextElementCommand(this, projectStore, navigationService);
        }
    }
}
