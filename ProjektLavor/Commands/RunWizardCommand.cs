using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using ProjektLavor.Stores;

namespace ProjektLavor.Commands
{
    public class RunWizardCommand : CommandBase
    {
        private readonly ProjectStore _projectStore;

        public RunWizardCommand(ProjectStore projectStore)
        {
            _projectStore = projectStore;
        }

        public override void Execute(object? parameter)
        {
            if (_projectStore.CurrentProject?.Document == null) return;

            _projectStore.SaveState();

            List<Image> placeholders = new List<Image>();
            foreach (var page in _projectStore.CurrentProject.Document.Pages)
            {
                if (page.Child is FixedPage fixedPage)
                {
                    placeholders.AddRange(fixedPage.Children.OfType<AdornerDecorator>()
                        .Where(d => ((d.Child as Image)?.Source?.ToString() ?? string.Empty).Contains("placeholder"))
                        .Select(d => d.Child as Image));
                }
            }

            if (placeholders?.Count > 0)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Multiselect = true,
                    Filter = @"Képfájlok (*.png,*.jpg,*.jpeg,*.bmp,*.gif,*.tiff,*.exif)|" +
                                        @"*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tiff;*.exif|" +
                                    @"Minden fájl (*.*)|" +
                                        @"*.*",
                    Title = "Kép megnyitása"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    var selectedFiles = openFileDialog.FileNames.Take(placeholders.Count).ToList();

                    foreach (var file in selectedFiles)
                    {
                        var bitmapImage = new BitmapImage(new Uri(file));
                        double imageAspectRatio = (double)bitmapImage.PixelWidth / bitmapImage.PixelHeight;

                        Image bestFitPlaceholder = placeholders
                            .OrderBy(p => Math.Abs(((double)p.Width / p.Height) - imageAspectRatio))
                            .FirstOrDefault();

                        if (bestFitPlaceholder != null)
                        {
                            _projectStore.ApplyLetterboxImage(bestFitPlaceholder, bitmapImage);
                            placeholders.Remove(bestFitPlaceholder);
                        }
                    }
                }
            }

            foreach (var page in _projectStore.CurrentProject.Document.Pages)
            {
                if (page.Child is FixedPage fixedPage)
                {
                    var images = fixedPage.Children.OfType<AdornerDecorator>().Where(d => ((d.Child as Image)?.Tag?.ToString() ?? string.Empty) != "ignore_wizard").Select(d => d.Child as Image).ToList();
                    if (images.Count == 0)
                    {
                        fixedPage.Background = new SolidColorBrush(Colors.White);
                        SetTextColor(fixedPage, Colors.Black);
                        continue;
                    }

                    int totalPixels = 0;
                    long totalR = 0, totalG = 0, totalB = 0;

                    foreach (var image in images)
                    {
                        if (image.Source is BitmapSource bitmapSource)
                        {
                            int width = bitmapSource.PixelWidth;
                            int height = bitmapSource.PixelHeight;
                            int stride = width * ((bitmapSource.Format.BitsPerPixel + 7) / 8);
                            byte[] pixels = new byte[height * stride];
                            bitmapSource.CopyPixels(pixels, stride, 0);

                            for (int y = 0; y < height; y++)
                            {
                                for (int x = 0; x < width; x++)
                                {
                                    int index = y * stride + x * 4;

                                    // Ensure index is within bounds of the pixels array
                                    if (index + 3 >= pixels.Length)
                                        continue; // Skip this pixel if out of bounds

                                    byte b = pixels[index];
                                    byte g = pixels[index + 1];
                                    byte r = pixels[index + 2];
                                    byte a = pixels[index + 3]; // For images with an alpha channel

                                    totalR += r;
                                    totalG += g;
                                    totalB += b;
                                    totalPixels++;
                                }
                            }
                        }
                    }

                    if (totalPixels > 0)
                    {
                        byte avgR = (byte)(totalR / totalPixels);
                        byte avgG = (byte)(totalG / totalPixels);
                        byte avgB = (byte)(totalB / totalPixels);

                        Color backgroundColor = Color.FromRgb(avgR, avgG, avgB);
                        fixedPage.Background = new SolidColorBrush(backgroundColor);

                        Color complementaryColor = GetComplementaryColor(backgroundColor);
                        SetTextColor(fixedPage, complementaryColor);
                    }
                }
            }
        }

        private Color GetComplementaryColor(Color color)
        {
            byte compR = (byte)(255 - color.R);
            byte compG = (byte)(255 - color.G);
            byte compB = (byte)(255 - color.B);
            return Color.FromRgb(compR, compG, compB);
        }

        private void SetTextColor(FixedPage fixedPage, Color color)
        {
            foreach (var textBlock in fixedPage.Children.OfType<TextBlock>())
            {
                textBlock.Foreground = new SolidColorBrush(color);
            }
        }
    }
}
