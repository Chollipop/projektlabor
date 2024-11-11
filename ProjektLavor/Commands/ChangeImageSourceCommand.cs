using Microsoft.Win32;
using ProjektLavor.Stores;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ProjektLavor.Commands
{
    public class ChangeImageSourceCommand : CommandBase
    {
        public override void Execute(object? parameter)
        {
            if (parameter is not Tuple<Image, ProjectStore> tuple) return;

            var (image, projectStore) = tuple;

            projectStore.SaveState();

            if (image == null) return;

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
