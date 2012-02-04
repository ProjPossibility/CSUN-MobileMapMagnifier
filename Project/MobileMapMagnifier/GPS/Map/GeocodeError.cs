using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using UsingBingMaps.Bing.Geocode;

namespace UsingBingMaps.Map
{
    public class GeocodeError
    {
        private const string NoResults = "No results.";

        /// <summary>
        /// Gets the reason of the error.
        /// </summary>
        public string Reason { get; private set; }

        /// <summary>
        /// Get the exception instance or null if none.
        /// </summary>
        public Exception Exception { get; private set; }

        internal GeocodeError(string reason, Exception exception)
        {
            Reason = reason;
            Exception = exception;
        }

        internal GeocodeError(ReverseGeocodeCompletedEventArgs e)
        {
            if (e.Result == null ||
                e.Result.ResponseSummary == null ||
                string.IsNullOrEmpty(e.Result.ResponseSummary.FaultReason))
            {
                Reason = NoResults;
            }
            else
            {
                Reason = e.Result.ResponseSummary.FaultReason;
            }

            Exception = e.Error;
        }
    }
}
