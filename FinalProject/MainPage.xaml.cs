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
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Core;

using System.Device.Location;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;

using UsingBingMaps.Models;
using UsingBingMaps.Helpers;
using Microsoft.Phone.Tasks;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Controls.Maps.Platform;
using UsingBingMaps.Bing.Route;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Scheduler;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using Microsoft.Phone.Net.NetworkInformation;

using System.Globalization;
using UsingBingMaps.Bing.Geocode;
using UsingBingMaps.Map;
using UsingBingMaps.Ocr;
using Microsoft.Devices;
//using UsingBingMaps.Bing.Route;

namespace GPS
{
    public partial class MainPage : PhoneApplicationPage
    {

        #region Variables

        IsolatedStorageSettings isolatedStorage;
        public enum DistanceIn { Miles, Kilometers };
        const double EarthRadiusInMiles = 3959;
        const double EarthRadiusInKilometers = 6371;
        static Color COLOR_RING_TONE = Colors.Yellow;
        static Color COLOR_ALARM = Colors.Red;
        static Color COLOR_CAR_LOCATOR = Colors.Blue;

        System.Windows.Point StartLocation;
        System.Windows.Point EndLocation;
        System.Windows.Point MyLocation;
        System.Windows.Point SelectedLocation;
        GeoCoordinateWatcher watcher;
        Pushpin Current_Location = new Pushpin();
        double longitude;
        double latitude;
        Boolean TrackLocation = true;
        DispatcherTimer tmr;
        bool gpsReady = false;
        Boolean alert_mode=false;
        Boolean show_route = false;
        //// Data context for the local database
        //private GPS.Row.GPSDataContext DB;
        // Define an observable collection property that controls can bind to.
        private ObservableCollection<Row> _Table;
        MapLayer _PushpinLayer;
        //MapLayer _RadiusLayer;
        //System.Windows.Media.BitmapCache mapCache;

        #region Getters & Setters

        public string To { get; set; }
        public string From { get; set; }

        #endregion // Getters & Setters


        
        /// <value>Provides credentials for the map control.</value>
        private readonly CredentialsProvider _credentialsProvider = new ApplicationIdCredentialsProvider(App.Id);
        ProgressIndicator prog;
        #endregion // Variables

        DispatcherTimer infoTimer = new DispatcherTimer();


        #region Initialization

        // Constructor
        public MainPage()
        {
                InitializeComponent();
                Debug.WriteLine("MainPage");

  
                InfoStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                InfoTexBlock.Visibility = System.Windows.Visibility.Visible;
                infoTimer.Tick += new EventHandler(infoTimer_Tick);
                infoTimer.Interval = new TimeSpan(0, 0, 1);
                infoTimer.Stop();

                try
                {
                    // Get the settings for this application.
                    isolatedStorage = IsolatedStorageSettings.ApplicationSettings;


                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception while using IsolatedStorageSettings: " + e.ToString());
                }


                App.Current.Host.Settings.MaxFrameRate = 30;
                //Application.Current.Host.Settings.EnableFrameRateCounter = true;

                Debug.WriteLine("Acceleration: " + App.Current.Host.Settings.EnableGPUAcceleration);
                #region Progress Indicator
                SystemTray.SetIsVisible(this, true);
                SystemTray.SetOpacity(this, 0.5);
                SystemTray.SetBackgroundColor(this, Colors.Black);
                SystemTray.SetForegroundColor(this, Colors.White);
                prog = new ProgressIndicator();
                prog.IsIndeterminate = true;
                prog.IsVisible = true;
                //if (Map.IsDownloading)
                //{
                //    prog.IsVisible = true;
                //    SystemTray.SetIsVisible(this, true);
                //}
                //else
                //{
                //    prog.IsVisible = false;
                //    SystemTray.SetIsVisible(this, false);
                //}

                //prog.Text = "Click me...";

                SystemTray.SetProgressIndicator(this, prog);

                #endregion


                _isNewPageInstance = true; // tombstoning

                // Layers for Map
                //_RadiusLayer = new MapLayer();
                //_PushpinLayer = new MapLayer();
                Map.ScaleVisibility = System.Windows.Visibility.Visible;
                Debug.WriteLine("Map height: " + Map.ActualHeightProperty);
                Debug.WriteLine("Map width: " + Map.ActualHeightProperty);
                Debug.WriteLine("Map cache mode: " + Map.CacheMode);
                //Map.Children.Add(_RadiusLayer);
                //Map.Children.Add(_PushpinLayer);
                // Caching to speed up map
                //mapCache = new System.Windows.Media.BitmapCache();
                //Map.CacheMode = mapCache;

                // caching
                Current_Location.CacheMode = new BitmapCache();

                if (Settings.Near_Location == null) Settings.Near_Location = new ObservableCollection<Row>();
                DB_Helper.connect();
                // Data context and observable collection are children of the main page.
                this.DataContext = this;


                //Map.ZoomLevel = 17;
                Current_Location.Style = (Style)(Application.Current.Resources["PushpinStyle"]);


                //Timer for cache the locations from database
                DispatcherTimer tmr = new DispatcherTimer();
                tmr.Interval = TimeSpan.FromMinutes(5);
                tmr.Tick += OnTimerTick;
                tmr.Start();

                //constructor technique
                SolidColorBrush scb = new SolidColorBrush(Colors.Red);
                circle.Fill = scb;

                if (Settings.alert_status == true)
                {
                    alert_stack.Visibility = Visibility.Visible;
                    FadeInOutAnimation().Begin();
                }
                else
                {
                    alert_stack.Visibility = Visibility.Collapsed;
                    FadeInOutAnimation().Stop();
                }

                //Map.Center = new GeoCoordinate(MyLocation.X, MyLocation.Y);
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    MessageBox.Show("Cannot connect to server.");
                }
        }

        #endregion // Intitialization




        #region User Interface

        private TurnstileTransition TurnstileTransitionElement(string mode)
        {
            TurnstileTransitionMode turnstileTransitionMode = (TurnstileTransitionMode)Enum.Parse(typeof(TurnstileTransitionMode), mode, false);
            return new TurnstileTransition { Mode = turnstileTransitionMode };
        }


        private TransitionElement TransitionElement(string family, string mode)
        {
            switch (family)
            {
                case "Turnstile":
                    return TurnstileTransitionElement(mode);
            }
            return null;
        }

        public void drawLine(LocationCollection points, Color color, int thickness)
        {
            Debug.WriteLine("drawLine");
            MapPolyline line = new MapPolyline();
            line.StrokeThickness = thickness;
            line.Stroke = new SolidColorBrush(color);
            line.Locations = points;
            line.Visibility = System.Windows.Visibility.Visible;
            this.Map.Children.Add(line);
        }


