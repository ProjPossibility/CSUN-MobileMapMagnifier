using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;


using UsingBingMaps.ServiceReference1;
using System.Collections.ObjectModel;
using Microsoft.Phone.Controls.Maps;


namespace GPS
{
    public partial class AddressPlotting : PhoneApplicationPage
    {
        GeocodeServiceClient _svc;
        public AddressPlotting()
        {
            InitializeComponent();

            _svc = new GeocodeServiceClient();
            _svc.GeocodeCompleted += (s, e) =>
            {
                // sort the returned record by ascending confidence in order for
                // highest confidence to be on the top. Based on the numeration High value is
                // at 0, Medium value at 1 and Low volue at 2
                var geoResult = (from r in e.Result.Results
                                 orderby (int)r.Confidence ascending
                                 select r).FirstOrDefault();
                if (geoResult != null)
                {
                    this.SetLocation(geoResult.Locations[0].Latitude,
                        geoResult.Locations[0].Longitude,
                        10,
                        true);
                }
            };
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

           string s = NavigationContext.QueryString["longitude"];
            this.set(s);
     
        }


        private void SetLocation(double latitude, double longitude, double zoomLevel, bool showLocator)
        {
            // Move the pushpin to geo coordinate
            Microsoft.Phone.Controls.Maps.Platform.Location location = new Microsoft.Phone.Controls.Maps.Platform.Location();
            location.Latitude = latitude;
            location.Longitude = longitude;
            bingMap.SetView(location, zoomLevel);
            bingMapLocator.Location = location;
            if (showLocator)
            {
                locator.Visibility = Visibility.Visible;
            }
            else
            {
                locator.Visibility = Visibility.Collapsed;
            }
        }

        private void set(string address)
        {
            UsingBingMaps.ServiceReference1.GeocodeRequest request = new UsingBingMaps.ServiceReference1.GeocodeRequest();

            // Only accept results with high confidence.
            request.Options = new GeocodeOptions()
            {
                Filters = new ObservableCollection<FilterBase>
        {
            new ConfidenceFilter()
            {
                MinimumConfidence = Confidence.High
            }
        }
            };

            request.Credentials = new Credentials()
            {
                ApplicationId = "ApSTxL8vPW3LwzcfWL1rgKGvLpm4Kmt5_pPrWQfbnU7BqpLDke69cWNWUkqjEVcz"
            };

            request.Query = address;

            // Make asynchronous call to fetch the geo coordinate data.
            _svc.GeocodeAsync(request);
        }

        private void GetAddress_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GetAddress_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
           // NavigationService.Navigate(new Uri("/New.xaml?from=map&page=new" + "&longitude=" + geo.Longitude + "&latitude=" + geo.Latitude, UriKind.RelativeOrAbsolute));
        }

    }
}