using ProjektLavor.ViewModels;

namespace ProjektLavor.Services
{
    public interface INavigationService<TViewModel>
        where TViewModel : ViewModelBase
    {
        void Navigate();
    }
    //public interface INavigationService
    //{
    //    void Navigate();
    //}
}