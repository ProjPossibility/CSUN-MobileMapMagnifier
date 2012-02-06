// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

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
using UsingBingMaps.Bing.Route;
using UsingBingMaps.Bing.Geocode;

namespace UsingBingMaps.Helpers
{
    /// <summary>
    /// Represents a route calculation error.
    /// </summary>
    public class RouteCalculationError
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

        internal RouteCalculationError(string reason, Exception exception)
        {
            Reason = reason;
            Exception = exception;
        }

        internal RouteCalculationError(GeocodeCompletedEventArgs e)
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

        internal RouteCalculationError(CalculateRouteCompletedEventArgs e)
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