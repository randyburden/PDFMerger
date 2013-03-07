using System;
using System.Diagnostics;

namespace PDFMerger.Helpers
{
    public static class StopwatchHelper
    {
        /// <summary>
        /// Returns the elapsed time of the stopwatch
        /// </summary>
        /// <returns>A string with a customized output of the elapsed time</returns>
        public static string GetElapsedTimeString( this Stopwatch stopWatch )
        {
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime;

            if ( ts.Minutes > 0 )
                elapsedTime = String.Format( "{0:00} min. {1:00}.{2:00} sec.",
                    ts.Minutes, ts.Seconds, ts.Milliseconds / 10 );
            else
                elapsedTime = String.Format( "{0:00}.{1:00} sec.",
                    ts.Seconds, ts.Milliseconds / 10 );

            return elapsedTime;
        }
    }
}
