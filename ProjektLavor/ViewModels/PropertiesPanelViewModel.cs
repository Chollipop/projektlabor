using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ProjektLavor.ViewModels
{
    public class PropertiesPanelViewModel : ViewModelBase
    {
        private FrameworkElement _element;
        private double resizeWidth;
        private double resizeHeight;

        private bool isFontChangeAvailable;
        public bool IsFontChangeAvailable => _element.GetType() == typeof(TextBlock);

        private DependencyPropertyDescriptor WidthDependencyPropertyDescriptor;
        
        private double rotationDegree;
        public double RotationDegree
        {
            get { return rotationDegree; }
            set
            {
                rotationDegree = value;
            }
        }
        public ICommand VerticalMirrorCommand { get; set; }
        public ICommand HorizontalMirrorCommand { get; set; }

        public double ResizeWidthPercent { get; set; }
        public double ResizeHeightPercent { get; set; }
        public double ResizeWidth
        {
            get => resizeWidth;
            set
            {
                resizeWidth = value;
                OnPropertyChanged(nameof(ResizeWidth));
            }
        }
        public double ResizeHeight
        {
            get => resizeHeight;
            set
            {
                resizeHeight = value;
                OnPropertyChanged(nameof(ResizeHeight));
            }
        }

        public List<string> AvailableFonts { get; set; }
        public string SelectedFont { get; set; }
        public List<int> AvailableFontSizes { get; set; }
        public int SelectedFontSize { get; set; }
        public ICommand ToggleBoldCommand { get; set; }
        public ICommand ToggleItalicCommand { get; set; }
        public ICommand ToggleUnderlineCommand { get; set; }

        public PropertiesPanelViewModel(FrameworkElement element)
        {
            _element = element;
            WidthDependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(FrameworkElement.WidthProperty, typeof(FrameworkElement));
            WidthDependencyPropertyDescriptor.AddValueChanged(_element, UpdateValues);

            AvailableFonts = Fonts.SystemFontFamilies.Select(x => x.Source).ToList();
            OnPropertyChanged(nameof(AvailableFonts));

            UpdateValues(null, null);
        }

        private void UpdateValues(object? sender, EventArgs e)
        {
            ResizeWidth = _element.Width;
            ResizeHeight = _element.Height;
        }
        public override void Dispose()
        {
            WidthDependencyPropertyDescriptor.RemoveValueChanged(_element, UpdateValues);
            base.Dispose();
        }
    }
}
