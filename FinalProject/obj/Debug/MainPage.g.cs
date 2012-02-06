﻿#pragma checksum "C:\Users\vic\Documents\Visual Studio 2010\Projects\GPS(1)\GPS\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "0898257802F9ECB471A5BEF7EB5DC70F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace GPS {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal Microsoft.Phone.Controls.PhoneApplicationPage Main;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal Microsoft.Phone.Controls.Maps.Map Map;
        
        internal Microsoft.Phone.Controls.Maps.MapLayer circle_layer;
        
        internal Microsoft.Phone.Controls.Maps.MapLayer InfoLayer;
        
        internal System.Windows.Controls.StackPanel InfoStackPanel;
        
        internal System.Windows.Controls.TextBlock InfoTexBlock;
        
        internal System.Windows.Controls.StackPanel alert_stack;
        
        internal System.Windows.Shapes.Ellipse circle;
        
        internal System.Windows.Controls.TextBlock alert_text;
        
        internal System.Windows.Controls.TextBlock speed_text;
        
        internal System.Windows.Controls.StackPanel stackPanel1;
        
        internal System.Windows.Controls.Button ImageToMap;
        
        internal System.Windows.Controls.Button TakeMeHome;
        
        internal System.Windows.Controls.Button LocationBookmarks;
        
        internal System.Windows.Controls.Button HelpMe;
        
        internal System.Windows.Controls.MediaElement Alert_Sound;
        
        internal Microsoft.Phone.Shell.ApplicationBarMenuItem where_s_my_car;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/GPS;component/MainPage.xaml", System.UriKind.Relative));
            this.Main = ((Microsoft.Phone.Controls.PhoneApplicationPage)(this.FindName("Main")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.Map = ((Microsoft.Phone.Controls.Maps.Map)(this.FindName("Map")));
            this.circle_layer = ((Microsoft.Phone.Controls.Maps.MapLayer)(this.FindName("circle_layer")));
            this.InfoLayer = ((Microsoft.Phone.Controls.Maps.MapLayer)(this.FindName("InfoLayer")));
            this.InfoStackPanel = ((System.Windows.Controls.StackPanel)(this.FindName("InfoStackPanel")));
            this.InfoTexBlock = ((System.Windows.Controls.TextBlock)(this.FindName("InfoTexBlock")));
            this.alert_stack = ((System.Windows.Controls.StackPanel)(this.FindName("alert_stack")));
            this.circle = ((System.Windows.Shapes.Ellipse)(this.FindName("circle")));
            this.alert_text = ((System.Windows.Controls.TextBlock)(this.FindName("alert_text")));
            this.speed_text = ((System.Windows.Controls.TextBlock)(this.FindName("speed_text")));
            this.stackPanel1 = ((System.Windows.Controls.StackPanel)(this.FindName("stackPanel1")));
            this.ImageToMap = ((System.Windows.Controls.Button)(this.FindName("ImageToMap")));
            this.TakeMeHome = ((System.Windows.Controls.Button)(this.FindName("TakeMeHome")));
            this.LocationBookmarks = ((System.Windows.Controls.Button)(this.FindName("LocationBookmarks")));
            this.HelpMe = ((System.Windows.Controls.Button)(this.FindName("HelpMe")));
            this.Alert_Sound = ((System.Windows.Controls.MediaElement)(this.FindName("Alert_Sound")));
            this.where_s_my_car = ((Microsoft.Phone.Shell.ApplicationBarMenuItem)(this.FindName("where_s_my_car")));
        }
    }
}

