using System.Collections.Generic;

namespace DirectoryScanner.Models
{
    internal class Node 
    {
        public Node(Node parent, string value)
        {
            this.Parent = parent;
            this.Value = value;
        }
        public Node Parent { get; set; }
        public string Value { get; set; }
        public NodeInfo NodeInfo { get; set; }
        public Stack<Node> GetParents()
        {
            Node currentNode = this;
            Stack<Node> parents = new Stack<Node>();
            while (currentNode.Parent != null)
            {
                currentNode = currentNode.Parent;
                parents.Push(currentNode);
            }
            return parents;
        }
    }
}
