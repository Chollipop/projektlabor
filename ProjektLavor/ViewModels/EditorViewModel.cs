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

namespace ProjektLavor.ViewModels
{
    public class EditorViewModel : ViewModelBase
    {
        private ProjectStore _projectStore;
        private SelectedElementStore _selectedElementStore;
        private PropertiesPanelViewModel _propertiesPanelViewModel;
        private ModalNavigationStore _modalNavigationStore;
        private bool _hasMoved;

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
            if (fixedPage != _selectedElementStore.SelectedElement.Parent)
            {
                DetachResizeAdorner(_selectedElementStore.SelectedElement);
                FixedPage oldParent = _selectedElementStore.SelectedElement.Parent as FixedPage;
                if (oldParent != null)
                {
                    oldParent.Children.Remove(_selectedElementStore.SelectedElement);
                }
                fixedPage.Children.Add(_selectedElementStore.SelectedElement);
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

            FixedPage.SetLeft(_selectedElementStore.SelectedElement, x);
            FixedPage.SetTop(_selectedElementStore.SelectedElement, y);
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
