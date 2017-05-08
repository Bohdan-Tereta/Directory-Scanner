using System.Collections.Generic;

namespace DirectoryScanner.Models
{
    internal class ErrorsModel
    {
        public ErrorsModel()
        {
            Errors = new List<string>(); 
        }
        internal List<string> Errors { get; set; }
    }
}