        //public MapPolyline drawLine(LocationCollection points, Color color, int thickness)
        //{
        //    MapPolyline line = new MapPolyline();
        //    line.StrokeThickness = thickness;
        //    line.Stroke = new SolidColorBrush(color);
        //    line.Locations = points;
        //    line.Visibility = System.Windows.Visibility.Visible;
        //    return line;
        //}


        public LocationCollection drawCircle(GeoCoordinate oLoc, double radius)
        {
            Debug.WriteLine("drawCircle");
            LocationCollection locationCollection = new LocationCollection();
            MapPolygon polygon = new MapPolygon();
            //polygon.CacheMode = new BitmapCache();
            polygon.CacheMode = new BitmapCache { RenderAtScale = .7 };
            polygon.Fill = new SolidColorBrush(Colors.Gray);
            polygon.Fill.Opacity = 0.07;
            polygon.Stroke = new SolidColorBrush(Colors.Black);
            polygon.StrokeThickness = 2.5;
            //polygon.Opacity = dOpacity;
            //this works in miles
            locationCollection = DrawACircle(oLoc, radius);
            polygon.Locations = locationCollection;
            polygon.Visibility = System.Windows.Visibility.Visible;
            //this.Map.Children.Add(polygon);
            //this._RadiusLayer.AddChild(polygon, oLoc, PositionOrigin.Center);
            this.circle_layer.Children.Add(polygon);
            return locationCollection;
        }


        public void drawCircle(LocationCollection locations, Color color)
        {
            Debug.WriteLine("drawCircle");
            LocationCollection locationCollection = new LocationCollection();
            MapPolygon polygon = new MapPolygon();
            //polygon.CacheMode = new BitmapCache();
            polygon.CacheMode = new BitmapCache { RenderAtScale = 0.7 };
            polygon.Fill = new SolidColorBrush(color);
            polygon.Fill.Opacity = 0.07;
            polygon.Stroke = new SolidColorBrush(color);
            polygon.StrokeThickness = 2.5;
            //polygon.Opacity = dOpacity;
            //this works in miles
            //locationCollection = DrawACircle(oLoc, radius);
            polygon.Locations = locations;
            polygon.Visibility = System.Windows.Visibility.Visible;
            //this.Map.Children.Add(polygon);
            this.circle_layer.Children.Add(polygon);
            //this._RadiusLayer.AddChild(polygon, oLoc, PositionOrigin.Center);
            //return locationCollection;
        }

        public LocationCollection drawCircle(GeoCoordinate oLoc, double radius, Color color)
        {
            Debug.WriteLine("drawCircle");
            LocationCollection locationCollection = new LocationCollection();
            MapPolygon polygon = new MapPolygon();
            //polygon.CacheMode = new BitmapCache();
            polygon.CacheMode = new BitmapCache { RenderAtScale=0.7};
            polygon.Fill = new SolidColorBrush(color);
            polygon.Fill.Opacity = 0.07;
            polygon.Stroke = new SolidColorBrush(color);
            polygon.StrokeThickness = 2.5;
            //polygon.Opacity = dOpacity;
            //this works in miles
            locationCollection = DrawACircle(oLoc, radius);
            polygon.Locations = locationCollection;
            polygon.Visibility = System.Windows.Visibility.Visible;
           // this.Map.Children.Add(polygon);
            this.circle_layer.Children.Add(polygon);
            //this._RadiusLayer.AddChild(polygon, oLoc, PositionOrigin.Center);
            return locationCollection;
        }


        public double ToRadian(double x) {
            //Debug.WriteLine("ToRadian");
            return x * (Math.PI / 180);
        }


        public LocationCollection DrawACircle(GeoCoordinate oLoc, double dRadius)
        {
            Debug.WriteLine("DrawACircle");
            LocationCollection oLocs = new LocationCollection();
            double earthRadius = EarthRadiusInMiles;
            double lat = ToRadian(oLoc.Latitude); //radians 
            double lon = ToRadian(oLoc.Longitude); //radians 
            double d = dRadius / earthRadius; // d = angular distance covered on earths surface 
            
            for (int x = 0; x <= 360; x++)
            {
                double brng = ToRadian(x); //radians 
                double latRadians = Math.Asin(Math.Sin(lat) * Math.Cos(d) + Math.Cos(lat) * Math.Sin(d) * Math.Cos(brng));
                double lngRadians = lon + Math.Atan2(Math.Sin(brng) * Math.Sin(d) * Math.Cos(lat), Math.Cos(d) - Math.Sin(lat) * Math.Sin(latRadians));

                GeoCoordinate pt = new GeoCoordinate(); 
                pt.Latitude = (180.0 * latRadians / Math.PI);
                pt.Longitude = (180.0 * lngRadians / Math.PI);
                oLocs.Add(pt);
            }
            return oLocs;
        }


        //public MapPolyline drawLine(double X1, double Y1, double X2, double Y2, Color color, int thickness)
        //{
        //    MapPolyline line = new MapPolyline();
        //    line.Locations.Add(new GeoCoordinate(X1, Y1));
        //    line.Locations.Add(new GeoCoordinate(X2, Y2));
        //    line.StrokeThickness = thickness;
        //    line.Stroke = new SolidColorBrush(color);
        //    line.Visibility = System.Windows.Visibility.Visible;
        //    return line;
        //}


        public void drawLine(double X1, double Y1, double X2, double Y2, Color color, int thickness)
        {
            Debug.WriteLine("drawLine");
            MapPolyline line = new MapPolyline();
            line.Locations.Add(new GeoCoordinate(X1, Y1));
            line.Locations.Add(new GeoCoordinate(X2, Y2));
            line.StrokeThickness = thickness;
            line.Stroke = new SolidColorBrush(color);
            line.Visibility = System.Windows.Visibility.Visible;
            this.Map.Children.Add(line);
        }

        // Create the popup object.
       // Popup p = new Popup();


        private void infoTimer_Tick(object sender, EventArgs e)
        {
            double altitude = watcher.Position.Location.Altitude;
            altitude = Math.Round(altitude, 2);
            double course = watcher.Position.Location.Course;
            course = Math.Round(course, 2);
            double speed = watcher.Position.Location.Speed;
            speed = Math.Round(speed, 2);
            double latitude = watcher.Position.Location.Latitude;
            //latitude = Math.Round(latitude, 2);
            double longitude = watcher.Position.Location.Longitude;
           // longitude = Math.Round(longitude, 2);
            InfoTexBlock.Text = "Latitude: "+ latitude + "\nLongitude: " + longitude +
                "\nAltitude: " + altitude + " meters above sea level\nCourse: " + course + " degrees north\nSpeed: " 
                + speed + " meters per second\n" + CurrentAddress_AddressLine + "\n" + CurrentAddress_Locality + ", " + CurrentAddress_AdminDistrict + " " + CurrentAddress_PostalCode ;

 


            //ReverseGeocodeAddress(
            //     Dispatcher,
            //     _credentialsProvider,
            //     watcher.Position.Location,
            //     result => address = result.Address.FormattedAddress);
        }


