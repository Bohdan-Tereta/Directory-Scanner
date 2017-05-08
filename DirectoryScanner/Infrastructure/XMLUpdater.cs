using DirectoryScanner.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;

namespace DirectoryScanner.Infrastructure
{
    internal class XMLUpdater
    {
        private EntriesModel entriesModel;
        private ErrorsModel errorsModel;
        private int startPosition = 0;
        private int endPosition = 0; 
        XmlDocument xmlDocument;
        public EventHandler OnLoadFinished = null;
        public XMLUpdater(EntriesModel entriesModel, ErrorsModel errorModel)
        {
            this.entriesModel = entriesModel;
            this.errorsModel = errorModel; 
        }
        public void Reset()
        {
            startPosition = 0;
            endPosition = 0;
        }
        public void Write(object fDir)
        {
            try
            {
                startPosition = 0;
                endPosition = 0;
                string path = fDir as string;
                xmlDocument = new XmlDocument();
                while (!(entriesModel.IsLoadFinished && startPosition == entriesModel.EntriesFound.Count))
                {
                    Write();
                }
                foreach (Node node in entriesModel.EntriesFound)
                {
                    if (node.NodeInfo != null && node.NodeInfo.Length != 0)
                    {
                        XmlNode current;
                        string xPath = GenerateXPath(node);
                        if (xPath != "")
                        {
                            current = xmlDocument.SelectSingleNode(xPath + "/" + "Node[@value='" + XmlConvert.EncodeName((node.Value)) + "']/Length");
                        }
                        else
                        {
                            current = xmlDocument.SelectSingleNode("/Node[@value='" + XmlConvert.EncodeName((node.Value)) + "']/Length");
                        }
                        current.Attributes["value"].Value = node.NodeInfo.Length.ToString();
                    }
                }
                xmlDocument.Save(path);
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
                lock (GlobalExceptionHandler.exceptions)
                {
                    ex.Data.Add("ComponentName", this.GetType());
                    GlobalExceptionHandler.exceptions.Add(ex);
                }
            }
        }
        private void Write()
        {
            lock (entriesModel.EntriesFound)
            {
                endPosition = entriesModel.EntriesFound.Count;
            }
            for (int i = startPosition; i < endPosition; i++)
            {
                WriteElementToFile(entriesModel.EntriesFound[i]);
            }
            startPosition = endPosition;
        }

        private void WriteElementToFile(Node node)
        {
            XmlNode parent;
            string xPath = GenerateXPath(node);
            if (xPath != "")
            {
                parent = xmlDocument.SelectSingleNode(xPath);
            }
            else
            {
                parent = xmlDocument;
            }
            XmlElement element = xmlDocument.CreateElement("Node");
            element.SetAttribute("value", XmlConvert.EncodeName((node.Value)));
            AddNodeInfo(element, node.NodeInfo);
            parent.AppendChild(element);
        }

        private string GenerateXPath(Node node)
        {
            StringBuilder stringBuilder = new StringBuilder();

            Stack<Node> nodeParents = node.GetParents();
            while (nodeParents.Count > 0)
            {
                stringBuilder.Append("/");
                stringBuilder.Append("Node[@value='" + XmlConvert.EncodeName(nodeParents.Pop().Value) + "']");
            }
            return stringBuilder.ToString();
        }

        private void AddNodeInfo(XmlElement element, NodeInfo nodeInfo)
        {
            if (nodeInfo != null)
            {
                AddChildElement("CreationTime", nodeInfo.CreationTime.ToString("yyyyMMdd"), element);
                AddChildElement("LastWriteTime", nodeInfo.LastWriteTime.ToString("yyyyMMdd"), element);
                AddChildElement("LastAccessTime", nodeInfo.LastAccessTime.ToString("yyyyMMdd"), element);
                AddChildElement("Attributes", XmlConvert.EncodeName(nodeInfo.Attributes), element);
                AddChildElement("Length", nodeInfo.Length.ToString(), element);
                AddChildElement("Owner", XmlConvert.EncodeName(nodeInfo.Owner), element);
                AddChildElement("AccessRules", XmlConvert.EncodeName(nodeInfo.AccessRules), element);
            }
        }

        private void AddChildElement(string name, string value, XmlElement parent)
        {

            XmlElement childElement;
            childElement = xmlDocument.CreateElement(name);
            childElement.SetAttribute("value", value);
            parent.AppendChild(childElement);
        }
    }
}
