using System.Windows;

namespace PDFMerger.Helpers
{
    public interface IMessageBoxWrapper
    {
        void Show( string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon );
    }
}