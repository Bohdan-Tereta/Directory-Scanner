using DirectoryScanner.Models;
using DirectoryScanner.ViewModels;
using System.Windows.Forms;

namespace DirectoryScanner.Commands
{
    internal class SelectOutputFileCommand : BaseMainWindowCommand
    {
       public SelectOutputFileCommand(MainWindowViewModel viewModel) 
            : base(viewModel)
        { 
        } 

        public override bool CanExecute(object parameter)
        {
            return true; 
        }

        public override void Execute(object parameter)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "XML Files (*.xml) | *.xml";
            DialogResult result = fileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                viewModel.OutputFilePath = fileDialog.FileName;
                PathsModel pathsModel = new PathsModel(viewModel.InputDirectoryPath, viewModel.OutputFilePath);
                viewModel.InputDirectoryButtonEnabled = false;
                viewModel.OutputFileButtonEnabled = false;
                viewModel.Tree.Clear();
                viewModel.ErrorsListItems.Clear();
                viewModel.threadRunner.OnStart(viewModel, pathsModel);
                viewModel.OutputFileButtonEnabled = false;
            }
        }
    }
}
