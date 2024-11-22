using ProjektLavor.Services;
using ProjektLavor.Stores;
using ProjektLavor.ViewModels;
using System.Windows.Controls;
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
                if (_selectedElementStore.SelectedElement == null) return;
                if (_selectedElementStore.SelectedElement.GetType() == typeof(Image) && ((Image)_selectedElementStore.SelectedElement).Tag?.ToString() != "ignore_wizard")
                {
                    _projectStore.SaveState();
                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(_selectedElementStore.SelectedElement);
                    foreach (var item in adornerLayer?.GetAdorners(_selectedElementStore.SelectedElement) ?? [])
                    {
                        if (item is FrameAdorner)
                        {
                            adornerLayer?.Remove(item);
                        }
                    }

                    adornerLayer?.Add(new FrameAdorner(_selectedElementStore.SelectedElement, _viewModel.SelectedImage.Source.Clone()));
                }
            }

            //_selectedElementStore.SelectedElement = null;
            _navigationService.Navigate();
        }
    }
}
