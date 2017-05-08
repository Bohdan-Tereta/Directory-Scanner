using System.Collections.Generic;

namespace DirectoryScanner.Models
{
    internal class EntriesModel
    {
        public EntriesModel()
        {
            EntriesFound = new List<Node>(); 
        }
        public List<Node> EntriesFound { get; set; }
        public bool IsLoadFinished { get; set; }
    }
}
