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

using UsingBingMaps.Bing.Geocode;

namespace UsingBingMaps.Helpers
{
    /// <summary>
    /// Internally used for passing state between route asynchronous calls.
    /// </summary>
    internal class RoutingState
    {
        internal RoutingState(GeocodeResult[] resultArray, int index, string tb)
        {
            Results = resultArray;
            LocationNumber = index;
            Output = tb;
        }

        internal bool GeocodesComplete
        {
            get
            {
                for (int idx = 0; idx < Results.Length; idx++)
                {
                    if (null == Results[idx])
                        return false;
                }
                return true;
            }
        }

        internal bool GeocodesSuccessful
        {
            get
            {
                for (int idx = 0; idx < Results.Length; idx++)
                {
                    if (null == Results[idx] || null == Results[idx].Locations || 0 == Results[idx].Locations.Count)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        internal GeocodeResult[] Results { get; set; }
        internal int LocationNumber { get; set; }
        internal string Output { get; set; }
    }
}