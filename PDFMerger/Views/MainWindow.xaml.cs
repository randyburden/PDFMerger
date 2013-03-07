using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Shell;
using PDFMerger.Helpers;
using PDFMerger.Utilities;
using System.Linq;

namespace PDFMerger.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        internal static IMessageBoxWrapper MessageBoxWrapper;

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            MessageBoxWrapper = new MessageBoxWrapper();

            SetTheme();
        }

        #endregion Constructor

        #region Initialization

        private void SetTheme()
        {
            try
            {
                foreach ( MenuItem item in ThemeMenuItem.Items )
                {
                    if ( item.Header.ToString().Replace( "_", "" ).ToUpper() == ConfigurationHelper.ThemeName.ToUpper() )
                    {
                        item.IsChecked = true;
                    }
                }

                string themeName = ConfigurationHelper.ThemeName;

                var theme = new ResourceDictionary();

                theme.Source = new Uri( string.Format( @"/ReuxablesLegacy;component/{0}.xaml", themeName ), UriKind.Relative );

                Application.Current.Resources.MergedDictionaries.Clear();

                Application.Current.Resources.MergedDictionaries.Add( theme );

                if ( themeName.ToUpper() == "INC" )
                {
                    var incThemeOverides = new ResourceDictionary();

                    incThemeOverides.Source = new Uri( "/Resources/IncThemeOverrides.xaml", System.UriKind.Relative );

                    Application.Current.Resources.MergedDictionaries.Add( incThemeOverides );
                }
            }
            catch
            {
                // Swallow Error in case third party dll does not exist
            }
        }

        #endregion Initialization

        #region Menu Events

        private void ExitApplication( object sender, RoutedEventArgs e )
        {
            Application.Current.Shutdown();
        }

        private void SwitchThemes( object sender, RoutedEventArgs e )
        {
            try
            {
                var clickedMenuItem = sender as MenuItem;

                foreach ( MenuItem menuItem in ThemeMenuItem.Items )
                {
                    menuItem.IsChecked = false;
                }

                clickedMenuItem.IsChecked = true;

                var control = ( System.Windows.Controls.HeaderedItemsControl ) sender;

                string themeName = control.Header.ToString().Replace( "_", "" );

                ConfigurationHelper.ThemeName = themeName;

                ConfigurationHelper.SaveAll();

                if ( themeName == "Plain Jane" )
                {
                    Application.Current.Resources.MergedDictionaries.Clear();

                    return;
                }

                var theme = new ResourceDictionary();

                theme.Source = new Uri( string.Format( @"/ReuxablesLegacy;component/{0}.xaml", themeName ), UriKind.Relative );

                Application.Current.Resources.MergedDictionaries.Clear();

                Application.Current.Resources.MergedDictionaries.Add( theme );

                if ( themeName.ToUpper() == "INC" )
                {
                    var incThemeOverides = new ResourceDictionary();

                    incThemeOverides.Source = new Uri( "/Resources/IncThemeOverrides.xaml", System.UriKind.Relative );

                    Application.Current.Resources.MergedDictionaries.Add( incThemeOverides );
                }
            }
            catch ( Exception ex )
            {
                MessageBoxWrapper.Show( string.Format( "An error occured when attempting to switch themes: {0}", ex.Message ),
                                 "Error", MessageBoxButton.OK, MessageBoxImage.Error );
            }
        }

        private void HelpItem_OnClick( object sender, RoutedEventArgs e )
        {
            var helpWindow = new HelpWindow { Owner = this };
            helpWindow.Show();
        }

        #endregion Menu Events

        #region UI Events

        private void DirectoryButton_Click( object sender, RoutedEventArgs e )
        {
            var dialog = new CommonOpenFileDialog
                {
                    EnsureReadOnly = true, 
                    IsFolderPicker = true, 
                    AllowNonFileSystemItems = false
                };

            if ( dialog.ShowDialog() == CommonFileDialogResult.Ok )
            {
                try
                {
                    // Try to get a valid selected item
                    var shellContainer = dialog.FileAsShellObject as ShellContainer;

                    InputDirectoryTextBox.Text = shellContainer.ParsingName;
                }
                catch
                {
                    MessageBoxWrapper.Show( "An error occurred selecting the selected directory", "Error", MessageBoxButton.OK, MessageBoxImage.Error );
                }
            }
        }

        private void OutputFileButton_Click( object sender, RoutedEventArgs e )
        {
            var dialog = new CommonSaveFileDialog
            {
                EnsureReadOnly = true,
                DefaultDirectory = "C:\\",
                DefaultFileName = "MergedPdf.pdf",
                Title = "Output File Name"
            };

            if ( dialog.ShowDialog() == CommonFileDialogResult.Ok )
            {
                OutputFileTextBox.Text = dialog.FileName;
            }
        }

        private void MergeButton_Click( object sender, RoutedEventArgs e )
        {
            if ( MergeButton.IsEnabled == false )
            {
                return;
            }

            var inputDirectoryPath = InputDirectoryTextBox.Text;

            if ( string.IsNullOrWhiteSpace( inputDirectoryPath ) )
            {
                MessageBoxWrapper.Show( "Please first select the directory containing the PDFs.", "Select a Directory", MessageBoxButton.OK, MessageBoxImage.Warning );

                DirectoryButton_Click( null, null );

                return;
            }

            if ( Directory.Exists( inputDirectoryPath ) == false )
            {
                MessageBoxWrapper.Show( "The selected directory does not exist", "Directory Not Found", MessageBoxButton.OK, MessageBoxImage.Warning );

                return;
            }

            var fullOutputPath = OutputFileTextBox.Text;

            if ( string.IsNullOrWhiteSpace( fullOutputPath ) )
            {
                MessageBoxWrapper.Show( "Please first select the output file.", "Select an Output File", MessageBoxButton.OK, MessageBoxImage.Warning );

                OutputFileButton_Click( null, null );

                return;
            }

            int batchSize = 1;

            if ( MergeInBatchesRadioButton.IsChecked != null && MergeInBatchesRadioButton.IsChecked.Value )
            {
                if ( int.TryParse( BatchSizeTextBox.Text, out batchSize ) == false )
                {
                    MessageBoxWrapper.Show( "Please input a valid batch size.", "Input a Batch Size", MessageBoxButton.OK, MessageBoxImage.Warning );

                    BatchSizeTextBox.SelectAll();

                    BatchSizeTextBox.Focus();

                    return;
                }
            }

            var files = Directory.GetFiles( inputDirectoryPath, "*.pdf" );

            if ( files.Length == 0 )
            {
                MessageBoxWrapper.Show( "No files with the .pdf extension were found.", "No Files Found", MessageBoxButton.OK, MessageBoxImage.Information );

                return;
            }

            ProgressBar.Maximum = files.Length;

            ProgressBar.Value = 0;

            if ( MergeAllRadioButton.IsChecked != null && MergeAllRadioButton.IsChecked.Value )
            {
                DisableMergeButton();

                Task.Factory.StartNew( () => MergeAll( fullOutputPath, files ) );
            }
            else if ( MergeInBatchesRadioButton.IsChecked != null && MergeInBatchesRadioButton.IsChecked.Value )
            {
                DisableMergeButton();
                
                Task.Factory.StartNew( () => MergeInBatches( fullOutputPath, files, batchSize ) );
            }
        }
        
        #endregion UI Events

        #region Helpers

        public void IncrementProgressBar()
        {
            ProgressBar.PerformAction( () => ProgressBar.Value++ );
        }

        public void EnableMergeButton()
        {
            MergeButton.PerformAction( () => MergeButton.IsEnabled = true );
        }

        public void DisableMergeButton()
        {
            MergeButton.PerformAction( () => MergeButton.IsEnabled = false );
        }

        public void ShowExceptionMessage( Exception ex )
        {
            string errorMessage = string.Format( "An error occurred: {0}\n\n{1}", ex.Message, ex );

            MessageBoxWrapper.Show( errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error );
        }

        public void OpenOutputDirectory()
        {
            Action action = () =>
                {
                    string fullOutputFileName = OutputFileTextBox.Text;

                    string path = Path.GetDirectoryName( fullOutputFileName );

                    if ( path != null && Directory.Exists( path ) )
                    {
                        System.Diagnostics.Process.Start( path );
                    }
                };

            OutputFileTextBox.PerformAction( action );
        }

        public void MergeAll( string fullOutputPath, string[] files )
        {
            var stopwatch = Stopwatch.StartNew();

            var pdfMerger = new PdfMerger( new PdfFileReader() );

            try
            {
                using ( var document = pdfMerger.Merge( files, IncrementProgressBar ) )
                {
                    document.Save( fullOutputPath );
                }
            }
            catch ( Exception ex )
            {
                ShowExceptionMessage( ex );
            }

            string message = string.Format( "Complete. Elapsed time: {0}", stopwatch.GetElapsedTimeString() );

            MessageBoxWrapper.Show( message, "Complete", MessageBoxButton.OK, MessageBoxImage.Information );

            EnableMergeButton();

            OpenOutputDirectory();
        }

        public void MergeInBatches( string fullOutputPath, string[] files, int batchSize  )
        {
            string outputPath = Path.GetDirectoryName( fullOutputPath );

            string outputFileName = Path.GetFileNameWithoutExtension( fullOutputPath );

            string extension = Path.GetExtension( fullOutputPath );

            var stopwatch = Stopwatch.StartNew();

            var pdfMerger = new PdfMerger( new PdfFileReader() );

            var filePaths = files.AsEnumerable();

            int iteration = 1;

            while ( filePaths.Any() )
            {
                var filePathBatch = filePaths.Take( batchSize );

                try
                {
                    using ( var document = pdfMerger.Merge( filePathBatch.ToArray(), IncrementProgressBar ) )
                    {
                        if ( iteration > 1 )
                        {
                            string newFileName = string.Format( "{0}{1}{2}", outputFileName, iteration, extension );

                            string newPath = Path.Combine( outputPath, newFileName );

                            document.Save( newPath );
                        }
                        else
                        {
                            document.Save( fullOutputPath );
                        }
                    }
                }
                catch ( Exception ex )
                {
                    ShowExceptionMessage( ex );
                }

                iteration++;

                filePaths = filePaths.Skip( batchSize );
            }


            string message = string.Format( "Complete. Elapsed time: {0}", stopwatch.GetElapsedTimeString() );

            MessageBoxWrapper.Show( message, "Complete", MessageBoxButton.OK, MessageBoxImage.Information );

            EnableMergeButton();

            OpenOutputDirectory();
        }

        #endregion Helpers
    }
}