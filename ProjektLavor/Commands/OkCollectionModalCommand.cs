using ProjektLavor.Services;
using ProjektLavor.Stores;
using ProjektLavor.ViewModels;

namespace ProjektLavor.Commands
{
    internal class OkCollectionModalCommand : CommandBase
    {
        private readonly CollectionBrowserViewModel _viewModel;
        private readonly INavigationService _navigationService;
        private readonly ProjectStore _projectStore;

        public OkCollectionModalCommand(CollectionBrowserViewModel viewModel, INavigationService navigationService, ProjectStore projectStore)
        {
            _viewModel = viewModel;
            _navigationService = navigationService;
            _projectStore = projectStore;
        }

        public override void Execute(object? parameter)
        {
            if(_viewModel?.SelectedImage == null) return;

            _projectStore.CurrentProject.AddNewImageField(_viewModel.SelectedImage.Source.Clone(), true);
            _navigationService.Navigate();
        }
    }
}
