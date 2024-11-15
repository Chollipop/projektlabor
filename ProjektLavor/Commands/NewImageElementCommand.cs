using Microsoft.Win32;
using ProjektLavor.Stores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektLavor.Commands
{
    public class NewImageElementCommand : CommandBase
    {
        private readonly ProjectStore _projectStore;

        public NewImageElementCommand(ProjectStore projectStore)
        {
            _projectStore = projectStore;
        }

        public override void Execute(object? parameter)
        {
            if (_projectStore.CurrentProject == null) return;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = @"Képfájlok (*.png,*.jpg,*.jpeg,*.bmp,*.gif,*.tiff,*.exif)|" +
                                        @"*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tiff;*.exif|" +
                                    @"Minden fájl (*.*)|" +
                                        @"*.*";
            openFileDialog.Title = "Kép megnyitása";
            if (openFileDialog.ShowDialog() ?? false)
            {
                if (!File.Exists(openFileDialog.FileName)) return;
                
                _projectStore.CurrentProject.AddNewImageField(openFileDialog.FileName);
            }
        }
    }
}
