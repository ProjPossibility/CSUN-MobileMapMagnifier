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

namespace GPS
{
    public partial class Share : PhoneApplicationPage
    {
        Point StartLocation;
        Point EndLocation;
        Point MyLocation;
        string Address="";
        public Share()
        {
            InitializeComponent();

  
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
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
            Address = (NavigationContext.QueryString["Address"]);
            //StartLocation.X = Double.Parse(NavigationContext.QueryString["StartLocationX"]);
            //StartLocation.Y = Double.Parse(NavigationContext.QueryString["StartLocationY"]);
            //EndLocation.X = Double.Parse(NavigationContext.QueryString["EndLocationX"]);
            //EndLocation.Y = Double.Parse(NavigationContext.QueryString["EndLocationY"]);
            MyLocation.X = Double.Parse(NavigationContext.QueryString["MyLocationX"]);
            MyLocation.Y = Double.Parse(NavigationContext.QueryString["MyLocationY"]);
        }
        private void SMS_Click(object sender, EventArgs e)
        {
            SmsComposeTask sms = new SmsComposeTask();
            //sms.To = "0123456789";
            //if (StartLocation != null)
            //{
            //    sms.Body = "Start Location: X:" + StartLocation.X + " Y: " + StartLocation.Y + "\n";
            //}
            //if (EndLocation != null)
            //{
            //    sms.Body = sms.Body + "End Location: X:" + EndLocation.X + " Y: " + EndLocation.Y + "\n";
            //}
            if (Address != "")
            {
                sms.Body = "My current location: " + Address + "\n";
            }
            if (MyLocation != null)
            {
                sms.Body = sms.Body + "Latitude:" + MyLocation.X + " Longitude: " + MyLocation.Y + "\n";
            }
            sms.Show();
        }

        private void Email_Click(object sender, EventArgs e)
        {
            EmailComposeTask email = new EmailComposeTask();
            email.Subject = "Map Magnifier";
            //if (StartLocation != null)
            //{
            //    email.Body = "Start Location: X:" + StartLocation.X + " Y: " + StartLocation.Y + "\n";
            //}
            //if (EndLocation != null)
            //{
            //    email.Body = email.Body + "End Location: X:" + EndLocation.X + " Y: " + EndLocation.Y + "\n";
            //}
            if (Address != "")
            {
                email.Body = "My current location: " + Address + "\n";
            }
            if (MyLocation != null)
            {
                email.Body = email.Body + "Latitude:" + MyLocation.X + " Longitude: " + MyLocation.Y + "\n";
            }
            email.Show();
        }
    }
}