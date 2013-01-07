using System;
using System.Windows;
using System.Windows.Threading;

namespace PDFMerger.Helpers
{
    public class FrameworkElementHelpers
    {
        public static void SafelyAccess( FrameworkElement frameworkElement, Action action )
        {
            if ( frameworkElement.Dispatcher.CheckAccess() == false )
            {
                Application.Current.Dispatcher.BeginInvoke( DispatcherPriority.Background, action );
            }
            else
            {
                action();
            }
        }
    }
}
