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
    public class ChangeImageSourceCommand : CommandBase
    {
        private Image _element;

        public ChangeImageSourceCommand(Image image)
        {
            _element = image;
        }

        public override void Execute(object? parameter)
        {
            if (_element == null) return;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = @"Képfájlok (*.png,*.jpg,*.jpeg,*.bmp,*.gif,*.tiff,*.exif)|" +
                                        @"*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tiff;*.exif|" +
                                    @"Minden fájl (*.*)|" +
                                        @"*.*";
            openFileDialog.Title = "Kép megnyitása";
            if (openFileDialog.ShowDialog() ?? false)
            {
                if (!File.Exists(openFileDialog.FileName)) return;

                if (double.IsNaN(_element.Width) || double.IsNaN(_element.Height))
                {
                    double width = _element.ActualWidth;
                    double height = _element.ActualHeight;
                    _element.Width = width;
                    _element.Height = height;
                }

                _element.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }
    }
}
