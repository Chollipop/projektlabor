using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace ProjektLavor.Models
{
    public class Project : IDisposable
    {
        public FlowDocument Document { get; set; }

        public Project()
        {
            Document = new FlowDocument();
            Document.PageHeight = 1123.2; //A4 page height
            Document.PageWidth = 796.8; //A4 page width
            Document.Background = Brushes.White;
        }

        public void Dispose()
        {
        }
    }
}
