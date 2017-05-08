using DirectoryScanner.Commands;
using DirectoryScanner.Infrastructure;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace DirectoryScanner.ViewModels
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public ThreadRunner threadRunner { get; set; }
        public MainWindowViewModel(ThreadRunner threadRunner)
        {
            this.threadRunner = threadRunner; 
            ErrorsListItems = new ObservableCollection<string>();
            Tree = new ObservableCollection<TreeViewItem>();
            inputDirectoryButtonEnabled = true;
            SelectInputDirectoryCommand = new SelectInputDirectoryCommand(this);
            SelectOutputFileCommand = new SelectOutputFileCommand(this);
        }
        private bool inputDirectoryButtonEnabled;
        public bool InputDirectoryButtonEnabled { get { return inputDirectoryButtonEnabled; } set { inputDirectoryButtonEnabled = value; OnPropertyChanged("InputDirectoryButtonEnabled"); } } 

        private bool outputFileButtonEnabled;
        public bool OutputFileButtonEnabled { get { return outputFileButtonEnabled; } set { outputFileButtonEnabled = value; OnPropertyChanged("OutputFileButtonEnabled"); } } 

        private string inputDirectoryPath;
        public string InputDirectoryPath { get { return inputDirectoryPath; } set { inputDirectoryPath = value; OnPropertyChanged("InputDirectoryPath"); } } 

        private string outputFilePath;
        public string OutputFilePath { get { return outputFilePath; } set { outputFilePath = value; OnPropertyChanged("OutputFilePath"); } } 

        public ObservableCollection<TreeViewItem> Tree { get; set; }
        public ObservableCollection<string> ErrorsListItems { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        public ICommand SelectInputDirectoryCommand { get; set; }
        public ICommand SelectOutputFileCommand { get; set; }
        public void  Close(object sender, EventArgs e)
        {
             this.threadRunner.OnClose(this, null);
        }
        public void OnLoadFinished(object sender, EventArgs e)
        {
            this.threadRunner.OnLoadFinished(this, null);
            this.InputDirectoryButtonEnabled = true;
            this.OutputFileButtonEnabled = true;
        }
    }
}
