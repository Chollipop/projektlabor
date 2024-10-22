using ProjektLavor.Services;
using ProjektLavor.Stores;
using ProjektLavor.ViewModels;

namespace ProjektLavor.Commands
{
    public class NewTextElementCommand : CommandBase
    {
        private readonly NewTextElementViewModel _viewModel;
        private readonly ProjectStore _projectStore;
        private readonly INavigationService _navigationService;

        public NewTextElementCommand(NewTextElementViewModel viewModel, ProjectStore projectStore, INavigationService navigationService)
        {
            _viewModel = viewModel;
            _projectStore = projectStore;
            _navigationService = navigationService;
        }

        public override void Execute(object? parameter)
        {
            _navigationService.Navigate();
            if (_projectStore.CurrentProject == null || String.IsNullOrEmpty(_viewModel.TextInput)) return;
            _projectStore.CurrentProject.AddNewTextField(_viewModel.TextInput);
            //_projectStore.CurrentProject.AddNewImageField(_viewModel.TextInput);
        }
    }
}
