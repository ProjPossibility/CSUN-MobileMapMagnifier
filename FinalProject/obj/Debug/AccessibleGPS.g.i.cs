﻿#pragma checksum "C:\Users\vic\Documents\Visual Studio 2010\Projects\GPS(1)\GPS\AccessibleGPS.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "266F672DB66A37A0A83EAD6FF113C770"
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
    
    
    public partial class Page1 : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.Button ImgToGPS;
        
        internal System.Windows.Controls.Button TakeMeHome;
        
        internal System.Windows.Controls.Button Settings;
        
        internal System.Windows.Controls.Button LocationBookmarks;
        
        internal System.Windows.Controls.Button HelpMe;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/GPS;component/AccessibleGPS.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.ImgToGPS = ((System.Windows.Controls.Button)(this.FindName("ImgToGPS")));
            this.TakeMeHome = ((System.Windows.Controls.Button)(this.FindName("TakeMeHome")));
            this.Settings = ((System.Windows.Controls.Button)(this.FindName("Settings")));
            this.LocationBookmarks = ((System.Windows.Controls.Button)(this.FindName("LocationBookmarks")));
            this.HelpMe = ((System.Windows.Controls.Button)(this.FindName("HelpMe")));
        }
    }
}
