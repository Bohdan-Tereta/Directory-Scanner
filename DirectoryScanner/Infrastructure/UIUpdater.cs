using DirectoryScanner.Models;
using DirectoryScanner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DirectoryScanner.Infrastructure
{
    internal class UIUpdater
    {
        private EntriesModel entriesModel; 
        private ErrorsModel errorsModel;
        private int startPosition = 0;
        private int endPosition = 0;
        private MainWindowViewModel viewModel; 
        private int errorsStartPosition = 0;
        private int errorsEndPosition = 0;
        private Dispatcher dispatcher; 
        public EventHandler OnLoadFinished = null; 
        public UIUpdater(EntriesModel entriesModel, ErrorsModel errorsModel, MainWindowViewModel viewModel)
        {
            this.dispatcher = Application.Current.Dispatcher; 
            this.entriesModel = entriesModel;
            this.errorsModel = errorsModel;
            this.viewModel = viewModel; 
        }
        public void Reset()
        {
            startPosition = 0;
            endPosition = 0;
            errorsStartPosition = 0;
            errorsEndPosition = 0;
        }
        public void UpdateUIDelegate()
        {
            try
            {
                startPosition = 0;
                endPosition = 0;
                errorsStartPosition = 0;
                errorsEndPosition = 0;
                while (!(entriesModel.IsLoadFinished && startPosition == entriesModel.EntriesFound.Count))
                {
                    UpdateUI();
                }
                if (OnLoadFinished != null)
                {
                    OnLoadFinished(this, null);
                }
            }
             catch (ThreadAbortException)
            {

            }
            catch (Exception ex)
            {
                ex.Data.Add("ComponentName", this.GetType());
                lock (GlobalExceptionHandler.exceptions)
                {
                    GlobalExceptionHandler.exceptions.Add(ex);
                }
            }
        }

        private void UpdateUI()
        {
            lock (entriesModel.EntriesFound)
            {
                endPosition = entriesModel.EntriesFound.Count;
            }
            for (int i = startPosition; i < endPosition; i++)
            {
                dispatcher.Invoke(new Action<Node>(AddNodeToTreeView), entriesModel.EntriesFound[i]);
            }
            startPosition = endPosition;
            UpdateErrors();
        }

        private void UpdateErrors()
        {
            lock (errorsModel.Errors)
            {
                errorsEndPosition = errorsModel.Errors.Count;
            }
            for (int i = errorsStartPosition; i < errorsEndPosition; i++)
            {
                dispatcher.Invoke(new Action(() => viewModel.ErrorsListItems.Add(errorsModel.Errors[i])));
            }
            errorsStartPosition = errorsEndPosition;
        }

        private void AddNodeToTreeView(Node node)
        {
            TreeViewItem parent = FindParentInTreeView(node); 
            if(parent!= null)
            {
                parent.Items.Add(new TreeViewItem { Header = node.Value, IsExpanded = true });
            } 
            else
            {
                viewModel.Tree.Add(new TreeViewItem { Header = node.Value, IsExpanded = true }); 
            }
        }

        private TreeViewItem FindParentInTreeView(Node node)
        {
            TreeViewItem currentItem;
            TreeViewItem parent = viewModel.Tree.FirstOrDefault();

            Node currentParentNode = null;
            Stack<Node> nodeParents = node.GetParents(); 
            if(nodeParents.Count!=0)
            {
                nodeParents.Pop(); 
            }
            while (nodeParents.Count > 0)
            {
                currentParentNode = nodeParents.Pop();
                currentItem = parent.Items.Cast<TreeViewItem>().Where(item => item.Header.ToString() == currentParentNode.Value).Single();
                parent = currentItem;
            }

            return parent;
        }

    }
}
