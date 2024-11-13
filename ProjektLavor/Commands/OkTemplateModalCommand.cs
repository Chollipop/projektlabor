using ProjektLavor.Services;
using ProjektLavor.ViewModels;
using ProjektLavor.Stores;
using System.Printing;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ProjektLavor.Commands
{
    public class OkTemplateModalCommand : CommandBase
    {
        private readonly TemplateBrowserViewModel _templateBrowserViewModel;

        public OkTemplateModalCommand(TemplateBrowserViewModel templateBrowserViewModel)
        {
            _templateBrowserViewModel = templateBrowserViewModel;
        }

        public override void Execute(object? parameter)
        {
            _templateBrowserViewModel.AddPage();
        }
    }
}
