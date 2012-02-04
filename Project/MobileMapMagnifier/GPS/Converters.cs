using System;
using System.Windows.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Phone.Controls.Maps.Platform;
using UsingBingMaps.Bing.Route;

namespace GPS
{
    public class LocationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Location)
            {
                return (value as Location).ToCoordinate();
            }
            else if (value is IEnumerable<Location>)
            {
                return (value as IEnumerable<Location>).ToCoordinates();
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ItineraryTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var textBuilder = new StringBuilder();
            string validXmlText = string.Format("<Directives xmlns:VirtualEarth=\"http://BingMaps\">{0}</Directives>", value);
            XDocument.Parse(validXmlText).Elements().Select(e => e.Value).ToList().ForEach(v => textBuilder.Append(v));
            return textBuilder.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    //Time
    public class TotalSecondsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            long seconds = (long)value;
            if (seconds > 60)
            {
                return String.Format("{0} min", Math.Round((double)(seconds / 60), 0));
            }
            else
            {
                return String.Format("{0} sec", seconds);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    //Distance 
    public class KilometersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double distance = (double)value;
            if (distance < 1)
            {
                return String.Format("{0} m", distance * 1000);
            }
            else
            {
                return String.Format("{0} km", Math.Round(distance, 1));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
