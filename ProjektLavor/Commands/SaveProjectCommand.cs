using Microsoft.Win32;
using ProjektLavor.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektLavor.Commands
{
    public class SaveProjectCommand : CommandBase
    {
        private ProjectStore _projectStore;

        public SaveProjectCommand(ProjectStore projectStore)
        {
            _projectStore = projectStore;
        }

        public override void Execute(object? property)
        {
            if (_projectStore.CurrentProject == null) return;

            if (string.IsNullOrEmpty(_projectStore.CurrentProjectFilePath))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Projektfájlok (*.proj)|*.proj|Minden fájl (*.*)|*.*",
                    DefaultExt = ".proj"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    _projectStore.SaveProject(saveFileDialog.FileName);
                }
            }
            else
            {
                _projectStore.SaveProject(_projectStore.CurrentProjectFilePath);
            }
        }
    }
}
