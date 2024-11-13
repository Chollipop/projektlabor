using ProjektLavor.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektLavor.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly ModalNavigationStore _modalNavigationStore;
        private readonly ProjectStore _projectStore;

        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;
        public ViewModelBase CurrentModalViewModel => _modalNavigationStore.CurrentViewModel;
        public bool IsModalOpen => _modalNavigationStore.IsOpen;

        public string Title => string.IsNullOrEmpty(_projectStore.CurrentProjectFilePath)
            ? "Fotókönyv készítő"
            : $"Projekt - {_projectStore.CurrentProjectFilePath}";

        public MainViewModel(NavigationStore navigationStore, ModalNavigationStore modalNavigationStore, ProjectStore projectStore)
        {
            _navigationStore = navigationStore;
            _modalNavigationStore = modalNavigationStore;
            _projectStore = projectStore;

            _modalNavigationStore.CurrentViewModelChanged += _modalNavigationStore_CurrentViewModelChanged;
            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _projectStore.CurrentProjectFilePathChanged += OnCurrentProjectFilePathChanged;
        }

        private void _modalNavigationStore_CurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentModalViewModel));
            OnPropertyChanged(nameof(IsModalOpen));
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        private void OnCurrentProjectFilePathChanged()
        {
            OnPropertyChanged(nameof(Title));
        }

        public override void Dispose()
        {
            _navigationStore.CurrentViewModelChanged -= OnCurrentViewModelChanged;
            _projectStore.CurrentProjectFilePathChanged -= OnCurrentProjectFilePathChanged;
            base.Dispose();
        }
    }
}
