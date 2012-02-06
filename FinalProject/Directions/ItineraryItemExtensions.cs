using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UsingBingMaps.Bing.Route;

namespace GPS
{
    public static class ItineraryItemExtensions
    {
        public static ObservableCollection<ItineraryItemDisplay> ToDisplay(this IEnumerable<ItineraryItem> items)
        {
            // New display list.
            var displayItems = new ObservableCollection<ItineraryItemDisplay>();

            // Convert each item.
            for (int i = 0; i < items.Count(); i++)
            {
                var currentItem = items.ElementAt(i);

                // New display item.
                var displayItem = new ItineraryItemDisplay();
                displayItem.Index = i + 1;
                displayItem.Distance = currentItem.Summary.Distance;
                displayItem.TotalSeconds = currentItem.Summary.TimeInSeconds;
                displayItem.Text = currentItem.Text;

                // Add to list.
                displayItems.Add(displayItem);
            }

            return displayItems;
        }
    }
}
