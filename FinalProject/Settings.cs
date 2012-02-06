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
using System.Collections.ObjectModel;
using UsingBingMaps.Bing.Route;

namespace GPS
{
    public class Settings
    {
        private static  ObservableCollection<Row> _Near_Location;
        public static ObservableCollection<Row> Near_Location
        {
            get
            {
                return _Near_Location;
            }
            set
            {
                if (_Near_Location != value)
                {
                    _Near_Location = value;
                }
            }
        }
        public static Row Alert;
        public static Boolean alert_status=false;

        private static ObservableCollection<ItineraryItem> itinerary;
        public static ObservableCollection<ItineraryItem> Itinerary
        {
            get
            {
                return itinerary;
            }
            set
            {
                itinerary = value;
            }
        }

        //Distance Unit
        public static String distance_unit = "miles";
    }
}
