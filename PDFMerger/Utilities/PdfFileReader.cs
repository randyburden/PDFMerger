using PDFMerger.Utilities.Interfaces;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PDFMerger.Utilities
{
    public class PdfFileReader : IPdfFileReader
    {
        #region Implementation of IPdfFileReader

        public PdfDocument ReadFile( string filePath )
        {
            PdfDocument document = PdfSharp.Pdf.IO.PdfReader.Open( filePath, PdfDocumentOpenMode.Import );

            return document;
        }

        #endregion Implementation of IPdfFileReader
    }
}