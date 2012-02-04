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
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Collections.ObjectModel;


namespace ScheduledTaskAgent1
{
    [Table]
    public class Row : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // Define ID: private field, public property and database column.
        private int _Id;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    NotifyPropertyChanging("Id");
                    _Id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }
        // Define item name: private field, public property and database column.
        private string _Title;

        [Column]
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                if (_Title != value)
                {
                    NotifyPropertyChanging("Title");
                    _Title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }
        // Define item name: private field, public property and database column.
        private string _Type;

        [Column]
        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                if (_Type != value)
                {
                    NotifyPropertyChanging("Type");
                    _Type = value;
                    NotifyPropertyChanged("Type");
                }
            }
        }

        // Define item name: private field, public property and database column.
        private string _Ring_Tones;

        [Column]
        public string Ring_Tones
        {
            get
            {
                return _Ring_Tones;
            }
            set
            {
                if (_Ring_Tones != value)
                {
                    NotifyPropertyChanging("Ring_Tones");
                    _Ring_Tones = value;
                    NotifyPropertyChanged("Ring_Tones");
                }
            }
        }

        // Define item name: private field, public property and database column.
        private double _longitude;

        [Column]
        public double longitude
        {
            get
            {
                return _longitude;
            }
            set
            {
                if (_longitude != value)
                {
                    NotifyPropertyChanging("longitude");
                    _longitude = value;
                    NotifyPropertyChanged("longitude");
                }
            }
        }

        // Define item name: private field, public property and database column.
        private double _latitude;

        [Column]
        public double latitude
        {
            get
            {
                return _latitude;
            }
            set
            {
                if (_latitude != value)
                {
                    NotifyPropertyChanging("latitude");
                    _latitude = value;
                    NotifyPropertyChanged("latitude");
                }
            }
        }

        // Define item name: private field, public property and database column.
        private double _Radius;

        [Column]
        public double Radius
        {
            get
            {
                return _Radius;
            }
            set
            {
                if (_Radius != value)
                {
                    NotifyPropertyChanging("Radius");
                    _Radius = value;
                    NotifyPropertyChanged("Radius");
                }
            }
        }

        // Define item name: private field, public property and database column.
        private string _Note;

        [Column]
        public string Note
        {
            get
            {
                return _Note;
            }
            set
            {
                if (_Note != value)
                {
                    NotifyPropertyChanging("Note");
                    _Note = value;
                    NotifyPropertyChanged("Note");
                }
            }
        }

        // Define completion value: private field, public property and database column.
        private bool _isComplete;

        [Column]
        public bool IsComplete
        {
            get
            {
                return _isComplete;
            }
            set
            {
                if (_isComplete != value)
                {
                    NotifyPropertyChanging("IsComplete");
                    _isComplete = value;
                    NotifyPropertyChanged("IsComplete");
                }
            }
        }
        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary _version;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the page that a data context property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify the data context that a data context property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
        public class GPSDataContext : DataContext
        {
            // Specify the connection string as a static, used in main page and app.xaml.
            public static string DBConnectionString = "Data Source=isostore:/Database.sdf";

            // Pass the connection string to the base class.
            public GPSDataContext(string connectionString)
                : base(connectionString)
            { }

            // Specify a single table for the to-do items.
            public Table<Row> Rows;
        }

    }
}
