using ProjektLavor.Stores;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ProjektLavor.Commands
{
    public class PrintProjectCommand : CommandBase
    {
        private readonly ProjectStore _projectStore;

        public PrintProjectCommand(ProjectStore projectStore)
        {
            _projectStore = projectStore;
        }

        public override void Execute(object? parameter)
        {
            if (_projectStore.CurrentProject == null)
                return;

            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                try
                {
                    FixedDocument document = _projectStore.CurrentProject.Document;

                    foreach (var page in document.Pages)
                    {
                        FixedPage fixedPage = page.Child as FixedPage;
                        if (fixedPage != null)
                        {
                            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(fixedPage);
                            if (adornerLayer != null)
                            {
                                Visual combinedVisual = CreateCombinedVisual(fixedPage, adornerLayer);
                                printDialog.PrintVisual(combinedVisual, "Document with Adorners");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }


        public Visual CreateCombinedVisual(FixedPage fixedPage, AdornerLayer adornerLayer)
        {
            DrawingVisual combinedVisual = new DrawingVisual();

            using (DrawingContext dc = combinedVisual.RenderOpen())
            {
                fixedPage.Measure(new Size(fixedPage.Width, fixedPage.Height));
                fixedPage.Arrange(new Rect(new Size(fixedPage.Width, fixedPage.Height)));
                fixedPage.UpdateLayout();
                dc.DrawRectangle(new VisualBrush(fixedPage), null, new Rect(0, 0, fixedPage.Width, fixedPage.Height));

                Adorner[] adorners = adornerLayer.GetAdorners(fixedPage);
                if (adorners != null)
                {
                    foreach (Adorner adorner in adorners)
                    {
                        adorner.InvalidateVisual();
                        adorner.Measure(new Size(fixedPage.Width, fixedPage.Height));
                        adorner.Arrange(new Rect(new Size(fixedPage.Width, fixedPage.Height)));
                        adorner.UpdateLayout();
                        dc.DrawRectangle(new VisualBrush(adorner), null, new Rect(0, 0, fixedPage.Width, fixedPage.Height));
                    }
                }
                foreach (FrameworkElement element in fixedPage.Children)
                {
                    adorners = AdornerLayer.GetAdornerLayer(element).GetAdorners(element);
                    foreach (Adorner adorner in adorners ?? [])
                    {
                        if (!(adorner is FrameAdorner)) continue;

                        adorner.InvalidateVisual();
                        adorner.Measure(element.RenderSize);
                        adorner.Arrange(new Rect(element.RenderSize));
                        adorner.UpdateLayout();

                        double x = FixedPage.GetLeft(element);
                        double y = FixedPage.GetTop(element);
                        double width = element.RenderSize.Width;
                        double height = element.RenderSize.Height;
                        double frameOverhangX = width * .1;
                        double frameOverhangY = height * .1;
                        dc.DrawRectangle(new VisualBrush(adorner), null, new Rect(x - frameOverhangX, y - frameOverhangY, width + (2 * frameOverhangX), height + (2 * frameOverhangY)));
                    }
                }
            }

            return combinedVisual;
        }
    }
}
