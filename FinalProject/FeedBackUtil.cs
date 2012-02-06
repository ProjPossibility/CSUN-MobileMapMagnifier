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
using wpSpeech;
using Microsoft.Devices;

namespace GPS
{
    public class FeedBackUtil
    {
        private static SpeechTTS tts;
        private static string _AppID = "00EFD818BED4355748D0D1AF0F19932E119F1E80";
        private static VibrateController vibrate = VibrateController.Default;


        private static FeedBackUtil instance;

        private FeedBackUtil() { }


        public static FeedBackUtil Instance
        {
            get 
              {
                 if (instance == null)
                 { 
                     tts = new SpeechTTS(_AppID);
                     tts.LoadLanguageCodes();
                     tts.SpeakLanguage = "en";
                     instance = new FeedBackUtil();
                 }
                 return instance;
              }
           
        }
        public void Speak(string textToSpeak)
        {
            tts.SpeakText(textToSpeak);
        }
        public void VibrateOnItemSelected()
        {
            vibrate.Start(TimeSpan.FromMilliseconds(200));
            System.Threading.Thread.Sleep(500);
            vibrate.Start(TimeSpan.FromMilliseconds(200));
            System.Threading.Thread.Sleep(500);
        }

    }
}
