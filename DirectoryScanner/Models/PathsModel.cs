namespace DirectoryScanner.Models
{
    public class PathsModel
    {
        public PathsModel(string inputPath, string outputPath)
        {
            InputDirectoryPath = inputPath;
            OutputDirectoryPath = outputPath; 
        }
        public string InputDirectoryPath { get; set; }
        public string OutputDirectoryPath { get; set; }
    }
}
