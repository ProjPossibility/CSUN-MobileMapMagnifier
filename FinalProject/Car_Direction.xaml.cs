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
using System.Windows.Data;
using UsingBingMaps.Bing.Route;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml.Linq;

namespace GPS
{
    public partial class Car_Direction : PhoneApplicationPage
    {
        public Car_Direction()
        {
            InitializeComponent();
            //ItemsSource="{Binding Settings.Itinerary, Converter={StaticResource itineraryConverter}}" 
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {


            ObservableCollection<ItineraryItemDisplay> items = ItineraryItemExtensions.ToDisplay(Settings.Itinerary);
            direction.ItemsSource = items;
        }       
    }
    
}