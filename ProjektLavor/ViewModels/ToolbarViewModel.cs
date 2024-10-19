using System;
using ProjektLavor.Commands;
using ProjektLavor.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjektLavor.ViewModels
{
    public class ToolbarViewModel : ViewModelBase
    {
        public ICommand OpenNewTextElementModalCommand { get; set; }

        public ToolbarViewModel(INavigationService newTextElementNavigationService)
        {
            OpenNewTextElementModalCommand = new NavigateCommand(newTextElementNavigationService);
        }
    }
}
