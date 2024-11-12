using System;
using System.IO;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using ProjektLavor.Stores;
using System.Printing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Xps;

namespace ProjektLavor.Commands
{
    public class ExportProjectCommand : CommandBase
    {
        private readonly ProjectStore _projectStore;

        public ExportProjectCommand(ProjectStore projectStore)
        {
            _projectStore = projectStore;
        }

        public override void Execute(object? parameter)
        {
            if (_projectStore.CurrentProject == null)
                return;

            string xpsFilePath = "exported_project.xps"; // Temporary XPS file path
            string pdfFilePath = "exported_project.pdf"; // Desired PDF file path

            try
            {
                ExportToXps(_projectStore.CurrentProject.Document, xpsFilePath);
                ConvertXpsToPdf(xpsFilePath, pdfFilePath);
                // Notify user of success, e.g., via a message box or logging
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., log the error or notify the user
            }
            finally
            {
                if (File.Exists(xpsFilePath))
                {
                    File.Delete(xpsFilePath); // Clean up temporary XPS file
                }
            }
        }

        private void ExportToXps(FixedDocument document, string xpsFilePath)
        {
            using (XpsDocument xpsDocument = new XpsDocument(xpsFilePath, FileAccess.Write))
            {
                XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
                writer.Write(document);
            }
        }

        private void ConvertXpsToPdf(string xpsFilePath, string pdfFilePath)
        {
            using (XpsDocument xpsDocument = new XpsDocument(xpsFilePath, FileAccess.Read))
            {
                FixedDocumentSequence fixedDocSeq = xpsDocument.GetFixedDocumentSequence();
                using (var pdfStream = new FileStream(pdfFilePath, FileMode.Create))
                {
                    var printDialog = new PrintDialog();
                    var printQueue = new PrintQueue(new PrintServer(), "Microsoft Print to PDF");

                    printDialog.PrintQueue = printQueue;
                    printDialog.PrintDocument(fixedDocSeq.DocumentPaginator, "Export to PDF");
                }
            }
        }
    }
}
