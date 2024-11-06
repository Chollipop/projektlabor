using ProjektLavor.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace ProjektLavor.ViewModels
{
    public class PropertiesPanelViewModel : ViewModelBase
    {
        private FrameworkElement _element;

        private DependencyPropertyDescriptor WidthDependencyPropertyDescriptor;

        private double rotationDegree;
        public double RotationDegree
        {
            get
            {
                RotateTransform rotation = _element.RenderTransform as RotateTransform;
                return rotation?.Angle??0;
            }
            set
            {
                rotationDegree = value;
                var rotateTransform = new RotateTransform(rotationDegree)
                {
                    CenterX = _element.ActualWidth / 2,
                    CenterY = _element.ActualHeight / 2
                };
                _element.RenderTransform = rotateTransform;
                OnPropertyChanged(nameof(RotationDegree));
            }
        }

        public ICommand VerticalMirrorCommand { get; set; }
        public ICommand HorizontalMirrorCommand { get; set; }

        private bool keepAspectRatio = true;
        public bool KeepAspectRatio
        {
            get => keepAspectRatio;
            set
            {
                if (keepAspectRatio != value)
                {
                    keepAspectRatio = value;
                    OnPropertyChanged(nameof(KeepAspectRatio));
                }
            }
        }

        //private double resizeWidthPercent = 100;
        //public double ResizeWidthPercent
        //{
        //    get
        //    {
        //        return resizeWidthPercent;
        //    }
        //    set
        //    {
        //        if (value > 500)
        //        {
        //            value = 500;
        //        }

        //        if (value < 1)
        //        {
        //            value = 1;
        //        }

        //        if (!KeepAspectRatio)
        //        {
        //            resizeWidthPercent = value;
        //            _element.Width = ResizeWidth * resizeWidthPercent / 100;
        //            OnPropertyChanged(nameof(ResizeWidthPercent));
        //        }
        //        else
        //        {
        //            resizeWidthPercent = value;
        //            _element.Width *= ResizeWidth * resizeWidthPercent / 100;

        //            resizeHeightPercent = value;
        //            _element.Height *= ResizeHeight * resizeHeightPercent / 100;

        //            OnPropertyChanged(nameof(ResizeWidthPercent));
        //            OnPropertyChanged(nameof(ResizeHeightPercent));
        //        }
        //    }
        //}

        //private double resizeHeightPercent = 100;
        //public double ResizeHeightPercent
        //{
        //    get
        //    {
        //        return resizeHeightPercent;
        //    }
        //    set
        //    {
        //        if (value > 500)
        //        {
        //            value = 500;
        //        }

        //        if (value < 1)
        //        {
        //            value = 1;
        //        }

        //        if (!KeepAspectRatio)
        //        {
        //            resizeHeightPercent = value;
        //            _element.Height = ResizeHeight * resizeHeightPercent / 100;
        //            OnPropertyChanged(nameof(ResizeHeightPercent));
        //        }
        //        else
        //        {
        //            resizeWidthPercent = value;
        //            _element.Width *= ResizeWidth * resizeWidthPercent / 100;

        //            resizeHeightPercent = value;
        //            _element.Height *= ResizeHeight * resizeHeightPercent / 100;

        //            OnPropertyChanged(nameof(ResizeWidthPercent));
        //            OnPropertyChanged(nameof(ResizeHeightPercent));
        //        }
        //    }
        //}

        private double resizeWidth;
        public double ResizeWidth
        {
            get
            {
                if (!double.IsNaN(_element.Width))
                {
                    return Math.Ceiling(_element.Width);
                }
                return Math.Ceiling(_element.ActualWidth);
            }
            set
            {
                if (!KeepAspectRatio)
                {
                    resizeWidth = Math.Ceiling(value);
                    _element.Width = resizeWidth;
                    OnPropertyChanged(nameof(ResizeWidth));
                }
                else
                {
                    resizeWidth = Math.Ceiling(value);
                    _element.Width = resizeWidth;
                    
                    resizeHeight = Math.Ceiling(value);
                    _element.Height = resizeHeight;

                    OnPropertyChanged(nameof(ResizeWidth));
                    OnPropertyChanged(nameof(ResizeHeight));
                }
            }
        }

        private double resizeHeight;

        public double ResizeHeight
        {
            get
            {
                if (!double.IsNaN(_element.Height))
                {
                    return Math.Ceiling(_element.Height);
                }
                return Math.Ceiling(_element.ActualHeight);
            }
            set
            {

                if (!KeepAspectRatio)
                {
                    resizeHeight = Math.Ceiling(value);
                    _element.Height = resizeHeight;
                    OnPropertyChanged(nameof(ResizeHeight));
                }
                else
                {
                    resizeWidth = Math.Ceiling(value);
                    _element.Width = resizeWidth;

                    resizeHeight = Math.Ceiling(value);
                    _element.Height = resizeHeight;

                    OnPropertyChanged(nameof(ResizeWidth));
                    OnPropertyChanged(nameof(ResizeHeight));
                }
            }
        }

        private int fontSize;

        public int FontSize
        {
            get
            {
                return _element.GetValue(TextElement.FontSizeProperty) is double fontSize ? (int)fontSize : 0;
            }
            set
            {
                fontSize = value;
                _element.SetValue(TextElement.FontSizeProperty, (double)fontSize);
                OnPropertyChanged(nameof(FontSize));
            }
        }


        public List<string> AvailableFonts { get; set; }

        private string selectedFont;
        public string SelectedFont
        {
            get
            {
                if (_element.GetType() == typeof(TextBlock))
                {
                    return _element.GetValue(TextElement.FontFamilyProperty).ToString();
                }
                return string.Empty;
            }
            set
            {
                selectedFont = value;
                _element.SetValue(TextElement.FontFamilyProperty, new FontFamily(selectedFont));
                OnPropertyChanged(nameof(SelectedFont));
            }
        
        }
        public int SelectedFontSize { get; set; }
        public ICommand ToggleBoldCommand { get; set; }
        public ICommand ToggleItalicCommand { get; set; }
        public ICommand ToggleUnderlineCommand { get; set; }

        public PropertiesPanelViewModel(FrameworkElement element)
        {
            _element = element;
            WidthDependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(FrameworkElement.WidthProperty, typeof(FrameworkElement));
            WidthDependencyPropertyDescriptor.AddValueChanged(_element, UpdateValues);
            VerticalMirrorCommand = new VerticalMirrorCommand(_element);
            HorizontalMirrorCommand = new HorizontalMirrorCommand(_element);

            if (_element.GetType() == typeof(TextBlock))
            {
                ToggleBoldCommand = new ToggleBoldCommand((TextBlock)_element);
                ToggleItalicCommand = new ToggleItalicCommand((TextBlock)_element);
                ToggleUnderlineCommand = new ToggleUnderlineCommand((TextBlock)_element);
            }

            AvailableFonts = Fonts.SystemFontFamilies.OrderBy(f => f.Source).Select(f => f.Source).ToList();
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
