using DirectoryScanner.Infrastructure;
using DirectoryScanner.ViewModels;
using DirectoryScanner.Views;
using System.Windows;

namespace DirectoryScanner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ThreadRunner threadRunner;
        private MainWindowViewModel viewModel; 
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            threadRunner = new ThreadRunner();
            viewModel = threadRunner.viewModel; 
            MainWindow view = new MainWindow();
            view.DataContext = viewModel;
            view.OnClose += viewModel.Close;
            view.Show();
        } 
    }
}
