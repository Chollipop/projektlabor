using Microsoft.Win32;
using ProjektLavor.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektLavor.Commands
{
    public class OpenProjectCommand : CommandBase
    {
        private ProjectStore _projectStore;

        public OpenProjectCommand(ProjectStore projectStore)
        {
            _projectStore = projectStore;
        }

        public override void Execute(object? parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Projektfájlok (*.proj)|*.proj|Minden fájl (*.*)|*.*",
                DefaultExt = ".proj"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _projectStore.LoadProject(openFileDialog.FileName);
            }
        }
    }
}