        static string CurrentAddress = "";
        static string CurrentAddress_Locality = "";
        static string CurrentAddress_PostalCode = "";
        static string CurrentAddress_AddressLine = "";
        static string CurrentAddress_AdminDistrict = "";
        static string CurrentAddress_CountryRegion = "";




        #region ADDRESSS
        public static void ReverseGeocodeAddress(Dispatcher uiDispatcher, CredentialsProvider credentialsProvider, Location location, Action<GeocodeResult> completed = null, Action<GeocodeError> error = null)
        {
            completed = completed ?? (r => { });
            error = error ?? (e => { });

            // Get credentials and only then place an async call on the geocode service.
            credentialsProvider.GetCredentials(credentials =>
            {
                var request = new ReverseGeocodeRequest()
                {
                    // Pass in credentials for web services call.
                    Credentials = credentials,

                    Culture = CultureInfo.CurrentUICulture.Name,
                    Location = location,

                    // Don't raise exceptions.
                    ExecutionOptions = new UsingBingMaps.Bing.Geocode.ExecutionOptions
                    {
                        SuppressFaults = true
                    },
                };

 

                EventHandler<ReverseGeocodeCompletedEventArgs> reverseGeocodeCompleted = (s, e) =>
                {
                    try
                    {
                        if (e.Result.ResponseSummary.StatusCode != UsingBingMaps.Bing.Geocode.ResponseStatusCode.Success ||
                            e.Result.Results.Count == 0)
                        {
                            // Report geocode error.
                            uiDispatcher.BeginInvoke(() => error(new GeocodeError(e)));
                        }
                        else
                        {
                            // Only report on first result.
                            var firstResult = e.Result.Results.First();
                            uiDispatcher.BeginInvoke(() => completed(firstResult));
                            Debug.WriteLine("street=" + firstResult.Address.AddressLine.ToString());
                            Debug.WriteLine("admin district=" + firstResult.Address.AdminDistrict.ToString());
                            Debug.WriteLine("country region=" + firstResult.Address.CountryRegion.ToString());
                            Debug.WriteLine("district=" + firstResult.Address.District.ToString());
                            Debug.WriteLine("formatted addres=" + firstResult.Address.FormattedAddress.ToString());
                            Debug.WriteLine("locality=" + firstResult.Address.Locality.ToString());
                            Debug.WriteLine("postal code=" + firstResult.Address.PostalCode.ToString());
                            Debug.WriteLine("postal town=" + firstResult.Address.PostalTown.ToString());
                            CurrentAddress = firstResult.Address.FormattedAddress.ToString();
                            CurrentAddress_AdminDistrict = firstResult.Address.AdminDistrict.ToString();
                            CurrentAddress_CountryRegion = firstResult.Address.CountryRegion.ToString();
                            CurrentAddress_Locality = firstResult.Address.Locality.ToString();
                            CurrentAddress_PostalCode = firstResult.Address.PostalCode.ToString();
                            CurrentAddress_AddressLine = firstResult.Address.AddressLine.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        uiDispatcher.BeginInvoke(() => error(new GeocodeError(ex.Message, ex)));
                    }
                };

                var geocodeClient = new GeocodeServiceClient();
                geocodeClient.ReverseGeocodeCompleted += reverseGeocodeCompleted;
                geocodeClient.ReverseGeocodeAsync(request);
            });
        }

        #endregion


        private void Info_Click(object sender, EventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                Debug.WriteLine("Info_Click");
                // Add all information about current user or settings ... altitude, longitude, latitude, speed, course
                GeoCoordinate current = new GeoCoordinate(MyLocation.Y, MyLocation.X);
                double altitude = watcher.Position.Location.Altitude;
                altitude = Math.Round(altitude, 2);
                double course = watcher.Position.Location.Course;
                course = Math.Round(course, 2);
                double speed = watcher.Position.Location.Speed;
                speed = Math.Round(speed, 2);
                double latitude = watcher.Position.Location.Latitude;
              //  latitude = Math.Round(latitude, 2);
                double longitude = watcher.Position.Location.Longitude;
               // longitude = Math.Round(longitude, 2);
                // Create some content to show in the popup. Typically you would 
                // create a user control.
                //Border border = new Border();
                //border.BorderBrush = new SolidColorBrush(Colors.Black);
                //border.BorderThickness = new Thickness(5.0);

                //StackPanel panel1 = new StackPanel();
                //panel1.Width = GetScreenSize().Width;
                //panel1.Height = GetScreenSize().Height / 3;
                //panel1.Margin = new Thickness(0);
                //panel1.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                //panel1.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                //panel1.Background = new SolidColorBrush(Colors.LightGray);

                //Button button1 = new Button();
                //button1.Content = "Close";
                //button1.Margin = new Thickness(5.0);
                //button1.Click += new RoutedEventHandler(button1_Click);
                //TextBlock textblock1 = new TextBlock();
                //textblock1.FontSize = 16;
                //textblock1.Text = "Current Altitude: " + current.Altitude + "\nCourse: "  + current.Course + "\nLatitude: " 
                //                    + current.Latitude + "\nLongitude: " + current.Longitude + "\nSpeed: " + current.Speed;
                //textblock1.Margin = new Thickness(5.0);
                //panel1.Children.Add(textblock1);
                //panel1.Children.Add(button1);
                //border.Child = panel1;

                // Set the Child property of Popup to the border 
                // which contains a stackpanel, textblock and button.
                //p.Child = border;

                // Set where the popup will show up on the screen.
                //p.VerticalOffset = 50;
                //p.HorizontalOffset = 0;

                // Open the popup.
                //p.IsOpen = true;

                //MessageBox.Show(  "Current Altitude: " + current.Altitude + "\nCourse: " + current.Course + "\nLatitude: "
                //        + current.Latitude + "\nLongitude: " + current.Longitude + "\nSpeed: " + current.Speed);

                InfoTexBlock.Text = "Latitude: " + latitude + "\nLongitude: " + longitude +
                    "\nAltitude: " + altitude + " meters \nCourse: " + course + " degrees north\nSpeed: " + speed + " meters per second";

                if (InfoStackPanel.Visibility == System.Windows.Visibility.Collapsed)
                {
                    InfoStackPanel.Visibility = System.Windows.Visibility.Visible;
                    InfoTexBlock.Visibility = System.Windows.Visibility.Visible;
                    infoTimer.Start();
                }
                else
                {
                    infoTimer.Stop();
                    InfoStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                    InfoTexBlock.Visibility = System.Windows.Visibility.Visible;
                }
            }
            else
            {
                MessageBox.Show("Cannot Connect To Server.");
            }
        }


