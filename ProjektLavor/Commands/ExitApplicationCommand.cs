using System.Windows;

namespace ProjektLavor.Commands
{
    public class ExitApplicationCommand : CommandBase
    {
        public override void Execute(object? parameter)
        {
            Application.Current.Shutdown();
        }
    }
}
