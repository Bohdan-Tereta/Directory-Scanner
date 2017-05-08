using System;
using System.Collections.ObjectModel;

namespace DirectoryScanner.Infrastructure
{
    internal static class GlobalExceptionHandler
    {
        static GlobalExceptionHandler()
        {
            exceptions = new ObservableCollection<Exception>();
        }

        public static ObservableCollection<Exception> exceptions { get; set; } 
    }
}
