using ProjektLavor.Services;
using ProjektLavor.Stores;
using ProjektLavor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjektLavor.Commands
{
    public class EditTextElementCommand : CommandBase
    {
        private ProjectStore _projectStore;
        private readonly TextElementInputViewModel _viewModel;
        private readonly TextBlock _element;
        private readonly INavigationService _navigationService;

        public EditTextElementCommand(TextElementInputViewModel viewModel, TextBlock element, INavigationService navigationService, ProjectStore projectStore)
        {
            _viewModel = viewModel;
            _element = element;
            _navigationService = navigationService;
            _projectStore = projectStore;
        }

        public override void Execute(object? parameter)
        {
            _projectStore.SaveState();

            _navigationService.Navigate();
            if (_element == null || String.IsNullOrEmpty(_viewModel.TextInput)) return;
            _element.Text = _viewModel.TextInput;
        }
    }
}
