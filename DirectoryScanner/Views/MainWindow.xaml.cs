using System;
using System.Windows;

namespace DirectoryScanner.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public EventHandler OnClose;

        public MainWindow()
        {
            InitializeComponent();
        }      

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OnClose(this, null); 
        }
    }
}
