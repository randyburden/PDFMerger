using System;
using System.Windows;
using PDFMerger.Utilities.Interfaces;
using PdfSharp.Pdf;

namespace PDFMerger.Utilities
{
    public class PdfMerger : IPdfMerger
    {
        private readonly IPdfFileReader _pdfReader;

        public delegate void ExceptionAction( Exception ex );

        public PdfMerger( IPdfFileReader pdfReader )
        {
            _pdfReader = pdfReader;
        }

        #region Implementation of IPdfMerger

        public PdfDocument Merge( string[] filePaths )
        {
           return Merge( filePaths, null );
        }

        public PdfDocument Merge( string[] filePaths, Action afterEachAddedFileAction )
        {
            PdfDocument mergedDocument = new PdfDocument();

            foreach ( var filePath in filePaths )
            {
                try
                {
                    using ( PdfDocument documentToAdd = _pdfReader.ReadFile( filePath ) )
                    {
                        for ( int i = 0; i < documentToAdd.PageCount; i++ )
                        {
                            PdfPage page = documentToAdd.Pages[ i ];

                            mergedDocument.AddPage( page );
                        }
                    }
                }
                catch ( Exception ex )
                {
                    string errorMessage = string.Format( "An error occurred processing the following file: {0}\n\nError Message: {1}\n\nFull Error: {2}", filePath, ex.Message, ex );

                    MessageBox.Show( errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error );
                }

                if ( afterEachAddedFileAction != null )
                {
                    afterEachAddedFileAction();
                }
            }

            return mergedDocument;
        }

        #endregion Implementation of IPdfMerger
    }
}
