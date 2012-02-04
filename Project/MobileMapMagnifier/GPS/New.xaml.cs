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
using System.Collections.ObjectModel;
using Microsoft.Phone.Shell;
using System.ComponentModel;

namespace GPS
{
    public partial class New : PhoneApplicationPage
    {
         #region Variables

        double longitude;
        double latitude;
        Boolean new_page = false;
        String old_title="";
        String old_radius="";
        String old_type="";
        String old_ringtones = "";
        String old_note = "";

        // Define an observable collection property that controls can bind to.
        private ObservableCollection<Row> _Table;
        private ObservableCollection<Row> _Edit_Query;

        #endregion // Variables


        #region Initialization

        public New()
        {
            InitializeComponent();

            // Data context and observable collection are children of the main page.
            this.DataContext = this;

            this.Type.ItemsSource = new List<string>() { "Car Locator" };
            this.Ring_Tones.ItemsSource = new List<string>() { "Ring", "Vibrate", "Ring and Vibrate", "Slient" };
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(List_Loaded);            
        }

        #endregion // Initialization


        #region Application Logic

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
        public ObservableCollection<Row> Edit_Query
        {
            get
            {
                return _Edit_Query;
            }
            set
            {
                if (_Edit_Query != value)
                {
                    _Edit_Query = value;
                    NotifyPropertyChanged("Edit_Query");
                }
            }
        }
        #endregion // Application Logic


        #region User Interface


        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Call the base method.
            base.OnNavigatedFrom(e);

            //NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
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

            Table = DB_Helper.getAllRows();
            if (NavigationContext.QueryString["from"] == "map")
            {
                Location.Visibility = Visibility.Collapsed;
                Location_Text.Visibility = Visibility.Collapsed;
                useMyLocation.Visibility = Visibility.Collapsed;
                if (NavigationContext.QueryString["page"] == "new")
                {
                    longitude = Double.Parse(NavigationContext.QueryString["longitude"]);
                    latitude = Double.Parse(NavigationContext.QueryString["latitude"]);
                    new_page = true;
                    Title_New.Visibility = Visibility.Visible;
                }
                else
                {
                    Title_Edit.Visibility = Visibility.Visible;
                    String title = NavigationContext.QueryString["title"];
                    Edit_Query = DB_Helper.getRowsbyTitle(title);
                    Title.Text = title;
                    Radius.Text= Edit_Query[0].Radius.ToString();
                    Type.SelectedItem = Edit_Query[0].Type;
                    if(Edit_Query[0].Ring_Tones!="") Ring_Tones.SelectedItem = Edit_Query[0].Ring_Tones;
                    latitude = Edit_Query[0].latitude;
                    longitude = Edit_Query[0].longitude;
                    Note.Text = Edit_Query[0].Note;

                    old_title = title;
                    old_radius = Edit_Query[0].Radius.ToString();
                    old_type = Edit_Query[0].Type;
                    old_ringtones = Edit_Query[0].Ring_Tones;
                    old_note = Edit_Query[0].Note;
                    change_UI(Edit_Query[0].Type);
                }
            }
            else
            {
                new_page = true;
                Title_New.Visibility = Visibility.Visible;
                longitude = Double.Parse(NavigationContext.QueryString["longitude"]);
                latitude = Double.Parse(NavigationContext.QueryString["latitude"]);
            }
            if (new_page)
            {
                var b = (ApplicationBarIconButton)ApplicationBar.Buttons[2]; //indx based!
                b.IsEnabled = false;
            }
            // Call the base method.
            base.OnNavigatedTo(e);
        }

