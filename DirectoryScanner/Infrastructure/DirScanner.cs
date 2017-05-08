using DirectoryScanner.Models;
using System;
using System.IO;
using System.Threading;

namespace DirectoryScanner.Infrastructure
{
    internal class DirScanner
    { 
        private EntriesModel entriesModel;
        private ErrorsModel errorsModel; 
        private int nestingLevel = 0;
        private Node currentNode = null;
        public EventHandler OnLoadFinished = null; 
        public DirScanner(EntriesModel entriesModel, ErrorsModel errorsModel)
        {
            this.entriesModel = entriesModel;
            this.errorsModel = errorsModel; 
            entriesModel.IsLoadFinished = false;
        } 
        public void Reset()
        {
            entriesModel.EntriesFound.Clear();
            entriesModel.IsLoadFinished = false;
            errorsModel.Errors.Clear();
            currentNode = null; 
        }
        public Int64 DirSearch(object sDir)
        {
            Int64 size = 0; 
            try
            {
                nestingLevel++;
                string path = sDir as string;
                currentNode = new Node(currentNode, GetShortPath(path));
                TryAddNodeInfo(path, currentNode);
                AddNodeThreadSafe(currentNode); 
                size += AddFiles(path);
                size += AddDirectoriesRecursively(path);
                if (currentNode.NodeInfo != null)
                {
                    currentNode.NodeInfo.Length = size;
                }
                nestingLevel--;
                if (nestingLevel == 0)
                {
                    entriesModel.IsLoadFinished = true;
                    if (OnLoadFinished != null)
                    {
                        OnLoadFinished(this, null);
                    }
                }
            } 
            catch(ThreadAbortException)
            {

            }
            catch (Exception ex)
            {
                lock (GlobalExceptionHandler.exceptions)
                {
                    ex.Data.Add("ComponentName", this.GetType());
                    GlobalExceptionHandler.exceptions.Add(ex);
                }
            }
            return size; 
        }

        private void TryAddNodeInfo(string path, Node node)
        {
            try
            {
                node.NodeInfo = new NodeInfo(path);
            }
            catch (Exception ex)
            {
                lock (errorsModel.Errors)
                {
                    errorsModel.Errors.Add(ex.GetType() + " for path: " + path + "; message: " + ex.Message + " Tried to get path info.");
                }
            }
        }

        private void AddNodeThreadSafe(Node node)
        {
            lock (entriesModel.EntriesFound)
            {
                entriesModel.EntriesFound.Add(node);
            }
        }

        private Int64 AddFiles(string path)
        {
            Int64 size = 0; 
            string[] files = TryGetEntries(Directory.GetFiles, path);
            if (files != null)
            {
                foreach (string file in files)
                {
                    Node childNode = new Node(currentNode, GetShortPath(file));
                    TryAddNodeInfo(file, childNode);
                    AddNodeThreadSafe(childNode); 
                    if(childNode.NodeInfo != null)
                    { 
                    size += childNode.NodeInfo.Length;
                    }
                }
            }
            return size; 
        }
        private string[] TryGetEntries(Func<string, string[]> entriesSource, string path)
        {
            string[] entries = null;
            try
            {
                entries = entriesSource(path);
            }
            catch (Exception ex)
            {
                lock (errorsModel.Errors)
                {
                    errorsModel.Errors.Add(ex.GetType() + " for path: " + path + "; message: " + ex.Message + "Tried to acces the path.");
                }
            }
            return entries;
        }

        private void AddError(string message)
        {
            errorsModel.Errors.Add(message);
        }

        private Int64 AddDirectoriesRecursively(string path)
        {
            Int64 size = 0; 
            string[] directories = TryGetEntries(Directory.GetDirectories, path);
            if (directories != null)
            {
                foreach (string directory in directories)
                {
                    size += DirSearch(directory);
                    currentNode = currentNode.Parent; 
                }
            }
            return size; 
        }

        string GetShortPath(string path)
        {
            string shortPath;
            shortPath = Path.GetFileName(path);
            if (shortPath == "")
            {
                shortPath = path;
            }
            return shortPath;
        }
    }
}
