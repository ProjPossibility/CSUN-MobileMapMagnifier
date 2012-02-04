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

using System.Collections.Generic;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Platform;

namespace UsingBingMaps.Models
{
    /// <summary>
    /// Represents the route data model.
    /// </summary>
    public class RouteModel
    {
        private readonly LocationCollection _locations;

        /// <summary>
        /// Gets the location collection of this route.
        /// </summary>
        public ICollection<GeoCoordinate> Locations
        {
            get { return _locations; }
        }

        /// <summary>
        /// Initializes a new instance of this type.
        /// </summary>
        /// <param name="locations">A collection of locations.</param>
        public RouteModel(ICollection<Location> locations)
        {
            _locations = new LocationCollection();
            foreach (Location location in locations)
            {
                _locations.Add(location);
            }
        }
    }
}