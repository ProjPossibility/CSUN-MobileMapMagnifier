//#define DEBUG_AGENT
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
using Microsoft.Phone.Scheduler;
using System.Diagnostics;
using Microsoft.Phone.Tasks;
using GPS;
        using System.IO.IsolatedStorage;
        
namespace GPS
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        IsolatedStorageSettings isolatedStorage;

        // Declare the MarketplaceDetailTask object with page scope
        // so we can access it from event handlers.
        MarketplaceDetailTask _marketPlaceDetailTask = new MarketplaceDetailTask();

        public SettingsPage()
        {
            InitializeComponent();

            Debug.WriteLine("SettingsPage");

        }

        PeriodicTask periodicTask;
        ResourceIntensiveTask resourceIntensiveTask;

        string periodicTaskName = "PeriodicAgent";
        string resourceIntensiveTaskName = "ResourceIntensiveAgent";


        private void StartPeriodicAgent()
        {
            try
            {
                Debug.WriteLine("StartPeriodicAgent");
                periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

                // If the task already exists and the IsEnabled property is false, background
                // agents have been disabled by the user
                if (periodicTask != null && !periodicTask.IsEnabled)
                {
                    MessageBox.Show("Background agents for this application have been disabled by the user.");
                    return;
                }

                // If the task already exists and background agents are enabled for the
                // application, you must remove the task and then add it again to update 
                // the schedule
                if (periodicTask != null && periodicTask.IsEnabled)
                {
                    RemoveAgent(periodicTaskName);
                }

                periodicTask = new PeriodicTask(periodicTaskName);

                // The description is required for periodic agents. This is the string that the user
                // will see in the background services Settings page on the device.
                periodicTask.Description = "This demonstrates a periodic task.";

                ScheduledActionService.Add(periodicTask);

                PeriodicStackPanel.DataContext = periodicTask;

                // If debugging is enabled, use LaunchForTest to launch the agent in one minute.

#if(DEBUG_AGENT)
                Debug.WriteLine("Start LaunchForTest");
                ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(5));
#endif
            }
            catch (Exception e)
            {
                if (e.Message.Contains("BNS Error: The action is disabled"))
                {
                    MessageBox.Show("The background engine has been disabled. Re-enable the background service in settings > background tasks.");
                }
                else
                {
                    Debug.WriteLine("Error!! ooops " + e.StackTrace.ToString());
                }
            }
        }

        private void StartResourceIntensiveAgent()
        {
            Debug.WriteLine("StartResourceIntensiveAgent");
            resourceIntensiveTask = ScheduledActionService.Find(resourceIntensiveTaskName) as ResourceIntensiveTask;

            // If the task already exists and the IsEnabled property is false, background
            // agents have been disabled by the user
            if (resourceIntensiveTask != null && !resourceIntensiveTask.IsEnabled)
            {
                MessageBox.Show("Background agents for this application have been disabled by the user.");
                return;
            }

            // If the task already exists and background agents are enabled for the
            // application, you must remove the task and then add it again to update 
            // the schedule
            if (resourceIntensiveTask != null && resourceIntensiveTask.IsEnabled)
            {
                RemoveAgent(resourceIntensiveTaskName);
            }

            resourceIntensiveTask = new ResourceIntensiveTask(resourceIntensiveTaskName);
            // The description is required for periodic agents. This is the string that the user
            // will see in the background services Settings page on the device.

            resourceIntensiveTask.Description = "This demonstrates a resource-intensive task.";
            ScheduledActionService.Add(resourceIntensiveTask);

            ResourceIntensiveStackPanel.DataContext = resourceIntensiveTask;

            // If debugging is enabled, use LaunchForTest to launch the agent in one minute.
#if(DEBUG_AGENT)
            
            Debug.WriteLine("Start LaunchForTest");
            ScheduledActionService.LaunchForTest(resourceIntensiveTaskName, TimeSpan.FromSeconds(5));
#endif
        }

        private void RemoveAgent(string name)
        {
            Debug.WriteLine("RemoveAgent");
            try
            {
                ScheduledActionService.Remove(name);
            }
            catch (Exception)
            {
            }
        }

        bool ignoreCheckBoxEvents = false;

        private void PeriodicCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (Background_Service != null)
            {
                isolatedStorage["BackgroundService"] = Background_Service.IsChecked;
                Debug.WriteLine("BackgroundService = " + Background_Service.IsChecked.ToString());
            }
            if (ignoreCheckBoxEvents)
                return;
            StartPeriodicAgent();
        }

        private void PeriodicCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Background_Service != null)
            {
                isolatedStorage["BackgroundService"] = Background_Service.IsChecked;
                Debug.WriteLine("BackgroundService = " + Background_Service.IsChecked.ToString());
            }
            if (ignoreCheckBoxEvents)
                return;
            RemoveAgent(periodicTaskName);
        }

        private void ResourceIntensiveCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (ignoreCheckBoxEvents)
                return;
            StartResourceIntensiveAgent();
        }

        private void ResourceIntensiveCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ignoreCheckBoxEvents)
                return;
            RemoveAgent(resourceIntensiveTaskName);
        }


        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {

            if (Background_Service != null)
            {
                isolatedStorage["BackgroundService"] = Background_Service.IsChecked;
                Debug.WriteLine("BackgroundService = " + Background_Service.IsChecked.ToString());
            }

            if (RadiusDisplayCheckBox != null)
            {
                isolatedStorage["RadiusDisplay"] = RadiusDisplayCheckBox.IsChecked;
                Debug.WriteLine("RadiusDisplay = " + RadiusDisplayCheckBox.IsChecked.ToString());
            }

            if (TrackPositionCheckBox != null)
            {
                isolatedStorage["TrackPosition"] = TrackPositionCheckBox.IsChecked;
                Debug.WriteLine("TrackPosition = " + TrackPositionCheckBox.IsChecked.ToString());
            }

            if (AerialViewToggle  != null)
            {
                isolatedStorage["AerialView"] = AerialViewToggle.IsChecked;
                Debug.WriteLine("AerialView = " + AerialViewToggle.IsChecked.ToString());
            }
            
            isolatedStorage.Save();
            base.OnNavigatedFrom(e);
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
            //    btnBuyApp.IsEnabled = true;
            //    btnBuyApp.Visibility = System.Windows.Visibility.Visible;
            //}
            //else
            //{
            //    // disables ads
            //    adControl.Visibility = System.Windows.Visibility.Collapsed;
            //    //adControl.IsAutoRefreshEnabled = false;
            //    adControl.IsEnabled = false;
            //    adControl.IsAutoCollapseEnabled = false;
            //    btnBuyApp.IsEnabled = false;
            //    btnBuyApp.Visibility = System.Windows.Visibility.Collapsed;
            //}
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

            if (isolatedStorage.Contains("BackgroundService"))
            {
                Background_Service.IsChecked = (bool)isolatedStorage["BackgroundService"];
                Debug.WriteLine("BackgroundService = " + Background_Service.IsChecked.ToString());
            }

            if (isolatedStorage.Contains("RadiusDisplay"))
            {
                RadiusDisplayCheckBox.IsChecked = (bool)isolatedStorage["RadiusDisplay"];
                Debug.WriteLine("RadiusDisplay = " + RadiusDisplayCheckBox.IsChecked.ToString());
            }


            if (isolatedStorage.Contains("TrackPosition"))
            {
                TrackPositionCheckBox.IsChecked = (bool)isolatedStorage["TrackPosition"];
                Debug.WriteLine("TrackPosition = " + TrackPositionCheckBox.IsChecked.ToString());
            }


            if (isolatedStorage.Contains("AerialView"))
            {
                AerialViewToggle.IsChecked = (bool)isolatedStorage["AerialView"];
                Debug.WriteLine("AerialView = " + AerialViewToggle.IsChecked.ToString());
            }

            ignoreCheckBoxEvents = true;

            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            if (periodicTask != null)
            {
                PeriodicStackPanel.DataContext = periodicTask;
            }

            resourceIntensiveTask = ScheduledActionService.Find(resourceIntensiveTaskName) as ResourceIntensiveTask;
            if (resourceIntensiveTask != null)
            {
                ResourceIntensiveStackPanel.DataContext = resourceIntensiveTask;
            }

            ignoreCheckBoxEvents = false;

        }

        
        private void btnBuyApp_Click(object sender, RoutedEventArgs e)
        {
            _marketPlaceDetailTask.Show();
        }

        private void RadiusDisplayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (RadiusDisplayCheckBox != null)
            {
                isolatedStorage["RadiusDisplay"] = RadiusDisplayCheckBox.IsChecked;
                Debug.WriteLine("RadiusDisplay = " + RadiusDisplayCheckBox.IsChecked.ToString());
            }
        }

        private void RadiusDisplayCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (RadiusDisplayCheckBox != null)
            {
                isolatedStorage["RadiusDisplay"] = RadiusDisplayCheckBox.IsChecked;
                Debug.WriteLine("RadiusDisplay = " + RadiusDisplayCheckBox.IsChecked.ToString());
            }
        }

        private void TrackPositionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (TrackPositionCheckBox != null) { 
                isolatedStorage["TrackPosition"] = TrackPositionCheckBox.IsChecked;
                Debug.WriteLine("TrackPosition = " + TrackPositionCheckBox.IsChecked.ToString());
            }
        }

        private void TrackPositionCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (TrackPositionCheckBox != null)
            {
                isolatedStorage["TrackPosition"] = TrackPositionCheckBox.IsChecked;
                Debug.WriteLine("TrackPosition = " + TrackPositionCheckBox.IsChecked.ToString());
            }
        }

        private void AerialViewToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (AerialViewToggle != null)
            {
                isolatedStorage["AerialView"] = AerialViewToggle.IsChecked;
                Debug.WriteLine("AerialView = " + AerialViewToggle.IsChecked.ToString());
            }
        }

        private void AerialViewToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (AerialViewToggle != null)
            {
                isolatedStorage["AerialView"] = AerialViewToggle.IsChecked;
                Debug.WriteLine("AerialView = " + AerialViewToggle.IsChecked.ToString());
            }
        }


    }
}