        void button1_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("button1_Click");
            // Close the popup.
            //p.IsOpen = false;

        }


        public Size GetScreenSize()
        {
            Debug.WriteLine("GetScreenSize");
            return Application.Current.RootVisual.RenderSize;
        }

        public double getDistance(GeoCoordinate start, GeoCoordinate end)
        {
            Debug.WriteLine("getDistance");
            return start.GetDistanceTo(end);
        }

        /// <summary>
        /// Click event handler for the low accuracy button
        /// </summary>
        /// <param name="sender">The control that raised the event</param>
        /// <param name="e">An EventArgs object containing event data.</param>
        private void LowButtonClick(object sender, EventArgs e)
        {
            Debug.WriteLine("LowButtonClick");
            // Start data acquisition from the Location Service, low accuracy
            //accuracyText = "power optimized";
            StartLocationService(GeoPositionAccuracy.Default);
        }

        /// <summary>
        /// Click event handler for the high accuracy button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HighButtonClick(object sender, EventArgs e)
        {
            Debug.WriteLine("HighButtonClick");
            // Start data acquisition from the Location Service, high accuracy
            //accuracyText = "high accuracy";
            StartLocationService(GeoPositionAccuracy.High);
        }
        private void StopButtonClick(object sender, EventArgs e)
        {
            Debug.WriteLine("StopButtonClick");
            if (watcher != null)
            {
                watcher.Stop();
            }
            //StatusTextBlock.Text = "location service is off";
            //LatitudeTextBlock.Text = " ";
            //LongitudeTextBlock.Text = " ";
        }
        


        private void MainPage_BarMenuItem_Aerial_Click(object sender, EventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                Debug.WriteLine("MainPage_BarMenuItem_Aerial_Click");
                if (Map.Mode is RoadMode)
                {
                    ((ApplicationBarMenuItem)sender).Text = "Aerial View Off";
                    Map.Mode = new AerialMode();
                }
                else
                {
                    ((ApplicationBarMenuItem)sender).Text = "Aerial View On";
                    Map.Mode = new RoadMode();
                }
            }
            else
            {
                MessageBox.Show("Cannot Connect To Server");
            }
        }




        //private void MenuItem_TrackMe_Click(object sender, RoutedEventArgs e)
        //{
        //    Debug.WriteLine("Track me clicked");
        //    if (TrackLocation.Equals(true))
        //    {
        //        TrackLocation = false;

        //        ContextMenu_TrackMe.Header = "Tracker: ON";
        //    }
        //    else
        //    {
        //        TrackLocation = true;
        //        ContextMenu_TrackMe.Header = "Tracker: OFF";
        //    }
        //}

        private void Map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Map_MouseLeftButtonDown");
            SelectedLocation = e.GetPosition(this.Map);
        }

        private void MenuItem_StartLocation_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("MenuItem_StartLocation_Click");
            double x = SelectedLocation.X;
            double y = SelectedLocation.Y;
            StartLocation.X = x;
            StartLocation.Y = y;

            GeoCoordinate geo = new GeoCoordinate();
            geo = Map.ViewportPointToLocation(StartLocation);
            ConvertLocation(geo);

        }

        private void MenuItem_Destination_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("MenuItem_Destination_Click");
            double x = SelectedLocation.X;
            double y = SelectedLocation.Y;
            EndLocation.X = x;
            EndLocation.Y = y;

            GeoCoordinate geo = new GeoCoordinate();
            geo = Map.ViewportPointToLocation(EndLocation);
            ConvertLocation(geo);
        }




       bool _isNewPageInstance = false;
       Color SelectColor(string type)
       {
           Color CircleColor;
           switch (type)
           {
               case "Alarm":
                   CircleColor = COLOR_ALARM;
                   break;
               case "Car Locator":
                   CircleColor = COLOR_CAR_LOCATOR;
                   break;
               case "Ring Tones":
                   CircleColor = COLOR_RING_TONE;
                   break;
               default:
                   CircleColor = Colors.Black;
                   break;
           }
           return CircleColor;
       }

        #endregion // User Interface


        #region Application Logic

        private readonly ObservableCollection<RouteModel> _routes = new ObservableCollection<RouteModel>();
        private FeedBackUtil user_feedback = FeedBackUtil.Instance;
        private bool avoid_double;
        public ObservableCollection<RouteModel> Routes
        {

            get { return _routes; }
        }

        public CredentialsProvider CredentialsProvider
        {
            get { return _credentialsProvider; }
        }



        private void CalculateRouteByAddress( String From, String To)
        {
            Debug.WriteLine("CalculateRouteByAddress");
            try
            {
                var routeCalculator = new RouteCalculator(
                    CredentialsProvider,
                    To,
                    From,
                    Dispatcher,
                    result =>
                    {
                        // Clear the route collection to have only one route at a time.
                        Routes.Clear();

                        // Clear previous route related itineraries.
                        //Itineraries.Clear();

                        // Create a new route based on route calculator result,
                        // and add the new route to the route collection.
                        var routeModel = new RouteModel(result.Result.RoutePath.Points);
                        Routes.Add(routeModel);

                        // Add new route itineraries to the itineraries collection.
                        //foreach (var itineraryItem in result.Result.Legs[0].Itinerary)
                        //{
                        //    Itineraries.Add(itineraryItem);
                        //}

                        // Set the map to center on the new route.
                        var viewRect = LocationRect.CreateLocationRect(routeModel.Locations);
                        Map.SetView(viewRect);

                        //ShowDirectionsView();
                    });

                // Display an error message in case of fault.
                routeCalculator.Error += r => MessageBox.Show(r.Reason);

                // Start the route calculation asynchronously.
                routeCalculator.CalculateAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void CalculateRouteByLocation(Location from, Location to)
        {
            Debug.WriteLine("CalculateRouteByLocation");
                var routeCalculator = new RouteCalculator(
                    CredentialsProvider,
                    to, from,
                    Dispatcher,
                    result =>
                    {
                        // Clear the route collection to have only one route at a time.
                        Routes.Clear();

                        // Clear previous route related itineraries.
                        Settings.Itinerary= new ObservableCollection<ItineraryItem>();

                        // Create a new route based on route calculator result,
                        // and add the new route to the route collection.
                        var routeModel = new RouteModel(result.Result.RoutePath.Points);
                        Routes.Add(routeModel);

                        // Add new route itineraries to the itineraries collection.
                        foreach (var itineraryItem in result.Result.Legs[0].Itinerary)
                        {
                            Settings.Itinerary.Add(itineraryItem);
                        }

                        // Set the map to center on the new route.
                        var viewRect = LocationRect.CreateLocationRect(routeModel.Locations);
                        Map.SetView(viewRect);                        
                        //ShowDirectionsView();
                    });

                // Display an error message in case of fault.
                routeCalculator.Error += r => MessageBox.Show(r.Reason);

                // Start the route calculation asynchronously.
                routeCalculator.CalculateAsync();
        }



        public void ConvertLocation(GeoCoordinate location)
        {
            Debug.WriteLine("ConvertLocation");
            // converts a point to an address
            Debug.WriteLine("ConvertLocation" );
            Debug.WriteLine("GeoCoordinate:" + location.ToString());
            CivicAddressResolver resolver = new CivicAddressResolver();
            CivicAddress d = new CivicAddress();
            if (location.IsUnknown == false)
            {
                CivicAddress address = resolver.ResolveAddress(location);
                Debug.WriteLine("CivicAddress:" + address.ToString());

                if (!address.IsUnknown)
                {
                    Debug.WriteLine("Country: {0}, Zip: {1}",
                            address.CountryRegion,
                            address.PostalCode);
                }
                else
                {
                    Debug.WriteLine("Address unknown.");
                }
            }
        }


        public ObservableCollection<Row> Table
        {
            get
            {
                return _Table;
            }
            set
            {
                if (_Table != value)
                {
                    _Table = value;
                    NotifyPropertyChanged("Table");
                }
            }
        }

        /// <summary>
        /// Helper method to start up the location data acquisition
        /// </summary>
        /// <param name="accuracy">The accuracy level </param>
        private void StartLocationService(GeoPositionAccuracy accuracy)
        {
            Debug.WriteLine("StartLocationService");
            // Reinitialize the GeoCoordinateWatcher
            //StatusTextBlock.Text = "starting, " + accuracyText;
            watcher = new GeoCoordinateWatcher(accuracy);
            watcher.MovementThreshold = 10;

            // Add event handlers for StatusChanged and PositionChanged events
            watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
            watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);

            // Start data acquisition
            watcher.Start();
        }


        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            Debug.WriteLine("watcher_StatusChanged");
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    Debug.WriteLine("disabled");
                    break;
                case GeoPositionStatus.Initializing:
                    Debug.WriteLine("initializing");
                    break;
                case GeoPositionStatus.NoData:
                    Debug.WriteLine("nodata");
                    break;
                case GeoPositionStatus.Ready:
                    Debug.WriteLine("ready");
                    break;
            }
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            string address = "";
            Debug.WriteLine("watcher_PositionChanged");
            ReverseGeocodeAddress(
                 Dispatcher,
                 _credentialsProvider,
                 watcher.Position.Location,
                 result => address = result.Address.FormattedAddress);

            MyLocation.X = e.Position.Location.Longitude;
            MyLocation.Y = e.Position.Location.Latitude;
            speed_text.Text = e.Position.Location.Speed.ToString();
            Debug.WriteLine("({0},{1})", e.Position.Location.Latitude, e.Position.Location.Longitude);

            // Check if tracker is on and center.
            if (TrackLocation==true)
              Map.Center = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);
            //Map.ZoomBarVisibility = System.Windows.Visibility.Visible;

            //---set the location for the Current_Location pushpin---
            Current_Location.Location = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);


            //drawCircle(new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude), 10);
            //drawCircle(new GeoCoordinate(e.Position.Location.Latitude-10, e.Position.Location.Longitude-10), 10);

            //---add the Current_Location pushpin to the map---
            if (Map.Children.Contains(Current_Location) != true)
            {
                Map.Children.Add(Current_Location);
            }
            else
            {
                Map.Children.Remove(Current_Location);
                Map.Children.Add(Current_Location);
            }
            if (gpsReady)
            {
                
                GeoCoordinate current = new GeoCoordinate(MyLocation.Y, MyLocation.X);
                if (Settings.alert_status==true)
                {
                    GeoCoordinate alert = new GeoCoordinate(Settings.Alert.latitude, Settings.Alert.longitude);
                    if ((current.GetDistanceTo(alert) * 0.000621371) > Settings.Alert.Radius)
                    {
                        Alert_Sound.Source = new Uri("/Resource/alarm.wav", UriKind.Relative);
                        Alert_Sound.Play();
                        alert_mode = true;
                    }
                    else
                    {
                        alert_mode = false;
                    }
                }
                if (Settings.Near_Location != null)
                {
                    for (int i = 0; i < Settings.Near_Location.Count(); i++)
                    {
                        GeoCoordinate location = new GeoCoordinate(Settings.Near_Location[i].latitude, Settings.Near_Location[i].longitude);
                        if ((current.GetDistanceTo(location) * 0.000621371) <= Settings.Near_Location[i].Radius)
                        {
                            //In_Area.Add(Settings.Near_Location[i]);
                        }
                    }
                }
            }
            else
            {
                gpsReady = true;
            }
        }






        private void OnTimerTick(object sender, EventArgs e)
        {
            Debug.WriteLine("OnTimerTick");
            Settings.Near_Location = new ObservableCollection<Row>();
            int mRadius = 2; 
            GeoCoordinate temp;
            
            if (Table != null && MyLocation.X != null && MyLocation.Y != null)
            {
                for (int i = 0; i < Table.Count(); i++)
                {
                    temp = new GeoCoordinate(Table[i].latitude, Table[i].longitude);
                    GeoCoordinate current = new GeoCoordinate(MyLocation.Y, MyLocation.X);
                }
            }
        }

        private Storyboard FadeInOutAnimation()
        {
            Debug.WriteLine("FadeInOutAnimation");
            Storyboard sb = new Storyboard();
            DoubleAnimation fadeInAnimation = new DoubleAnimation();
            fadeInAnimation.From = 0.0;
            fadeInAnimation.To = 1.0;
            fadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.0));
            fadeInAnimation.AutoReverse = true;
            Storyboard.SetTarget(fadeInAnimation, this.alert_stack);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity"));           
            sb.Children.Add(fadeInAnimation);
            sb.RepeatBehavior = RepeatBehavior.Forever;
            return sb;
        }

        #endregion // Application Logic


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify Silverlight that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            Debug.WriteLine("NotifyPropertyChanged");
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion //INotifyPropertyChanged Members



        private void Alert_Sound_MediaEnded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Alert_Sound_MediaEnded");
            if (alert_mode == true)
            {
                TimeSpan ts = new TimeSpan(0, 0, 0, 0);
                Alert_Sound.Position = ts;
                Alert_Sound.Play();
            }
        }

        private void findMyCar()
        {
                Debug.WriteLine("findMyCar");
                bool exist = false;
                Location g_from = new Location();
                g_from.Latitude = MyLocation.Y;
                g_from.Longitude = MyLocation.X;
                Location g_to = new Location();

                for (int i = 0; i < Table.Count(); i++)
                {
                    if (Table[i].Type == "Car Locator")
                    {
                        g_to.Longitude = Table[i].longitude;
                        g_to.Latitude = Table[i].latitude;
                        exist = true;
                        break;
                    }
                }
                if (exist == true)
                {
                    show_route = true;
                    CalculateRouteByLocation(g_from, g_to);
                }
                else
                {
                    MessageBox.Show("Please set the location of your car.");
                }
        }

        private void findMyCar_Click(object sender, EventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (watcher.Status == GeoPositionStatus.Disabled)
                {
                    MessageBox.Show("Please Turn Location Service On");
                }
                else
                {
                    Debug.WriteLine("findMyCar_Click");
                    if (show_route == false)
                    {
                        findMyCar();
                        if (show_route == true)
                            ((ApplicationBarMenuItem)sender).Text = "Show Directions";
                    }
                    else
                    {
                        NavigationService.Navigate(new Uri("/Car_Direction.xaml", UriKind.RelativeOrAbsolute));
                    }
                }
            }
            else
            {
                MessageBox.Show("Cannot Connect To Server.");
            }
        }





        #region Navigation

        private void MainPage_BarMenuItem_CarLocator_Click(object sender, EventArgs e)
        {
            this.Navigate_To_Bookmarks();

        }
        public void Navigate_To_Bookmarks()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (watcher.Status == GeoPositionStatus.Disabled)
                {
                    MessageBox.Show("Please Turn Location Service On");
                }
                else
                {
                    Debug.WriteLine("MainPage_BarMenuItem_CarLocator_Click");
                    NavigationService.Navigate(new Uri("/New.xaml?from=button" + "&longitude=" + MyLocation.X + "&latitude=" + MyLocation.Y, UriKind.RelativeOrAbsolute));
                }
            }
            else
            {
                MessageBox.Show("Cannot Connect To Server");
            }

        }

        private void DeviceStatus_Click(object sender, EventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                Debug.WriteLine("DeviceStatus_Click");
                NavigationService.Navigate(new Uri("/DeviceStatusPage.xaml", UriKind.RelativeOrAbsolute));
            }
            else
            {
                MessageBox.Show("Cannot Connect To Server.");
            }
        }

        private void MenuItem_CarLocation_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("MenuItem_CarLocation_Click");
            //  Point p = e.GetPosition(this.Map);
            GeoCoordinate geo = new GeoCoordinate();
            geo = Map.ViewportPointToLocation(SelectedLocation);

            NavigationService.Navigate(new Uri("/New.xaml?from=map&page=new" + "&longitude=" + geo.Longitude + "&latitude=" + geo.Latitude, UriKind.RelativeOrAbsolute));

        }

        void Pushpin_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Pushpin_MouseLeftButtonUp");
            Pushpin temp = (Pushpin)sender;
            String title = temp.Content.ToString();
            NavigationService.Navigate(new Uri("/New.xaml?from=map&page=edit&title=" + title, UriKind.RelativeOrAbsolute));
        }

        private void List_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("List_Click");
            NavigationService.Navigate(new Uri("/List.xaml", UriKind.RelativeOrAbsolute));
        }
        private void Alert_Click(object sender, EventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (watcher.Status == GeoPositionStatus.Disabled)
                {
                    MessageBox.Show("Please Turn Location Service On");
                }
                else
                {
                    Debug.WriteLine("Alert_Click");
                    NavigationService.Navigate(new Uri("/Alert.xaml?longitude=" + MyLocation.X.ToString() + "&latitude=" + MyLocation.Y, UriKind.RelativeOrAbsolute));
                }
            }
            else
            {
                MessageBox.Show("Cannot Connect To Server.");
            }
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                Debug.WriteLine("Settings_Click");
                NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.RelativeOrAbsolute));
            }
            else
            {
                MessageBox.Show("Cannot Connect To Server.");
            }
        }
        private void Share_Click(object sender, EventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (watcher.Status == GeoPositionStatus.Disabled)
                {
                    MessageBox.Show("Please Turn Location Service On");
                }
                else
                {
                    //Debug.WriteLine("Share_Click");
                    //NavigationService.Navigate(new Uri("/Share.xaml?StartLocationX=" + StartLocation.X + "&StartLocationY=" + StartLocation.Y
                    //                                             + "&EndLocationX=" + EndLocation.X + "&EndLocationY=" + EndLocation.Y
                    //                                             + "&MyLocationX=" + MyLocation.X + "&MyLocationY=" + MyLocation.Y
                    //, UriKind.RelativeOrAbsolute));
                    Debug.WriteLine("Share_Click");
                    NavigationService.Navigate(new Uri("/Share.xaml?Address=" + CurrentAddress + "&MyLocationX=" + MyLocation.X + "&MyLocationY=" + MyLocation.Y
                    , UriKind.RelativeOrAbsolute));
                }
            }
            else
            {
                MessageBox.Show("Cannot Connect To Server");
            }
        }


        [DataMember]
        LocationCollection storedPolygon;
        //MapPolygon storedPolygon;



        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            Debug.WriteLine("OnNavigatedTo");
            #region RestorePageState
            if (_isNewPageInstance == true)
            {
                Debug.WriteLine("State count: " + State.Count.ToString() );
                if (State.Count > 0)
                {
                    // assign saved stats to local variables
                    if ( ! State["StartLocation"].Equals(null) )
                        StartLocation = (System.Windows.Point)State["StartLocation"];

                    if ( ! State["EndLocation"].Equals(null) )
                        EndLocation = (System.Windows.Point)State["EndLocation"];
                    
                    if( ! State["MyLocation"].Equals(null) )
                        MyLocation = (System.Windows.Point)State["MyLocation"];

                    //  SelectedLocation=(Point) State["SelectedLocation"];
                    if( ! State["watcher"].Equals(null) )
                        watcher = (GeoCoordinateWatcher) State["watcher"];

                    //  Current_Location = (Pushpin) State["Current_Location"];
                    if( ! State["longitude"].Equals(null) )
                        longitude = (double)State["longitude"];

                    if ( ! State["latitude"].Equals(null) )
                        latitude = (double) State["latitude"];

                    if ( ! State["TrackLocation"].Equals(null) )
                        TrackLocation =(Boolean)State["TrackLocation"];
                    //  tmr = (DispatcherTimer) State["tmr"];

                    if ( ! State["gpsReady"].Equals(null) )
                        gpsReady = (bool)State["gpsReady"];

                    if( ! State["alert_mode"].Equals(null) )
                        alert_mode = (Boolean)State["alert_mode"];
                    //  _Table = (ObservableCollection<Row>)State["_Table"];

                    if( ! State["To"].Equals(null) )
                        To = (string)State["To"];

                    if( ! State["From"].Equals(null) )
                        From = (string)State["From"];
                    //Map = (Map)State["Map"];
                }
                else
                {
                    // new instance of each variable
                    //     Current_Location = new Pushpin();
                    //     TrackLocation = true;
                    //     gpsReady = false;
                    //     alert_mode=false;
                }
            }
            base.OnNavigatedTo(e);
            _isNewPageInstance = false;
            #endregion


            try
            {
                // Get the settings for this application.
                isolatedStorage = IsolatedStorageSettings.ApplicationSettings;
            }
            catch (Exception err)
            {
                Debug.WriteLine("Exception while using IsolatedStorageSettings: " + err.ToString());
            }

            #region Settings

            if (isolatedStorage.Contains("MapCenterLocation"))
            {
                Map.Center = (GeoCoordinate)isolatedStorage["MapCenterLocation"];
            }
            else
            {
                // need current location
            }

            if (isolatedStorage.Contains("MapZoomLevel"))
            {
                Map.ZoomLevel = (double)isolatedStorage["MapZoomLevel"];
            }
            else
            {
                Map.ZoomLevel = 17;
            }

           if (isolatedStorage.Contains("AerialView"))
            {
                bool view = (bool)isolatedStorage["AerialView"];
                if (view == true)
                {

                    Map.Mode = new AerialMode();
                }
                else
                {
                    Map.Mode = new RoadMode();
                }
            }
            else
            {
                Map.Mode = new RoadMode();
            }



            if (isolatedStorage.Contains("RadiusDisplay"))
            {

                if (isolatedStorage["RadiusDisplay"].Equals(true))
                {
                    circle_layer.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    circle_layer.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            else
            {
                circle_layer.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (isolatedStorage.Contains("TrackPosition"))
            {
                if (isolatedStorage["TrackPosition"].Equals(true))
                {
                    TrackLocation = true;
                }
                else
                {
                    TrackLocation = false;
                }
            }
            else
            {
                TrackLocation = true;
            }
            #endregion


            #region Trial Mode Check
            //if (App.IsTrial == true)
            //{
            //    // enable ads
            //    adControl.Visibility = System.Windows.Visibility.Visible;
            //    //adControl.IsAutoRefreshEnabled = true;
            //    adControl.IsEnabled = true;
            //    adControl.IsAutoCollapseEnabled = false;
            //}
            //else
            //{
            //    // disables ads
            //    adControl.Visibility = System.Windows.Visibility.Collapsed;
            //    //adControl.IsAutoRefreshEnabled = false;
            //    adControl.IsEnabled = false;
            //    adControl.IsAutoCollapseEnabled = false;
            //}
            #endregion

            if (watcher != null)
            {
                StartLocationService(GeoPositionAccuracy.High);
                //watcher.Start();
            }
            else if (watcher == null)
            {
                //---get the highest accuracy---
                StartLocationService(GeoPositionAccuracy.High);
                //watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High)
                //{
                //    //---the minimum distance (in meters) to travel before the next 
                //    // location update---
                //    MovementThreshold = 10
                //};

                //---event to fire when a new position is obtained---
                watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);

                //---event to fire when there is a status change in the location 
                // service API---
                watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
                watcher.Start();

            }

            
           // Map.Center = new GeoCoordinate( watcher.Position.Location.Longitude,  watcher.Position.Location.Latitude);


            Table = DB_Helper.getAllRows();


            // draw all layers before pushpin!!
            //for (int i = 0; i < Table.Count(); i++)
            //{
            //    Color CircleColor = SelectColor(Table[i].Type);
            //    if (Table[i].Radius > 0)
            //    {
            //        drawCircle(new GeoCoordinate(Table[i].latitude, Table[i].longitude), Table[i].Radius, CircleColor);
            //    }
            //}


            //string[] ids = new string[Table.Count()];
            //// remove stale database data from isolated storage
            //for (int j = 0; j < Table.Count(); j++)
            //{
            //    //store id info
            //    ids[j] = "id=" + Table[j].Id;
            //}
            //// compare each id to isolated storage and delete the one that dont match

            //for (int k = 0; k < isolatedStorage.Count(); k++)
            //{

            //    for (int j = 0; j < ids.Count(); j++)
            //    {
            //        if (ids[j] == isolatedStorage.ElementAt(k).Key)
            //        {
            //            //do nthong
            //        }
            //        else
            //        {
            //            //remove stale data
            //            string stale = isolatedStorage.ElementAt(k).Key;
            //            isolatedStorage.Remove(stale);
            //        }
            //    }
            //}



            // DRAW RADII
            // Color for Radius
            Color CircleColor = SelectColor("Alert");

            if (Settings.alert_status == true)
            {
                GeoCoordinate alert = new GeoCoordinate(Settings.Alert.latitude, Settings.Alert.longitude);
                drawCircle(new GeoCoordinate(Settings.Alert.latitude, Settings.Alert.longitude), Settings.Alert.Radius, CircleColor);
            }


            // List<Pushpin> pushpin = new List<Pushpin>();
            for (int i = 0; i < Table.Count(); i++)
            {
                CircleColor = SelectColor(Table[i].Type);
                if (Table[i].Radius > 0)
                {
                    string tableKey = "id=" + Table[i].Id;
                    Debug.WriteLine(tableKey);


                        if (Table[i].Type == "Ring Tones" || Table[i].Type == "Alarm" )
                        {
                            if (isolatedStorage.Contains("RadiusDisplay")) 
                                if( isolatedStorage["RadiusDisplay"].Equals(true))
                                    drawCircle(new GeoCoordinate(Table[i].latitude, Table[i].longitude), Table[i].Radius, CircleColor);
                            #region Isolated Storage of Circles

                            //if (isolatedStorage.Contains(tableKey))
                            //{
                            //    Debug.WriteLine("Isolated Storage Contains Key = " + tableKey);
                            //    storedPolygon = (LocationCollection)isolatedStorage[tableKey];
                            //    // If the value has changed
                            //    if (isolatedStorage[tableKey] != (LocationCollection)storedPolygon)
                            //    {
                            //        Debug.WriteLine("Isolated Storage Value Has Changed, Key = " + tableKey);
                            //        // Store the new value, draw cirlce
                            //        storedPolygon = (LocationCollection)drawCircle(new GeoCoordinate(Table[i].latitude, Table[i].longitude), Table[i].Radius, CircleColor);
                            //        isolatedStorage[tableKey] = (LocationCollection)storedPolygon;
                            //    }
                            //    else
                            //    {
                            //        Debug.WriteLine("Isolated Storage Key Exists, Key = " + tableKey);
                            //        // add existing circle
                            //        drawCircle(storedPolygon, CircleColor);
                            //        // Map.Children.Add(storedPolygon);
                            //    }
                            //}
                            //// Otherwise create the key.
                            //else
                            //{
                            //    Debug.WriteLine("Isolated Storage Key Doesn't Exist, Key =" + tableKey);
                            //    storedPolygon = (LocationCollection)drawCircle(new GeoCoordinate(Table[i].latitude, Table[i].longitude), Table[i].Radius, CircleColor);
                            //    isolatedStorage.Add(tableKey, (LocationCollection)storedPolygon);
                            //}
                            //Debug.WriteLine("Isolated Storage Saved.");
                            //isolatedStorage.Save();
                            #endregion
                        }
                }

                longitude = Table[i].longitude;
                latitude = Table[i].latitude;
                Pushpin pushpin1 = new Pushpin();
                pushpin1.CacheMode = new BitmapCache();
                pushpin1.Content = Table[i].Title;
                pushpin1.Location = new GeoCoordinate(latitude, longitude);

                //Image image = new Image(); 
                //image.Source = new BitmapImage(new Uri("Images/PushpinCar.png", UriKind.RelativeOrAbsolute));
                //image.Width = 40;
                //image.Height = 40;
               

                Debug.WriteLine("Type: " + Table[i].Type);
                switch (Table[i].Type)
                {
                    case "Alarm":
                        Debug.WriteLine("PushPin Style = AlarmPin");
                        pushpin1.Style = (Style)(Application.Current.Resources["AlarmPin"]);
                        //image.Source = new BitmapImage(new Uri("Images/PushpinHouse.png", UriKind.RelativeOrAbsolute));
                        break;
                    case "Car Locator":
                        Debug.WriteLine("PushPin Style = CarPin");
                        pushpin1.Style = (Style)(Application.Current.Resources["CarPin"]);
                        //image.Source = new BitmapImage(new Uri("Images/PushpinCar.png", UriKind.RelativeOrAbsolute));
                        break;
                    case "Ring Tones":
                        Debug.WriteLine("PushPin Style = RingtonePin");
                        pushpin1.Style = (Style)(Application.Current.Resources["RingtonePin"]);
                        //image.Source = new BitmapImage(new Uri("Images/PushpinLocation.png", UriKind.RelativeOrAbsolute));
                        break;
                    default:
                        Debug.WriteLine("PushPin Style = Pin");
                        //image.Source = new BitmapImage(new Uri("Images/PushpinShop.png", UriKind.RelativeOrAbsolute));
                        pushpin1.Style = (Style)(Application.Current.Resources["Pin"]);
                        break;
                }

               // pushpin1.Style = (Style)(Application.Current.Resources["Pin"]);
                pushpin1.MouseLeftButtonUp += new MouseButtonEventHandler(Pushpin_MouseLeftButtonUp);
                Map.Children.Add(pushpin1);
                //_PushpinLayer.AddChild(image, pushpin1.Location);
            }
            OnTimerTick(new Object(), new EventArgs());
        }



        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            Debug.WriteLine("OnNavigatedFrom");
            #region SaveState
            if (StartLocation != null)
                State["StartLocation"] = StartLocation;
            if (EndLocation != null)
                State["EndLocation"] = EndLocation;
            if (MyLocation != null)
                State["MyLocation"] = MyLocation;
            //State["SelectedLocation"] = SelectedLocation;
            if (watcher != null)
                State["watcher"] = watcher; 
            //State["Current_Location"] = Current_Location;
            if (longitude != null)
                State["longitude"] = longitude;
            if (latitude != null)
                State["latitude"] = latitude;
            if (TrackLocation != null)
                State["TrackLocation"] = TrackLocation;
            ////State["tmr"] = tmr;
            if (gpsReady != null)
                State["gpsReady"] = gpsReady;
            if (alert_mode != null)
                State["alert_mode"] = alert_mode;
            ////State["_Table"] = _Table;
            if (To != null)
                State["To"] = To;
            if (From != null)
                State["From"] = From;
            //State["Map"] = Map;
 // assign saved stats to local variables

            isolatedStorage["MapCenterLocation"] = Map.Center;
            isolatedStorage["MapZoomLevel"] = Map.ZoomLevel; 
            isolatedStorage.Save();

            #endregion
            if (watcher.Status == GeoPositionStatus.Ready)
            {
                watcher.Stop();
            }
            // Call the base method.
            base.OnNavigatedFrom(e);
        }
        #endregion // Navigation

        private void Map_MapPan(object sender, MapDragEventArgs e)
        {
            Debug.WriteLine("Map_MapPan");
            if (Map.IsDownloading)
            {
                prog.IsVisible = true;
                SystemTray.SetIsVisible(this, true);                
            }
            else
            {
                prog.IsVisible = false;
                SystemTray.SetIsVisible(this, false);
            }
        }

        private void Map_MapResolved(object sender, EventArgs e)
        {
            Debug.WriteLine("Map_MapResolved");
            if (Map.IsDownloading)
            {
                prog.IsVisible = true;
                SystemTray.SetIsVisible(this, true);
            }
            else
            {
                prog.IsVisible = false;
                SystemTray.SetIsVisible(this, false);
            }
        }

        private void FindMe_Click(object sender, EventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (watcher.Status == GeoPositionStatus.Disabled)
                {
                    MessageBox.Show("Please Turn Location Service On");
                }
                else
                {
                    Map.Center = new GeoCoordinate(watcher.Position.Location.Latitude, watcher.Position.Location.Longitude);
                }
            }
            else
            {
                MessageBox.Show("Cannot Connect To Server");
            }
        }

        private void Help_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/HelpPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Test_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AccessibleGPS.xaml", UriKind.RelativeOrAbsolute));
        }

        private void ImageToMap_Click(object sender, RoutedEventArgs e)
        {
            user_feedback.Speak("Image To Map");

        }
        private void LocationBookmarks_Click(object sender, RoutedEventArgs e)
        {
            user_feedback.Speak("Location Bookmarks");
        }

        private void HelpMe_Click(object sender, RoutedEventArgs e)
        {
            if (avoid_double == false)
            {
                user_feedback.VibrateOnItemSelected();
                user_feedback.Speak("When you press on an option the functionality is spoken to you. To Select an option double tap, the phone will vibrate twice to confirm selection.....The Image to Map Option... will allow you to snap a picture of a map and retrieve directions from your current location..." + "- The Image Take Me Home Options...will display directions to your home from your current location...." + "- The Location Bookmarks Option..allow you to bookmark locations and display directions to your bookmarks.");
            }
            else avoid_double = false;
        }
        private void TakeMeHome_Click(object sender, RoutedEventArgs e)
        {
            user_feedback.Speak("Take Me Home");
        }

        private void ImageToMap_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            user_feedback.VibrateOnItemSelected();
            NavigationService.Navigate(new Uri("/OcrMainPage.xaml", UriKind.RelativeOrAbsolute));

        }

        private void TakeMeHome_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            user_feedback.VibrateOnItemSelected();

        }

        private void LocationBookmarks_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {

            this.findMyCar();
            user_feedback.VibrateOnItemSelected();
            this.Navigate_To_Bookmarks();

        }

        private void HelpMe_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            avoid_double = true;
  
        }





 






    } // Class5
} // Namespace