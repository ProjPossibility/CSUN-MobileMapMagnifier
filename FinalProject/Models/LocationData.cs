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
using System.Runtime.Serialization;
using UsingBingMaps.Bing.Search;
using System.Collections.Generic;

namespace UsingBingMaps.Models
{
    [DataContract]
    public class LocationData
    {
        // http://msdn.microsoft.com/en-us/library/ff701725.aspx

  

        /// <summary>
        /// The name of the resource.
        /// </summary>
        public string name {get;set;}

        /// <summary>
        /// The latitude and longitude coordinates of the location.
        /// </summary>
        public UsingBingMaps.Models.Point point { get; set; }

        /// <summary>
        /// A geographic area that contains the location. A bounding box contains SouthLatitude, WestLongitude, NorthLatitude, and EastLongitude values in units of degrees.
        /// </summary>
       // public BoundingBox bbox {get;set;}

        /// <summary>
        /// The classification of the geographic entity returned, such as Address. For a list of entity types, see Entity Types.
        /// </summary>
        public string entityType {get;set;}

        /// <summary>
        /// The postal address for the location. An address can contain AddressLine, Neighborhood, Locality, AdminDistrict, AdminDistrict2, CountryRegion, FormattedAddress, PostalCode, and Landmark fields. For more information about these fields, see Location and Area Types.
        /// </summary>
        public Address address{get;set;}

        /// <summary>
        /// One of the following values:
        /// High, Medium, Low
        /// The level of confidence that the geocoded location result is a match. Use this value with the match code to determine for more complete information about the match.
        /// The confidence of a geocoded location is based on many factors including the relative importance of the geocoded location and the user’s location, if specified. The following description provides more information about how confidence scores are assigned and how results are ranked.
        /// If the confidence is set to High, one or more strong matches were found. Multiple High confidence matches are sorted in ranked order by importance when applicable. For example, landmarks have importance but addresses do not.
        /// If a request includes a user location or a map area (see User Context Parameters), then the ranking may change appropriately. For example, a location query for "Paris" returns "Paris, France" and "Paris, TX" both with High confidence. "Paris, France" is always ranked first due to importance unless a user location indicates that the user is in or very close to Paris, TX or the map view indicates that the user is searching in that area.
        /// In some situations, the returned match may not be at the same level as the information provided in the request. For example, a request may specify address information and the geocode service may only be able to match a postal code. In this case, if the geocode service has a high confidence that the postal code matches the data, the confidence is set to High and the match code is set to UpHierarchy to specify that it could not match all of the information and had to search up-hierarchy.
        /// If the location information in the query is ambiguous, and there is no additional information to rank the locations (such as user location or the relative importance of the location), the confidence is set to Medium. For example, a location query for "148th Ave, Bellevue" may return "148th Ave SE" and "148th Ave NE" both with Medium confidence.
        /// If the location information in the query does not provide enough information to geocode a specific location, a less precise location value may be returned and the confidence is set to Medium. For example, if an address is provided, but a match is not found for the house number, the geocode result with a Roadblock entity type may be returned. You can check the entityType field value to determine what type of entity the geocode result represents. For a list of entity types, see Entity Types.
        /// </summary>
        public Confidence confidence { get;set;}

        /// <summary>
        /// One or more of the following values: Good, Ambiguous, UpHierarchy
        /// One or more match code values that represent the geocoding level for each location in the response.
        /// For example, a geocoded location with match codes of Good and Ambiguous means that more than one geocode location was found for the location information and that the geocode service did not have search up-hierarchy to find a match.
        /// Similarly, a geocoded location with match codes of Ambiguous and UpHierarchy means that a geocode location could not be found that matched all of the location information, so the geocode service had to search up-hierarchy and found multiple matches at that level. An example of up an Ambiguous and UpHierarchy result is when you provide complete address information, but the geocode service cannot locate a match for the street address and instead returns information for more than one RoadBlock value.
        /// The possible values are:
        /// Good: The location has only one match or all returned matches are considered strong matches. For example, a query for New York returns several Good matches.
        /// Ambiguous: The location is one of a set of possible matches. For example, when you query for the street address 128 Main St., the response may return two locations for 128 North Main St. and 128 South Main St. because there is not enough information to determine which option to choose.
        /// UpHierarchy: The location represents a move up the geographic hierarchy. This occurs when a match for the location request was not found, so a less precise result is returned. For example, if a match for the requested address cannot be found, then a match code of UpHierarchy with a RoadBlock entity type may be returned.
        /// </summary>
     //   public MatchCode matchCodes {get;set;}


        /// <summary>
        /// A collection of geocoded points that differ in how they were calculated and their suggested use. For a description of the points in this collection, see the Geocode Point Fields section below.
        /// </summary>
       // public GeocodePoint geocodePoints {get;set;}



    }

    public class Point
    {
        [DataMember]
        public String type { get; set; }

        [DataMember]
        public List<Double> coordinates { get; set; }
    }
}
