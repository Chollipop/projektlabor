using Microsoft.Win32;
using ProjektLavor.Stores;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFCustomMessageBox;
using Xceed.Wpf.Toolkit;

namespace ProjektLavor.Commands
{
    public class ChangeImageSourceCommand : CommandBase
    {
        public override void Execute(object? parameter)
        {
            if (parameter is not Tuple<Image, ProjectStore> tuple) return;

            var (image, projectStore) = tuple;

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

                BitmapImage newImage = new BitmapImage(new Uri(openFileDialog.FileName));

                MessageBoxResult result = CustomMessageBox.ShowYesNoCancel(
                    "Válassza ki a módosítás típusát:",
                    "Kép módosítása",
                    "Eredeti méret megtartása",
                    "Kitöltés nagyítással",
                    "Fekete sávok");

                string changeMethod = result switch
                {
                    MessageBoxResult.Yes => "keep",
                    MessageBoxResult.No => "zoom",
                    MessageBoxResult.Cancel => "letterbox",
                    _ => null
                };

                if (changeMethod == null) return;

                projectStore.SaveState();

                if (changeMethod == "keep")
                {
                    if (double.IsNaN(image.Width) || double.IsNaN(image.Height))
                    {
                        double width = image.ActualWidth;
                        double height = image.ActualHeight;
                        image.Width = width;
                        image.Height = height;
                    }
                    image.Source = newImage;
                }
                if (changeMethod == "zoom")
                {
                    double containerWidth = image.ActualWidth;
                    double containerHeight = image.ActualHeight;
                    double containerAspectRatio = containerWidth / containerHeight;

                    double imageAspectRatio = newImage.Width / newImage.Height;

                    double scale;
                    if (imageAspectRatio > containerAspectRatio)
                    {
                        scale = containerHeight / newImage.Height;
                    }
                    else
                    {
                        scale = containerWidth / newImage.Width;
                    }

                    double scaledWidth = newImage.Width * scale;
                    double scaledHeight = newImage.Height * scale;

                    DrawingVisual drawingVisual = new DrawingVisual();
                    using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                    {
                        drawingContext.DrawImage(newImage, new Rect(
                            (containerWidth - scaledWidth) / 2,
                            (containerHeight - scaledHeight) / 2,
                            scaledWidth,
                            scaledHeight));
                    }

                    RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)containerWidth, (int)containerHeight, 96, 96, PixelFormats.Pbgra32);
                    renderTargetBitmap.Render(drawingVisual);

                    image.Source = renderTargetBitmap;
                }
                if (changeMethod == "letterbox")
                {
                    double containerWidth = image.ActualWidth;
                    double containerHeight = image.ActualHeight;
                    double containerAspectRatio = containerWidth / containerHeight;

                    double imageAspectRatio = newImage.Width / newImage.Height;

                    DrawingVisual drawingVisual = new DrawingVisual();
                    using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                    {
                        if (imageAspectRatio > containerAspectRatio)
                        {
                            double scaledHeight = containerWidth / imageAspectRatio;
                            double yOffset = (containerHeight - scaledHeight) / 2;

                            drawingContext.DrawRectangle(Brushes.Black, null, new Rect(0, 0, containerWidth, yOffset));
                            drawingContext.DrawImage(newImage, new Rect(0, yOffset, containerWidth, scaledHeight));
                            drawingContext.DrawRectangle(Brushes.Black, null, new Rect(0, yOffset + scaledHeight, containerWidth, yOffset));
                        }
                        else
                        {
                            double scaledWidth = containerHeight * imageAspectRatio;
                            double xOffset = (containerWidth - scaledWidth) / 2;

                            drawingContext.DrawRectangle(Brushes.Black, null, new Rect(0, 0, xOffset, containerHeight));
                            drawingContext.DrawImage(newImage, new Rect(xOffset, 0, scaledWidth, containerHeight));
                            drawingContext.DrawRectangle(Brushes.Black, null, new Rect(xOffset + scaledWidth, 0, xOffset, containerHeight));
                        }
                    }

                    RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)containerWidth, (int)containerHeight, 96, 96, PixelFormats.Pbgra32);
                    renderTargetBitmap.Render(drawingVisual);

                    image.Source = renderTargetBitmap;
                }
            }
        }
    }
}
