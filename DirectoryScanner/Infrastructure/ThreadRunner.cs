using DirectoryScanner.Models;
using DirectoryScanner.ViewModels;
using DirectoryScanner.Views;
using System;
using System.Linq;
using System.Threading;
using System.Windows;

namespace DirectoryScanner.Infrastructure
{
    internal class ThreadRunner
    {
        private Thread searchThread;
        private Thread updateUIThread;
        private Thread updateXMLThread;
        private int threadsFinished;
        private DirScanner dirScanner;
        private UIUpdater uiUpdater;
        private XMLUpdater xmlUpdater;
        public MainWindowViewModel viewModel;
        public ThreadRunner()
        {
            EntriesModel entriesModel = new EntriesModel();
            ErrorsModel errorsModel = new ErrorsModel();
            viewModel = new MainWindowViewModel(this);
            dirScanner = new DirScanner(entriesModel, errorsModel);
            xmlUpdater = new XMLUpdater(entriesModel, errorsModel);
            dirScanner.OnLoadFinished += viewModel.OnLoadFinished;
            xmlUpdater.OnLoadFinished += viewModel.OnLoadFinished;
            searchThread = new Thread(new ParameterizedThreadStart((p) => dirScanner.DirSearch(p)));
            updateXMLThread = new Thread(new ParameterizedThreadStart((p) => xmlUpdater.Write(p))); 
            uiUpdater = new UIUpdater( entriesModel, errorsModel, viewModel);
            uiUpdater.OnLoadFinished += viewModel.OnLoadFinished;
            updateUIThread = new Thread(uiUpdater.UpdateUIDelegate);
            GlobalExceptionHandler.exceptions.CollectionChanged += GlobalExceptionOccured;
        }

        public void InitializeAllThreads()
        {
            searchThread = new Thread(new ParameterizedThreadStart((p) => dirScanner.DirSearch(p)));
            updateUIThread = new Thread(uiUpdater.UpdateUIDelegate);
            updateXMLThread = new Thread(new ParameterizedThreadStart((p) => xmlUpdater.Write(p)));
        }
        private void TryAbortThread(Thread thread)
        {
            try
            {
                thread.Abort();
            }
            catch (ThreadAbortException)
            {

            }
        }
        public void TryAbortAllThreads()
        {
            TryAbortThread(searchThread);
            TryAbortThread(updateUIThread);
            TryAbortThread(updateXMLThread);
        }
        public void ResetAllObjectStates()
        {
            dirScanner.Reset();
            uiUpdater.Reset();
            xmlUpdater.Reset();
        }

        public void OnLoadFinished(object sender, EventArgs e)
        {
            threadsFinished++;
            if (threadsFinished == 3)
            {
                if (MessageBox.Show("All tasks completed!", "Success", MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK)
                {
                    InitializeAllThreads();
                    threadsFinished = 0;
                    ResetAllObjectStates();
                }
            }
        }
        public void OnClose(object sender, EventArgs e)
        {
            GlobalExceptionHandler.exceptions.CollectionChanged -= GlobalExceptionOccured;
            try {
                TryAbortAllThreads();
               MessageBox.Show(" Closed all threads!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + " Could not close all threads!", "Error", MessageBoxButton.OK, MessageBoxImage.Error); 
            }
        }
        public void OnStart(object sender, PathsModel e)
        {
            searchThread.Start(e.InputDirectoryPath);
            updateUIThread.Start();
            updateXMLThread.Start(e.OutputDirectoryPath);
        }

        private void GlobalExceptionOccured(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Exception currentException = e.NewItems.Cast<Exception>().First();
            viewModel.InputDirectoryButtonEnabled = false; viewModel.OutputFileButtonEnabled = false;
            if (MessageBox.Show("Exception in " + currentException.Data["ComponentName"] + ". " + e.NewItems.Cast<Exception>().First().Message, "Error", MessageBoxButton.OK) == MessageBoxResult.OK)
            {
                try
                {
                    TryAbortAllThreads();
                    InitializeAllThreads();
                    Application.Current.Dispatcher.Invoke(() => viewModel.Tree.Clear());
                    Application.Current.Dispatcher.Invoke(() => viewModel.ErrorsListItems.Clear());
                    ResetAllObjectStates();
                    viewModel.InputDirectoryButtonEnabled = true;
                    viewModel.OutputFileButtonEnabled = true; 
                }
                catch (Exception ex)
                {
                    if (MessageBox.Show(ex.Message + " Could not close all threads!", "Error", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                    {
                        (sender as MainWindow).Close();
                    }
                }
            }
        }
    }
}
