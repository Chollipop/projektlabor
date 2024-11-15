using ProjektLavor.Commands;
using ProjektLavor.Stores;
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

namespace ProjektLavor.ViewModels
{
    public class PropertiesPanelViewModel : ViewModelBase
    {
        private ProjectStore _projectStore;
        private FrameworkElement _element;

        private bool isFontChangeAvailable;
        public bool IsFontChangeAvailable => _element.GetType() == typeof(TextBlock);

        private DependencyPropertyDescriptor WidthDependencyPropertyDescriptor;
        private DependencyPropertyDescriptor PositionDependencyPropertyDescriptor;

        private double _elementX;
        private double _elementY;

        public double ElementX
        {
            get
            {
                double left = FixedPage.GetLeft((UIElement)(_element is Image ? _element.Parent : _element));
                if (double.IsNaN(left))
                {
                    return Math.Ceiling(_elementX);
                }
                return Math.Ceiling(left);
            }
            set
            {
                if (_elementX != value)
                {
                    if (value > 797) value = 797;
                    if (value < 0) value = 0;
                    _projectStore.SaveState();
                    _elementX = value;
                    FixedPage.SetLeft((UIElement)(_element is Image ? _element.Parent : _element), _elementX);
                    OnPropertyChanged(nameof(ElementX));
                }
            }
        }

        public double ElementY
        {
            get
            {
                double top = FixedPage.GetTop((UIElement)(_element is Image ? _element.Parent : _element));
                if (double.IsNaN(top))
                {
                    return Math.Ceiling(_elementX);
                }
                return Math.Ceiling(top);
            }
            set
            {
                if (_elementY != value)
                {
                    if (value > 1124) value = 1124;
                    if (value < 0) value = 0;
                    _projectStore.SaveState();
                    _elementY = value;
                    FixedPage.SetTop((UIElement)(_element is Image ? _element.Parent : _element), _elementY);
                    OnPropertyChanged(nameof(ElementY));
                }
            }
        }

        public double RotationDegree
        {
            get
            {
                TransformGroup transformGroup = _element.RenderTransform as TransformGroup;
                return transformGroup?.Children.OfType<RotateTransform>().FirstOrDefault()?.Angle ?? 0;
            }
            set
            {
                _projectStore.SaveState();

                if (value > 360)
                {
                    value = 360;
                }

                if (value < -360)
                {
                    value = -360;
                }

                var transformGroup = _element.RenderTransform as TransformGroup;
                if (transformGroup == null)
                {
                    transformGroup = new TransformGroup();
                    _element.RenderTransform = transformGroup;
                }
                var rotateTransform = transformGroup.Children.OfType<RotateTransform>().FirstOrDefault();
                if (rotateTransform == null)
                {
                    rotateTransform = new RotateTransform();
                    transformGroup.Children.Add(rotateTransform);
                }
                rotateTransform.CenterX = _element.ActualWidth / 2;
                rotateTransform.CenterY = _element.ActualHeight / 2;
                rotateTransform.Angle = value;
                OnPropertyChanged(nameof(RotationDegree));
            }
        }

        public ICommand VerticalMirrorCommand { get; set; }
        public ICommand HorizontalMirrorCommand { get; set; }
        public ICommand MoveElementForwardCommand { get; set; }
        public ICommand MoveElementBackwardCommand { get; set; }

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

        private bool keepAspectRatio = false;
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
                _projectStore.SaveState();

                if (value < 1)
                {
                    value = 1;
                }

                if (!KeepAspectRatio)
                {
                    if (double.IsNaN(_element.Height))
                    {
                        _element.Height = _element.ActualHeight;
                    }
                    resizeWidth = Math.Ceiling(value);
                    _element.Width = resizeWidth;
                    OnPropertyChanged(nameof(ResizeWidth));
                }
                else
                {
                    double aspectRatio = ResizeWidth / ResizeHeight;

                    resizeWidth = Math.Ceiling(value);
                    _element.Width = resizeWidth;

                    resizeHeight = Math.Ceiling(resizeWidth / aspectRatio);
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
                _projectStore.SaveState();

                if (value < 1)
                {
                    value = 1;
                }

                if (!KeepAspectRatio)
                {
                    if (double.IsNaN(_element.Width))
                    {
                        _element.Width = _element.ActualWidth;
                    }
                    resizeHeight = Math.Ceiling(value);
                    _element.Height = resizeHeight;
                    OnPropertyChanged(nameof(ResizeHeight));
                }
                else
                {
                    double aspectRatio = ResizeWidth / ResizeHeight;

                    resizeHeight = Math.Ceiling(value);
                    _element.Height = resizeHeight;

                    resizeWidth = Math.Ceiling(resizeHeight * aspectRatio);
                    _element.Width = resizeWidth;

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
                _projectStore.SaveState();

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
                _projectStore.SaveState();

                selectedFont = value;
                _element.SetValue(TextElement.FontFamilyProperty, new FontFamily(selectedFont));
                OnPropertyChanged(nameof(SelectedFont));
            }
        
        }
        public int SelectedFontSize { get; set; }
        public ICommand ToggleBoldCommand { get; set; }
        public ICommand ToggleItalicCommand { get; set; }
        public ICommand ToggleUnderlineCommand { get; set; }

        public PropertiesPanelViewModel(FrameworkElement element, ProjectStore projectStore)
        {
            _projectStore = projectStore;
            _element = element;
            WidthDependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(FrameworkElement.WidthProperty, typeof(FrameworkElement));
            WidthDependencyPropertyDescriptor.AddValueChanged(_element, UpdateValues);

            PositionDependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(FixedPage.LeftProperty, typeof(FrameworkElement));
            PositionDependencyPropertyDescriptor.AddValueChanged((_element is Image ? _element.Parent : _element), UpdateValues);

            VerticalMirrorCommand = new VerticalMirrorCommand(_projectStore, _element);
            HorizontalMirrorCommand = new HorizontalMirrorCommand(_element, _projectStore);
            MoveElementForwardCommand = new MoveElementForwardCommand(_element, _projectStore);
            MoveElementBackwardCommand = new MoveElementBackwardCommand(_element, _projectStore);

            if (_element.GetType() == typeof(TextBlock))
            {
                ToggleBoldCommand = new ToggleBoldCommand((TextBlock)_element, _projectStore);
                ToggleItalicCommand = new ToggleItalicCommand((TextBlock)_element, _projectStore);
                ToggleUnderlineCommand = new ToggleUnderlineCommand((TextBlock)_element, _projectStore);
            }

            AvailableFonts = Fonts.SystemFontFamilies.OrderBy(f => f.Source).Select(f => f.Source).ToList();
            OnPropertyChanged(nameof(AvailableFonts));

            UpdateValues(null, null);
        }

        private void UpdateValues(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(ResizeWidth));
            OnPropertyChanged(nameof(ResizeHeight));
            OnPropertyChanged(nameof(ElementX));
            OnPropertyChanged(nameof(ElementY));
        }
        public override void Dispose()
        {
            WidthDependencyPropertyDescriptor.RemoveValueChanged(_element, UpdateValues);
            base.Dispose();
        }
    }
}
