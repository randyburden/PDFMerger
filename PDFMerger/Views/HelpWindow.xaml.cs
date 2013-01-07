using System;

namespace PDFMerger.Views
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow
    {
        public HelpWindow()
        {
            InitializeComponent();

            const string fileName = "help.html";

            var path = System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "HelpFile" );

            path = System.IO.Path.Combine( path, fileName );

            HelpWindowWebBrowser.Navigate( path );
        }
    }
}
