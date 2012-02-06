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
using wpSpeech;
using UsingBingMaps.Ocr;
using Microsoft.Devices;

namespace GPS
{
    public partial class Page1 : PhoneApplicationPage
    {

        private FeedBackUtil user_feedback = FeedBackUtil.Instance;
       
        
        public Page1()
        {
            InitializeComponent();
            this.MainIntro();
            

            
        }

 
        private void MainIntro()
        {
            user_feedback.Speak("Main Menu");
        }
        private void TakeMeHome_Click(object sender, RoutedEventArgs e)
        {
            user_feedback.Speak("Take Me Home");

        }
        private void ImgToGPS_Click(object sender, RoutedEventArgs e)
        {
            user_feedback.Speak("Image To GPS");
        }
        private void LocationBookmarks_Click(object sender, RoutedEventArgs e)
        {
            user_feedback.Speak("Location Bookmarks");
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            user_feedback.Speak("Settings");
        }

        private void HelpMe_Click(object sender, RoutedEventArgs e)
        {
            user_feedback.Speak("Help Me");
        }


        private void TakeMeHome_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
          //  tts.Speak("Selected Take Me Home");
            user_feedback.VibrateOnItemSelected();
        }

        private void LocationBookmarks_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
          //  tts.Speak("Selected Location Booksmarks");
            user_feedback.VibrateOnItemSelected();
          //  NavigationService.Navigate(new Uri("/New.xaml?from=button" + "&longitude=" + MyLocation.X + "&latitude=" + MyLocation.Y, UriKind.RelativeOrAbsolute));
        }

        private void Settings_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
          //  tts.Speak("Selected Settings");
            user_feedback.VibrateOnItemSelected();
        }

        private void HelpMe_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            user_feedback.VibrateOnItemSelected();
          

            //Tap on different areas of the screen, when you hear an option you would like to execute
            //long press that area.
            //
           // tts.Speak("Tap ");

        }

        private void ImgToGPS_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
           // tts.Speak("Selected Image To GPS");
            //vibrate.Start(TimeSpan.FromMilliseconds(1000));

            user_feedback.VibrateOnItemSelected();
            System.Threading.Thread.Sleep(500);
       //     user_feedback.Speak("Camera loaded, Tap on screen to snap picture");
            System.Threading.Thread.Sleep(2000);
            CameraOcr cameraOcr = new CameraOcr();
    
            cameraOcr.init();
            cameraOcr.getPhotoStream();
            //tts.Speak("Im back");
        }

        private void ImgToGPS_GotFocus(object sender, RoutedEventArgs e)
        {
          //  tts.Speak("FOCUS LOL");
        }

    }
}