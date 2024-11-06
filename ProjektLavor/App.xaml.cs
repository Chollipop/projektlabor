using ProjektLavor.ViewModels;
using ProjektLavor.Stores;
using ProjektLavor.Services;
using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace ProjektLavor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<NavigationStore>();
            services.AddSingleton<ModalNavigationStore>();
            services.AddSingleton<ProjectStore>();
            services.AddSingleton<SelectedElementStore>();

            services.AddSingleton<INavigationService>(x => CreateEditorLayoutNavigationService(x));

            services.AddTransient<EditorLayoutViewModel>(x => new EditorLayoutViewModel(
                CreateToolbarViewModel(x),
                new EditorViewModel(x)
                ));
            services.AddSingleton<NavigationBarViewModel>(CreateNavigationBarViewModel);
            services.AddSingleton<MainViewModel>();

            services.AddSingleton<MainWindow>(x => new MainWindow()
            {
                DataContext = x.GetRequiredService<MainViewModel>(),
                WindowState = WindowState.Maximized
            });

            _serviceProvider = services.BuildServiceProvider();
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            INavigationService initialNavigationService = _serviceProvider.GetRequiredService<INavigationService>();
            //INavigationService initialNavigationService = CreateEditorLayoutNavigationService(_serviceProvider);
            initialNavigationService.Navigate();

            MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            MainWindow.Show();

            base.OnStartup(e);
        }

        private INavigationService CreateEditorLayoutNavigationService(IServiceProvider serviceProvider)
        {
            return new LayoutNavigationService<EditorLayoutViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<EditorLayoutViewModel>(),
                () => serviceProvider.GetRequiredService<NavigationBarViewModel>()
            );
        }
        private INavigationService CreateNewTextElementModalNavigationService(IServiceProvider serviceProvider)
        {
            return new ModalNavigationService<TextElementInputViewModel>(
                    serviceProvider.GetRequiredService<ModalNavigationStore>(),
                    () => new TextElementInputViewModel(
                        serviceProvider.GetRequiredService<ProjectStore>(),
                        new CloseModalNavigationService(serviceProvider.GetRequiredService<ModalNavigationStore>())
                    )
                );
        }

        private ToolbarViewModel CreateToolbarViewModel(IServiceProvider serviceProvider)
        {
            //return new ToolbarViewModel(CreateNewTextElementModalNavigationService(serviceProvider));
            return new ToolbarViewModel(serviceProvider);
        }
        private NavigationBarViewModel CreateNavigationBarViewModel(IServiceProvider serviceProvider)
        {
            return new NavigationBarViewModel(_serviceProvider);
        }
    }

}
