﻿using ProjektLavor.Services;
using ProjektLavor.Stores;
using ProjektLavor.ViewModels;

namespace ProjektLavor.Commands
{
    public class NewTextElementCommand : CommandBase
    {
        private readonly TextElementInputViewModel _viewModel;
        private readonly ProjectStore _projectStore;
        private readonly INavigationService _navigationService;

        public NewTextElementCommand(TextElementInputViewModel viewModel, ProjectStore projectStore, INavigationService navigationService)
        {
            _viewModel = viewModel;
            _projectStore = projectStore;
            _navigationService = navigationService;
        }

        public override void Execute(object? parameter)
        {
            _projectStore.SaveState();

            _navigationService.Navigate();
            if (_projectStore.CurrentProject == null || String.IsNullOrEmpty(_viewModel.TextInput)) return;
            _projectStore.CurrentProject.AddNewTextField(_viewModel.TextInput);
        }
    }
}
