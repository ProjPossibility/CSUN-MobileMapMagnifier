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
using UsingBingMaps.Model;
using Newtonsoft.Json;
using UsingBingMaps.Models;
using System.Diagnostics;

namespace UsingBingMaps.REST
{
    public class RestClient
    {
        string BING_KEY = "ApSTxL8vPW3LwzcfWL1rgKGvLpm4Kmt5_pPrWQfbnU7BqpLDke69cWNWUkqjEVcz";
        WebClient wc;
        public RestClient()
        {
            wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(JSONString);
            //TEST 
            SearchAddress("18111%20Nordhoff%20Street");
            // http://dev.virtualearth.net/REST/v1/Locations?addressLine=18111%20Nordhoff%20Street%20Northridge%20CA%2091330&key=ApSTxL8vPW3LwzcfWL1rgKGvLpm4Kmt5_pPrWQfbnU7BqpLDke69cWNWUkqjEVcz
            //http://dev.virtualearth.net/REST/v1/Locations?addressLine=18111%20Nordhoff%20Street&key=ApSTxL8vPW3LwzcfWL1rgKGvLpm4Kmt5_pPrWQfbnU7BqpLDke69cWNWUkqjEVcz
        }

        public void SearchAddress(string address)
        {
            Debug.WriteLine("Searching for address " + address);
            //if (!wc.IsBusy)
            //{
                wc.DownloadStringAsync(new Uri("http://dev.virtualearth.net/REST/v1/Locations?addressLine=" + address + "&key=" + BING_KEY));
            //}
        }

        public void JSONString(object sender, DownloadStringCompletedEventArgs e)
        {
            Debug.WriteLine("Receiving JSON: " + e.Result);
            if (e.Error != null)
                return;
            RestResponse rr = JsonConvert.DeserializeObject<RestResponse>(e.Result);

            foreach (ResourceSet locationSet in rr.resourceSets)
            {
                foreach (LocationData location in locationSet.resources)
                {
                    Debug.WriteLine("address line = " +location.address);
                    Debug.WriteLine("confi= " + location.confidence);
                    Debug.WriteLine("entiyty type = " + location.entityType);
                    Debug.WriteLine("name= " + location.name);
                    Debug.WriteLine("point = " + location.point);
                //    Debug.WriteLine("point x= " + location.point.coordinates);
                 //   Debug.WriteLine("point y = " + location.point.coordinates);
                    Debug.WriteLine("NEED TO FIX LOCATION DATA AND FINISH TODOS");
                }
            }
        }
    }
}
