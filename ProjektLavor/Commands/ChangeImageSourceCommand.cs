using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjektLavor.Commands
{
    class ChangeImageSourceCommand : CommandBase
    {
        public override void Execute(object? parameter)
        {
            if (parameter == null || parameter.GetType() != typeof(Image)) return;
            Image image = (Image)parameter;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = @"Képfájlok (*.png,*.jpg,*.jpeg,*.bmp,*.gif,*.tiff,*.exif)|" +
                                        @"*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tiff;*.exif|" +
                                    @"Minden fájl (*.*)|" +
                                        @"*.*";
            openFileDialog.Title = "Kép megnyitása";
            if (openFileDialog.ShowDialog() ?? false)
            {
                if (!File.Exists(openFileDialog.FileName)) return;

                if (double.IsNaN(image.Width) || double.IsNaN(image.Height))
                {
                    double width = image.ActualWidth;
                    double height = image.ActualHeight;
                    image.Width = width;
                    image.Height = height;
                }

                image.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }
    }
}
