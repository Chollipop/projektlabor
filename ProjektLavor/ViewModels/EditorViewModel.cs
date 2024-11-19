using Microsoft.Extensions.DependencyInjection;
using ProjektLavor.Commands;
using ProjektLavor.Components;
using ProjektLavor.Services;
using ProjektLavor.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ProjektLavor.ViewModels
{
    public class EditorViewModel : ViewModelBase
    {
        private ProjectStore _projectStore;
        private SelectedElementStore _selectedElementStore;
        private PropertiesPanelViewModel _propertiesPanelViewModel;
        private ModalNavigationStore _modalNavigationStore;
        private bool _hasMoved;

        private FixedPage _activePage;

        private void OnActivePageChanged()
        {
            if (_activePage != null)
            {
                _activePage.Effect = null;
            }

            _activePage = _projectStore.CurrentProject.ActivePage;
            if (_activePage != null)
            {
                _activePage.Effect = new System.Windows.Media.Effects.DropShadowEffect()
                {
                    Color = Colors.Red,
                    Direction = 0,
                    ShadowDepth = 0,
                    Opacity = 1,
                    BlurRadius = 100,
                };
                _activePage.Effect.BeginAnimation(System.Windows.Media.Effects.DropShadowEffect.OpacityProperty, new System.Windows.Media.Animation.DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.75)))
                {
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever
                });
            }
        }

        public PropertiesPanelViewModel PropertiesPanelViewModel
        {
            get => _propertiesPanelViewModel;
            set
            {
                _propertiesPanelViewModel?.Dispose();
                _propertiesPanelViewModel = value;
                OnPropertyChanged(nameof(PropertiesPanelViewModel));
            }
        }

        public FixedDocument CurrentDocument => _projectStore.CurrentProject?.Document;
        public bool HasSelectedItem => _selectedElementStore?.SelectedElement != null;

        public ICommand ApplyPropertiesCommand { get; set; }
        public ICommand ScrollChangedCommand { get; set; }

        public int DocumentZoom { get; set; } = 80;

        private bool IsDragging = false;
        private Point PointerOffsetInElement;

        public EditorViewModel(IServiceProvider serviceProvider)
        {
            _projectStore = serviceProvider.GetRequiredService<ProjectStore>();
            _selectedElementStore = serviceProvider.GetRequiredService<SelectedElementStore>();
            _modalNavigationStore = serviceProvider.GetRequiredService<ModalNavigationStore>();

            _projectStore.NewPageAdded += _projectStore_NewPageAdded;
            _projectStore.CurrentProjectChanged += _projectStore_CurrentProjectChanged;
            _selectedElementStore.PreviewSelectedElementChanged += _selectedElementStore_PreviewSelectedElementChanged;
            _selectedElementStore.SelectedElementChanged += _selectedElementStore_SelectedElementChanged;

            ApplyPropertiesCommand = new ApplyPropertiesCommand(_selectedElementStore);
            ScrollChangedCommand = new ScrollChangedCommand(_projectStore);
            if (_projectStore.CurrentProject != null)
            {
                _projectStore.CurrentProject.ActivePageChanged += OnActivePageChanged;
            }
        }

        private void _selectedElementStore_PreviewSelectedElementChanged(object sender, PreviewSelectedElementChangedEventArgs e)
        {
            if (e.LastValue == null) return;
            DetachResizeAdorner(e.LastValue);
            if (e.LastValue.GetType() == typeof(TextBlock))
                e.LastValue.MouseDown -= TextBlock_MouseDown;
        }

        private void _selectedElementStore_SelectedElementChanged()
        {
            OnPropertyChanged(nameof(HasSelectedItem));
            if (!HasSelectedItem) return;

            PropertiesPanelViewModel = new PropertiesPanelViewModel(_selectedElementStore.SelectedElement, _projectStore);

            if (_selectedElementStore.SelectedElement.GetType() == typeof(TextBlock))
            {
                TextBlock textBlock = _selectedElementStore.SelectedElement as TextBlock;
                textBlock.MouseDown += TextBlock_MouseDown;
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2) return;
            _projectStore.SaveState();

            e.Handled = true;
            IsDragging = false;

            new NavigateCommand(
                new ModalNavigationService<TextElementInputViewModel>(
                    _modalNavigationStore,
                    () => _projectStore.CurrentProject == null ? null : new TextElementInputViewModel(
                        _projectStore,
                        new CloseModalNavigationService(_modalNavigationStore),
                        sender as TextBlock
                    )
            )).Execute(null);
        }

        #region ELEMENT MOVING
        private void FixedPage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right &&
                (e.OriginalSource.GetType() == typeof(Image) || e.OriginalSource.GetType() == typeof(TextBlock))) return;

            e.Handled = true;
            if (e.OriginalSource.GetType() == typeof(FixedPage))
            {
                _projectStore.CurrentProject.SetActivePage(sender as FixedPage);
                _selectedElementStore.Select(null);
                return;
            }

            FixedPage fixedPage = (FixedPage)sender;
            FrameworkElement element = (FrameworkElement)e.OriginalSource;
            PointerOffsetInElement = e.GetPosition(element);
            IsDragging = true;
            _selectedElementStore.Select(element);

            _hasMoved = false;
            ReattachResizeAdorner(element);
        }

        private void FixedPage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right &&
                (e.OriginalSource.GetType() == typeof(Image) || e.OriginalSource.GetType() == typeof(TextBlock))) return;

            e.Handled = true;
            IsDragging = false;
        }

        private void FixedPage_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            if (!IsDragging || _selectedElementStore.SelectedElement == null) return;
            
            if(!_hasMoved)
            {
                _hasMoved = true;
                _projectStore.SaveState();
            }

            FixedPage fixedPage = (FixedPage)sender;
            foreach (PageContent pageContent in _projectStore.CurrentProject.Document.Pages)
            {
                FixedPage fixedPageElement = pageContent.Child;
                Point posToElement = Mouse.GetPosition(pageContent.Child);
                if (posToElement.X > 0 && posToElement.X < fixedPageElement.Width &&
                    posToElement.Y > 0 && posToElement.Y < fixedPageElement.Height)
                {
                    fixedPage = fixedPageElement;
                    break;
                }
            }

            FrameworkElement element = _selectedElementStore.SelectedElement;
            FixedPage? parentPage = element.Parent as FixedPage;
            if (element.Parent is AdornerDecorator)
            {
                parentPage = (FixedPage)((AdornerDecorator)element.Parent).Parent;
                element = (FrameworkElement)element.Parent;
            }

            if (fixedPage != parentPage)
            {
                DetachResizeAdorner(_selectedElementStore.SelectedElement);
                if (parentPage != null)
                {
                    parentPage.Children.Remove(element);
                }
                fixedPage.Children.Add(element);
                ReattachResizeAdorner(_selectedElementStore.SelectedElement);
            }
            Point cPos = Mouse.GetPosition(fixedPage);

            double x = cPos.X - PointerOffsetInElement.X;
            double y = cPos.Y - PointerOffsetInElement.Y;

            TransformGroup? transformGroup = _selectedElementStore.SelectedElement.RenderTransform as TransformGroup;
            ScaleTransform? scaleTransform = null;
            RotateTransform? rotateTransform = null;
            foreach (var transform in transformGroup?.Children ?? [])
            {
                if (transform is ScaleTransform st)
                    scaleTransform = st;
                else if (transform is RotateTransform rt)
                    rotateTransform = rt;
            }
            if (scaleTransform != null)
            {
                if (scaleTransform.ScaleX < 0)
                {
                    double width = double.IsNaN(_selectedElementStore.SelectedElement.Width) ? _selectedElementStore.SelectedElement.ActualWidth : _selectedElementStore.SelectedElement.Width;
                    x = cPos.X + PointerOffsetInElement.X - Math.Abs(scaleTransform.ScaleX) * width;
                }
                if (scaleTransform.ScaleY < 0)
                {
                    double height = double.IsNaN(_selectedElementStore.SelectedElement.Height) ? _selectedElementStore.SelectedElement.ActualHeight : _selectedElementStore.SelectedElement.Height;
                    y = cPos.Y + PointerOffsetInElement.Y - Math.Abs(scaleTransform.ScaleY) * height;
                }
            }

            FixedPage.SetLeft(element, x);
            FixedPage.SetTop(element, y);
        }
        #endregion

        private void DetachResizeAdorner(FrameworkElement element)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(element);
            if (adornerLayer == null) return;

            UIElement[] adorners = adornerLayer.GetAdorners(element);

            if (adorners == null) return;

            foreach (Adorner adorner in adorners)
            {
                if (adorner is ResizeAdorner)
                    adornerLayer.Remove(adorner);
            }
        }

        private void ReattachResizeAdorner(FrameworkElement element)
        {
            DetachResizeAdorner(element);
            AdornerLayer.GetAdornerLayer(element).Add(new ResizeAdorner(_projectStore, element));
        }

        private void _projectStore_CurrentProjectChanged()
        {
            OnPropertyChanged(nameof(CurrentDocument));
            if (CurrentDocument == null) return;

            foreach (PageContent pageContent in CurrentDocument.Pages)
            {
                pageContent.Child.MouseDown += FixedPage_MouseDown;
                pageContent.Child.MouseUp += FixedPage_MouseUp;
                pageContent.Child.MouseMove += FixedPage_MouseMove;
            }

            if (_projectStore.CurrentProject != null)
            {
                _projectStore.CurrentProject.ActivePageChanged += OnActivePageChanged;
            }

            Task.Run(() =>
            {
                Thread.Sleep(1000);
                Application.Current.Dispatcher.Invoke(() => _projectStore.AddAdorners());
            });
        }

        private void _projectStore_NewPageAdded(PageContent pageContent)
        {
            pageContent.Child.MouseDown += FixedPage_MouseDown;
            pageContent.Child.MouseUp += FixedPage_MouseUp;
            pageContent.Child.MouseMove += FixedPage_MouseMove;
        }

        public override void Dispose()
        {
            foreach (PageContent pageContent in CurrentDocument.Pages)
            {
                pageContent.Child.MouseDown -= FixedPage_MouseDown;
                pageContent.Child.MouseUp -= FixedPage_MouseUp;
                pageContent.Child.MouseMove -= FixedPage_MouseMove;
            }
            _projectStore.NewPageAdded -= _projectStore_NewPageAdded;
            _projectStore.CurrentProjectChanged -= _projectStore_CurrentProjectChanged;
            _selectedElementStore.PreviewSelectedElementChanged -= _selectedElementStore_PreviewSelectedElementChanged;
            _selectedElementStore.SelectedElementChanged -= _selectedElementStore_SelectedElementChanged;
        }
    }
}
