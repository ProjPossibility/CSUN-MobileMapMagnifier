using System;
using System.Device.Location;
using System.Collections.Generic;

using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Platform;

namespace GPS
{
    public static class LocationExtensions 
    {
        public static GeoCoordinate ToCoordinate(this Location routeLocation)
        {
            return new GeoCoordinate(routeLocation.Latitude, routeLocation.Longitude);
        }
        public static Location ToLocation(this GeoCoordinate coordinate)
        {
            return new Location() { Latitude = coordinate.Latitude, Longitude = coordinate.Longitude };
        }

        public static LocationCollection ToCoordinates(this IEnumerable<Location> points)
        {
            var locations = new LocationCollection();

            if (points != null)
            {
                foreach (var point in points)
                {
                    locations.Add(point.ToCoordinate());
                }
            }

            return locations;
        }
    }
}
