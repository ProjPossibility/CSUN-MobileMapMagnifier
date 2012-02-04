//#define DEBUG_AGENT
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
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System.Device.Location;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;

namespace ScheduledTaskAgent1
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>


        GeoCoordinateWatcher watcher;

        private ObservableCollection<Row> _Table;
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
                }
            }
        }
        protected override void OnInvoke(ScheduledTask task)
        {
            Boolean inside = false;
            Debug.WriteLine("OnInvoke");
            //Start watcher
            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High)
            {
                MovementThreshold = 10
            };
            watcher.Start();

            //TODO: Add code to perform your task in background
            string toastMessage = "";
            GeoCoordinate temp;

            // If your application uses both PeriodicTask and ResourceIntensiveTask
            // you can branch your application code here. Otherwise, you don't need to.
            if (task is PeriodicTask)
            {
                Debug.WriteLine("PeriodicTask");
                GeoCoordinate current = watcher.Position.Location;
                Debug.WriteLine("Current Location : "+current.Longitude + " " + current.Latitude);
                DB_Helper.connect();
                Table = DB_Helper.getAllRows();

                String message = "Change ringtone to ";
                Boolean first_circle = true;
                for (int i = 0; i < Table.Count(); i++)
                {
                    Debug.WriteLine("Start working with Database");
                    if (Table[i].Type == "Ring Tones")
                    {
                        temp = new GeoCoordinate(Table[i].latitude, Table[i].longitude);
                        Double distance = current.GetDistanceTo(temp) * 0.000621371;
                        if (distance <= Table[i].Radius)
                        {
                            Debug.WriteLine("Inside a circle");
                            Debug.WriteLine("Ring_tones is " + Table[i].Ring_Tones);
                            inside = true;
                            if (first_circle == true)
                            {
                                message += Table[i].Ring_Tones;
                                first_circle = false;
                            }
                            else
                            {
                                message += " or " + Table[i].Ring_Tones;
                            }
                        }
                    }
                }
                // Execute periodic task actions here.
                if (inside == true)
                {
                    toastMessage = message;
                    Debug.WriteLine("Toast is displayed");
                }
            }
            else
            {
                Debug.WriteLine("Resource-Intensive Task");
                GeoCoordinate current = watcher.Position.Location;
                DB_Helper.connect();
                Table = DB_Helper.getAllRows();

                String message = "Change ringtone to ";
                Boolean first_circle = true;
                for (int i = 0; i < Table.Count(); i++)
                {
                    Debug.WriteLine("Start working with Database");
                    if (Table[i].Type == "Ring Tones")
                    {
                        temp = new GeoCoordinate(Table[i].latitude, Table[i].longitude);
                        Double distance = current.GetDistanceTo(temp) * 0.000621371;
                        if (distance <= Table[i].Radius)
                        {
                            Debug.WriteLine("Inside a circle");
                            Debug.WriteLine("Ring_tones is " + Table[i].Ring_Tones);
                            inside = true;
                            if (first_circle == true)
                            {
                                message += Table[i].Ring_Tones;
                                first_circle = false;
                            }
                            else
                            {
                                message += " or " + Table[i].Ring_Tones;
                            }
                        }
                    }
                }
                // Execute periodic task actions here.
                if (inside == true)
                {
                    toastMessage = message;
                    Debug.WriteLine("Toast is displayed");
                }
            }
             if (inside == true)
             {
                // Launch a toast to show that the agent is running.
                // The toast will not be shown if the foreground application is running.
                ShellToast toast = new ShellToast();
                toast.Title = "GPS";
                toast.Content = toastMessage;
                toast.Show();
             }

            // If debugging is enabled, launch the agent again in one minute.
#if DEBUG_AGENT
            ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(60));
#endif

            // Call NotifyComplete to let the system know the agent is done working.
            NotifyComplete();
        }
    }
}

