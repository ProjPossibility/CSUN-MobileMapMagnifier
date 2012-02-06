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
using Microsoft.Phone.Tasks;
using System.IO;
using GPS;

namespace UsingBingMaps.Ocr
{
    public class CameraOcr : ICamercaOcr
    {
        private CameraCaptureTask cameraCaptureTask;

        private Stream photoStream;
      //  private TTSHelper tts = new TTSHelper();

        public void init()
        {
            this.cameraCaptureTask = new CameraCaptureTask();

            this.cameraCaptureTask.Completed += new System.EventHandler<PhotoResult>(this.PhotoChooserCompleted);

            this.cameraCaptureTask.Show();
           
            //tts.Speak("Camera loaded, Tap on screen to take picture");
        }

        private void PhotoChooserCompleted(object sender, PhotoResult e)
        {
            //this.showInProgress = false;
            if (e.TaskResult == TaskResult.OK)
            {

                // Defensive. This should not happen unless maybe someone programmatically saved a 0 length image in 
                // the picture gallery which I am not even sure the phone API will allow. 
                // Since this.ocrData.PhotoStream will not accept a zero length stream we'll act defensively here.
                if (e.ChosenPhoto.Length > 0)
                {
                                      
                    // This is the point where we have a photo available. 
                    photoStream = e.ChosenPhoto;
                }
            }
        }

        public Stream getPhotoStream()
        {
            return photoStream;   
        }
    }

}
