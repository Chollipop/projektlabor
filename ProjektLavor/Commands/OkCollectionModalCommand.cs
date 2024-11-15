using ProjektLavor.Services;
using ProjektLavor.Stores;
using ProjektLavor.ViewModels;
using System.Windows.Documents;

namespace ProjektLavor.Commands
{
    internal class OkCollectionModalCommand : CommandBase
    {
        private readonly CollectionBrowserViewModel _viewModel;
        private readonly INavigationService _navigationService;
        private readonly ProjectStore _projectStore;
        private readonly SelectedElementStore? _selectedElementStore;

        public OkCollectionModalCommand(CollectionBrowserViewModel viewModel, INavigationService navigationService, ProjectStore projectStore, SelectedElementStore? selectedElementStore = null)
        {
            _viewModel = viewModel;
            _navigationService = navigationService;
            _projectStore = projectStore;
            _selectedElementStore = selectedElementStore;
        }

        public override void Execute(object? parameter)
        {
            if (_viewModel?.SelectedImage == null) return;

            if (_selectedElementStore == null)
            {
                _projectStore.CurrentProject.AddNewImageField(_viewModel.SelectedImage.Source.Clone(), true);
            }
            else
            {
                _projectStore.SaveState();

                AdornerLayer.GetAdornerLayer((FixedPage)_selectedElementStore.SelectedElement.Parent).Add(new FrameAdorner(_selectedElementStore.SelectedElement, _viewModel.SelectedImage.Source.Clone()));
            }
            _navigationService.Navigate();
        }
    }
}
