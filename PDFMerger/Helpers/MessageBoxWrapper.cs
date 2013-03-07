using System.Windows;

namespace PDFMerger.Helpers
{
    class MessageBoxWrapper : IMessageBoxWrapper
    {
        #region Implementation of IMessageBoxWrapper

        public void Show( string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon )
        {
            MessageBox.Show( messageBoxText, caption, button, icon );
        }

        #endregion Implementation of IMessageBoxWrapper
    }
}