using ProjektLavor.Stores;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ProjektLavor.Commands
{
    public class ChangeColorCommand : CommandBase
    {
        private ProjectStore _projectStore;
        public ChangeColorCommand(ProjectStore projectStore)
        {
            _projectStore = projectStore;
        }

        public override void Execute(object? parameter)
        {
            if (parameter == null || parameter.GetType() != typeof(Button)) return;

            if(_projectStore?.CurrentProject?.Document == null) return;
            foreach(PageContent pageContent in _projectStore.CurrentProject.Document.Pages)
            {
                pageContent.Child.Background = (parameter as Button).Background;
            }
        }
    }
}
