using PdfSharp.Pdf;

namespace PDFMerger.Utilities.Interfaces
{
    public interface IPdfFileReader
    {
        PdfDocument ReadFile( string filePath );
    }
}