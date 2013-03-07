using System;
using System.Windows;
using System.Windows.Threading;

namespace PDFMerger.Helpers
{
    public static class FrameworkElementHelpers
    {
        /// <summary>
        /// Provides safe access to a <see cref="FrameworkElement"/> and allows you to perform an
        /// Action on the element 
        /// </summary>
        /// <param name="frameworkElement"></param>
        /// <param name="action"></param>
        /// <param name="dispatcherPriority"></param>
        public static void SafelyAccess( FrameworkElement frameworkElement, Action action, DispatcherPriority dispatcherPriority = DispatcherPriority.Render )
        {
            if ( frameworkElement.Dispatcher.CheckAccess() == false )
            {
                Application.Current.Dispatcher.BeginInvoke( dispatcherPriority, action );
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// Provides a method for safely accessing a <see cref="FrameworkElement"/> from any thread and performing an action
        /// on it.
        /// </summary>
        /// <param name="frameworkElement">An element.</param>
        /// <param name="action">An action to perform.</param>
        /// <param name="dispatcherPriority">Optional priority to use when invoking an action on this element from a non-UI thread.</param>
        public static void PerformAction( this FrameworkElement frameworkElement, Action action, DispatcherPriority dispatcherPriority = DispatcherPriority.Render )
        {
            if ( frameworkElement.Dispatcher.CheckAccess() == false )
            {
                Application.Current.Dispatcher.BeginInvoke( dispatcherPriority, action );
            }
            else
            {
                action();
            }
        }
    }
}
