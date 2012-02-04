using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.IO;


namespace GPS
{
    public class MainViewModel : INotifyPropertyChanged
    {
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
                    NotifyPropertyChanged("Table");
                }
            }
        }


        public MainViewModel()
        {
            this.List_Items = new ObservableCollection<ItemViewModel>();
            GPS.DB_Helper.connect();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ItemViewModel> List_Items { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        public bool IsDataLoaded
        {
            get;
            set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            // Sample data; replace with real data
            /*this.Items.Add(new ItemViewModel() { LineOne = "runtime one", LineTwo = "Maecenas praesent accumsan bibendum", LineThree = "Facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu" });
            this.Items.Add(new ItemViewModel() { LineOne = "runtime two", LineTwo = "Dictumst eleifend facilisi faucibus", LineThree = "Suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus" });
            
        */
            this.List_Items.Clear();

            Visibility darkBackgroundVisibility =(Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"];
            Table =DB_Helper.getAllRows();
            // List<Pushpin> pushpin = new List<Pushpin>();
            for (int i = 0; i < Table.Count(); i++)
            {
                if (darkBackgroundVisibility == Visibility.Visible)
                {
                    this.List_Items.Add(new ItemViewModel() { LineOne = Table[i].Title, LineTwo = Table[i].Radius.ToString(), LineThree = Table[i].Icon_Source_Dark, LineFour = Table[i].Ring_Tones });
                }
                else
                {
                    this.List_Items.Add(new ItemViewModel() { LineOne = Table[i].Title, LineTwo = Table[i].Radius.ToString(), LineThree = Table[i].Icon_Source_Light, LineFour = Table[i].Ring_Tones });
                }
            }
            this.IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}