using ProjektLavor.ViewModels;
using ProjektLavor.Commands;
using ProjektLavor.Services;
using System.Windows.Input;
using System.Windows.Navigation;

namespace ProjektLavor.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        public string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand NavigateRegisterCommand { get; }
        public LoginViewModel(/*AccountStore accountStore, INavigationService<CarsViewModel> carsNavigationService, INavigationService<AdminCategoriesViewModel> adminCategoriesNavigationService, INavigationService<RegisterViewModel> registerNavigationService*/)
        {
            //LoginCommand = new LoginCommand(this, accountStore, carsNavigationService, adminCategoriesNavigationService);
            //NavigateRegisterCommand = new NavigateCommand<RegisterViewModel>(registerNavigationService);
        }
    }
}
