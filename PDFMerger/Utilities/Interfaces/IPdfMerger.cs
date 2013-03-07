using System;
using PdfSharp.Pdf;

namespace PDFMerger.Utilities.Interfaces
{
    public interface IPdfMerger
    {
        PdfDocument Merge( string[] filePaths );

        PdfDocument Merge( string[] filePaths, Action afterEachAddedFileAction );
    }
}