        private void Save_Click(object sender, EventArgs e)
        {
            String temp_Radius = "0";
            String temp_Note = "";
            String temp_RingTones = "";
            String temp_icon_light = "";
            String temp_icon_dark = "";
            String error = "";
            if (Title.Text == ""&&error=="") error = "Title";

            // check if radius contains more than one "."
            int count=0;
            if (Radius.Visibility == Visibility.Visible && error==""){
                for (int i = 0; i < Radius.Text.Count(); i++ )
                {
                    if (Radius.Text.ElementAt(i).Equals('.'))
                    {
                        count++;
                    }
                }
                if (count > 1){
                    error = "Radius";
                } else if (Radius.Text == "." || Radius.Text=="")
                {
                    error = "Radius";
                } else if (Convert.ToDouble(Radius.Text) > 0)
                {
                    // do nothing, radius is greater than 0
                }
                else
                {
                    error = "Radius";
                }
            }

            if (Radius.Visibility == Visibility.Visible && (Radius.Text == "" || Radius.Text == "0" || Radius.Text =="." || Radius.Text =="0.0" || Radius.Text=="0." || Radius.Text == ".0") && error == "") error = "Radius";
            if (Note.Visibility == Visibility.Visible && Note.Text == "" && error == "") error = "Note";
            if (Location.Visibility == Visibility.Visible)
            {
                if (Location.Text == "" && error == "") error = "Location";
            }
            
            if (error!="")
            {
                if (error != "Radius")
                {
                    MessageBox.Show(error + " cannot be empty ", "", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show(error + " cannot be empty or 0", "", MessageBoxButton.OK);
                }
            }
            else
            {
                if (Radius.Visibility == Visibility.Visible) temp_Radius = Radius.Text;
                if (Ring_Tones.Visibility == Visibility.Visible) temp_RingTones = (String)Ring_Tones.SelectedItem;
                if (Note.Visibility == Visibility.Visible) temp_Note = Note.Text;                
                switch ((String)Type.SelectedItem)
                   {
                       case "Alarm":
                           temp_icon_dark = "/Images/Mode.png/";
                           temp_icon_light = "/Images/Mode_Dark.png/";
                           break;
                       case "Car Locator":
                           temp_icon_dark = "/Images/PushpinCar.png/";
                           temp_icon_light = "/Images/PushpinCar_Dark.png/";
                           break;
                       case "Ring Tones":
                           temp_icon_dark = "/Images/Location.png/";
                           temp_icon_light = "/Images/Location_Dark.png/";
                           break;
                       default:
                           temp_icon_dark = "/Images/PushpinCar.png/";
                           temp_icon_light = "/Images/PushpinCar_Dark.png/";
                           break;
                   }
                if (!new_page)//Edit Page
                {
                    if ((old_title != Title.Text) || (old_radius != temp_Radius) || (old_ringtones != temp_RingTones) || (old_type != (String)Type.SelectedItem) || (old_note !=temp_Note))
                    {
                        Table.Remove(Edit_Query[0]);
                        DB_Helper.deleteRow(Edit_Query[0]);

                        // Create a new to-do item based on the text box.
                        Row newRow = new Row { Title = Title.Text, longitude = longitude, latitude = latitude, Radius = Double.Parse(temp_Radius), Ring_Tones = temp_RingTones, Type = (String)Type.SelectedItem, Note = temp_Note, Icon_Source_Dark = temp_icon_dark, Icon_Source_Light = temp_icon_light };

                        // Add a to-do item to the observable collection.
                        Table.Add(newRow);

                        DB_Helper.insertRow(newRow);
                        NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
                    }
                    else
                    {
                        NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
                    }
                }
                else// New Page
                {
                    if (DB_Helper.getRowsbyTitle(Title.Text).Count!=0)
                    {
                        MessageBox.Show("The title is already used ","", MessageBoxButton.OK);
                    }
                    else
                    {
                        Row newRow = new Row { Title = Title.Text, longitude = longitude, latitude = latitude, Radius = Double.Parse(temp_Radius), Ring_Tones = temp_RingTones, Type = (String)Type.SelectedItem, Note = temp_Note, Icon_Source_Dark = temp_icon_dark, Icon_Source_Light = temp_icon_light };

                        Table.Add(newRow);
                        DB_Helper.insertRow(newRow);

                        NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
                    }
                }
            }
        }


        private void Cancel_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }


        private void useMyLocation_Checked(object sender, RoutedEventArgs e)
        {
            Location.IsEnabled = false;
        }

        private void useMyLocation_Unchecked(object sender, RoutedEventArgs e)
        {
            Location.IsEnabled = true;
        }
                
        #endregion // User Interface


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify Silverlight that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion // INotifyPropertyChanged Members

        private void Delete_Click(object sender, EventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure?", "Delete", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                Table.Remove(Edit_Query[0]); 
                DB_Helper.deleteRow(Edit_Query[0]);
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
            }

        }
        // Load data for the ViewModel Items
        private void List_Loaded(object sender, RoutedEventArgs e)
        {
            App.ViewModel.LoadData();
            //if (!App.ViewModel.IsDataLoaded)
            //{
            //    App.ViewModel.LoadData();
            //}
        }
        private void lb_personal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ItemViewModel item = lb_personal.SelectedItem as ItemViewModel;
            //MessageBox.Show(title.LineOne);
            if (item != null)
            {
                NavigationService.Navigate(new Uri("/New.xaml?from=map&page=edit&title=" + item.LineOne, UriKind.RelativeOrAbsolute));
            }
        }

        private void Radius_GotFocus(object sender, RoutedEventArgs e)
        {
            
        }

        private void Title_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                if (Location.Visibility == System.Windows.Visibility.Collapsed)
                {
                    Radius.Focus();
                }
                else
                {
                    Location.Focus();
                }  
            }
        }

        private void Location_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                Radius.Focus();
            }
        }

        private void Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            change_UI((String)Type.SelectedItem);
        }
        void change_UI(String type)
        {
            if (type == "Car Locator")
            {
                Radius.Visibility = Visibility.Collapsed;
                Ring_Tones.Visibility = Visibility.Collapsed;
                Radius_Text.Visibility = Visibility.Collapsed;
                RingTones_Text.Visibility = Visibility.Collapsed;
                Note.Visibility = Visibility.Collapsed;
                Note_Text.Visibility = Visibility.Collapsed;
            }
            else if (type == "Alarm")
            {
                Radius.Visibility = Visibility.Visible;
                Ring_Tones.Visibility = Visibility.Collapsed;
                Radius_Text.Visibility = Visibility.Visible;
                RingTones_Text.Visibility = Visibility.Collapsed;
                Note.Visibility = Visibility.Visible;
                Note_Text.Visibility = Visibility.Visible;
            }
            else
            {
                Radius.Visibility = Visibility.Visible;
                Ring_Tones.Visibility = Visibility.Visible;
                Radius_Text.Visibility = Visibility.Visible;
                RingTones_Text.Visibility = Visibility.Visible;
                Note.Visibility = Visibility.Collapsed;
                Note_Text.Visibility = Visibility.Collapsed;
            }
        }

        private void Radius_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}