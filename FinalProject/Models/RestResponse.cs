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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UsingBingMaps.Models;

namespace UsingBingMaps.Model
{
    [DataContract]
    public class RestResponse
    {
        // http://msdn.microsoft.com/en-us/library/ff701707.aspx

        /// <summary>
        /// The HTTP Status code for the request.
        /// </summary>
        [DataMember]
        public int statusCode { get; set; }


        /// <summary>
        /// A description of the HTTP status code.
        /// </summary>
        [DataMember]
        public string statusDescription { get; set; }

        /// <summary>
        /// A status code that offers additional information about authentication success or failure.
        /// One of the following values: 
        ///     ValidCredentials
        ///     InvalidCredentials
        ///     CredentialsExpired
        ///     NotAuthorized
        ///     NoCredentials
        ///     None
        /// </summary>
        [DataMember]
        public string authenticationResultCode { get; set; }

        /// <summary>
        /// A unique identifier for the request.
        /// </summary>
        [DataMember]
        public string traceId { get; set; }

        /// <summary>
        /// A copyright notice.
        /// </summary>
        [DataMember]
        public string copyright { get; set; }

        /// <summary>
        /// A URL that references a brand image to support contractual branding requirements.
        /// </summary>
        [DataMember]
        public string brandLogoUri { get; set; }

        /// <summary>
        /// A collection of ResourceSet objects. A ResourceSet is a container of Resources returned by the request. For more information, see the ResourceSet section below.
        /// A collection of one or more resources. The resources that are returned depend on the request. Information about resources is provided in the API reference for each Bing Maps REST Services API.
        /// </summary>
        [DataMember]
        public ObservableCollection<ResourceSet> resourceSets { get; set; }

        /// <summary>
        /// A collection of error descriptions. For example, ErrorDetails can identify parameter values that are not valid or missing.
        /// </summary>
        [DataMember]
        public string[] errorDetails { get; set; }
    }
    public class ResourceSet
    {
        /// <summary>
        //An estimate of the total number of resources in the ResourceSet.
        /// </summary>
        [DataMember]
        public string estimatedTotal { get; set; }

        /// <summary>
        /// A collection of ResourceSet objects. A ResourceSet is a container of Resources returned by the request. For more information, see the ResourceSet section below.
        /// A collection of one or more resources. The resources that are returned depend on the request. Information about resources is provided in the API reference for each Bing Maps REST Services API.
        /// </summary>
        [DataMember]
        public ObservableCollection<LocationData> resources { get; set; }

    }
}
