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

namespace UsingBingMaps.Models
{
    [DataContract]
    public class RestLocation
    {
        /// <summary>
        /// Optional for unstructured URL. The subdivision name in the country or region for an address. This element is typically treated as the first order administrative subdivision, but in some cases it is the second, third, or fourth order subdivision in a country, dependency, or region.
        /// A string that contains a subdivision, such as the abbreviation of a US state.
        /// Example: WA
        /// </summary>
        [DataMember]
        string adminDistrict {get;set;}

        /// <summary>
        /// Optional for unstructured URL. The locality, such as the city or neighborhood, that corresponds to an address.
        /// A string that contains the locality, such as a US city.
        /// Example: Seattle
        /// </summary>
        [DataMember]
        string locality {get;set;}

        /// <summary>
        /// Optional for unstructured URL. The post code, postal code, or ZIP Code of an address.
        /// A string that contains the postal code, such as a US ZIP Code.
        /// Example: 98178
        /// </summary>
        [DataMember]
        string postalCode{get;set;}

        /// <summary>
        /// Optional for unstructured URL. The official street line of an address relative to the area, as specified by the Locality, or PostalCode, properties. Typical use of this element would be to provide a street address or any official address.
        /// A string specifying the street line of an address.
        /// Example: 1 Microsoft Way
        /// </summary>
        [DataMember]
        string addressLine {get;set;}

        /// <summary>
        /// Optional for unstructured URL. The ISO country code for the country.
        /// A string specifying the ISO country code.
        /// Example: AU
        /// </summary>
        [DataMember]
        string countryRegion{get;set;}

        /// <summary>
        /// Optional. Specifies to include the neighborhood in the response when it is available.
        /// Alias value: inclnb
        /// Note:	 When you create your URL request, you can set the Locality parameter to a neighborhood. In this case, the neighborhood you provide may be returned in the neighborhood field of the response and a greater locality may be returned in the locality field. For example, you can create a request that specifies to include neighborhood information (inclnb=1) and that sets the Locality to Ballard and the AdminDistrict to WA (Washington State). In this case, the neighborhood field in the response is set to Ballard and the locality field is set to Seattle. You can find this example in the Examples section. 
        /// One of the following values:
        /// 1: Include neighborhood information when available.
        /// 0 [default]: Do not include neighborhood information.
        /// Example: 
        /// inclnb=1
        /// </summary>
        [DataMember]
        string includeNeighborhood {get;set;}

    }
}
