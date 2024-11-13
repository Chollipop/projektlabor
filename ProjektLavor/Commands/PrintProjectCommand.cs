using ProjektLavor.Stores;
using System;
using System.Windows.Controls;
using System.Windows.Documents;

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
                    printDialog.PrintDocument(document.DocumentPaginator, "Print Project");
                    // Notify user of success, e.g., via a message box or logging
                }
                catch (Exception ex)
                {
                    // Handle exceptions, e.g., log the error or notify the user
                }
            }
        }
    }
}
