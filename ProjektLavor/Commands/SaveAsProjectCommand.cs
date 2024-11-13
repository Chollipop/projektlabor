using Microsoft.Win32;
using ProjektLavor.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjektLavor.Commands
{
    public class SaveAsProjectCommand : CommandBase
    {
        private ProjectStore _projectStore;

        public SaveAsProjectCommand(ProjectStore projectStore)
        {
            _projectStore = projectStore;
        }

        public override void Execute(object? parameter)
        {
            if (_projectStore.CurrentProject == null) return;

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Project Files (*.proj)|*.proj|All Files (*.*)|*.*",
                DefaultExt = ".proj"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                _projectStore.SaveProject(saveFileDialog.FileName);
            }
        }
    }
}
