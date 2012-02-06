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
using Microsoft.Phone.Tasks;
using GPS;
using Microsoft.Phone.Tasks;
using Coding4Fun.Phone.Controls.Data;

namespace UsingBingMaps
{
    public partial class HelpPage : PhoneApplicationPage
    {
        MarketplaceDetailTask _marketPlaceDetailTask = new MarketplaceDetailTask();
        MarketplaceReviewTask _marketplaceReview = new MarketplaceReviewTask();
        MarketplaceSearchTask _marketplaceSearch = new MarketplaceSearchTask();
        EmailComposeTask _emailComposeTask = new EmailComposeTask();

        public HelpPage()
        {
            InitializeComponent();

            txtAppName.Text = PhoneHelper.GetAppAttribute("Title") + " by " + PhoneHelper.GetAppAttribute("Author");
            txtVersion.Text = "version " + PhoneHelper.GetAppAttribute("Version");
            txtDescription.Text = PhoneHelper.GetAppAttribute("Description");
        }

        private void btnBuyApp_Click(object sender, RoutedEventArgs e)
        {
            _marketPlaceDetailTask.Show();
        }


        private void btnMarketplace_Click(object sender, RoutedEventArgs e)
        {
          //  _marketplaceSearch.Show();
        }

        private void btnReview_Click(object sender, RoutedEventArgs e)
        {
            _marketplaceReview.Show();
        }

        private void btnContact_Click(object sender, RoutedEventArgs e)
        {
            _emailComposeTask.To = "ss12@hotmail.com";
            _emailComposeTask.Subject = "Map Magnifier Feedback";
            _emailComposeTask.Show();
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

        }


    }
}