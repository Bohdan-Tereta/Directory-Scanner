using DirectoryScanner.ViewModels;
using System;
using System.Windows.Input;

namespace DirectoryScanner.Commands
{
    internal abstract class BaseMainWindowCommand: ICommand
    {
        protected MainWindowViewModel viewModel; 
        public BaseMainWindowCommand(MainWindowViewModel viewModel)
        {
            this.viewModel = viewModel; 
        }

        public event EventHandler CanExecuteChanged;

        public abstract bool CanExecute(object parameter);  

        public abstract void Execute(object parameter); 
    }
}
