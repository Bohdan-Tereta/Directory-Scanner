using DirectoryScanner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DirectoryScanner.Commands
{
    internal class SelectInputDirectoryCommand : BaseMainWindowCommand
    {
        public SelectInputDirectoryCommand(MainWindowViewModel viewModel) 
            :base(viewModel)
        {

        } 

        public override bool CanExecute(object parameter)
        {
            return true; 
        }

        public override void Execute(object parameter)
        {
            if (viewModel != null)
            {
                FolderBrowserDialog folderDialog = new FolderBrowserDialog();
                DialogResult result = folderDialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    viewModel.InputDirectoryPath = folderDialog.SelectedPath;
                    viewModel.OutputFileButtonEnabled = true;
                    viewModel.OutputFilePath = "";
                    viewModel.Tree.Clear();
                    viewModel.ErrorsListItems.Clear(); 
                }
            }
        }
    }
}
