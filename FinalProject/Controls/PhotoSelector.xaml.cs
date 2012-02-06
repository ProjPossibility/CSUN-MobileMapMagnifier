// -
// <copyright file="PhotoSelector.xaml.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -
using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using GPS.Data;
using GPS.Utils;
using GPS.ViewModels;

namespace GPS.Controls
{
    /// <summary>
    /// PhotoSelector class contains the code behind for the PhotoSelector user control.
    /// The PhotoSelector user control allows the user to select the method of retriving 
    /// a photo: capture from the camera or load from the camera roll.
    /// </summary>
    public partial class PhotoSelector : UserControl
    {
        /// <summary>
        /// Indicates that the size of the photo has to be reduced (scaled-down)
        /// to a certain limit before sending it over the wire.
        /// </summary>
        private const bool DoLimitPhotoSize = true;

        /// <summary>
        /// The diagonal of the scaled-down size.
        /// </summary>
        private const double PhotoMaxSizeDiagonal = 600;

        // A reference to the OcrData instance that stores the 
        // photo stream and the text obtained after the OCR conversion.
        private OcrData ocrData;

        // cameraCaptureTask is needed to show the Camera Capture Task of the WP7 OS.
        private CameraCaptureTask cameraCaptureTask;

        // photoChooserTask is needed to show the Photo Chooser Task of the WP7 OS.
        private PhotoChooserTask photoChooserTask;

        private bool showInProgress;
        private FeedBackUtil user_feedback = FeedBackUtil.Instance;
        /// <summary>
        /// Initializes a new instance of the PhotoSelector class.
        /// </summary>
        public PhotoSelector()
        {
            InitializeComponent();

            this.ocrData = OcrData.Instance;
            this.cameraCaptureTask = new CameraCaptureTask();
            this.photoChooserTask = new PhotoChooserTask();

            this.cameraCaptureTask.Completed += new System.EventHandler<PhotoResult>(this.PhotoChooserCompleted);
            this.photoChooserTask.Completed += new EventHandler<PhotoResult>(this.PhotoChooserCompleted);
        }

        private void TakePicture_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            user_feedback.Speak("Use camera to take map picture");
        }

        private void OpenPicture_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            user_feedback.Speak("Choose picture from storage");
        }

        private void PhotoChooserCompleted(object sender, PhotoResult e)
        {
           
            this.showInProgress = false;
            if (e.TaskResult == TaskResult.OK)
            {
                // Defensive. This should not happen unless maybe someone programmatically saved a 0 length image in 
                // the picture gallery which I am not even sure the phone API will allow. 
                // Since this.ocrData.PhotoStream will not accept a zero length stream we'll act defensively here.
                if (e.ChosenPhoto.Length > 0)
                {
                    Stream photoStream;

                    // This is the point where we have a photo available. 
                    photoStream = e.ChosenPhoto;

                    // Extract the orientation flag from the photo before we do the scaling down. 
                    // If the scaling down is performed we'll no longer have the Exif info in the photo stream.
                    ExifUtils exifUtils = new ExifUtils(OcrClientUtils.GetPhotoBits(photoStream));
                    this.ocrData.ExifOrientationFlag = exifUtils.GetOrientationFlag();

                    if (DoLimitPhotoSize)
                    {
                        photoStream = OcrClientUtils.LimitPhotoSize(photoStream, PhotoMaxSizeDiagonal);
                    }

                    // When setting this.ocrData.PhotoStream, the ocrData instance will notify anyone who subscribed
                    // to its PropertyChanged event. One of the subscribers to PropertyChanged will see that 
                    // the PhotoStream became available and it will trigger the OCR conversion.
                    this.ocrData.PhotoStream = photoStream;
                }
            }
        }

        private void Image_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            user_feedback.VibrateOnItemSelected();

            // Make sure that we don't call this.cameraCaptureTask.Show() a second time before the first Show returned. 
            // This may happend if we click really fast the btnTakePicture button. If that happens the second Show will 
            // throw an InvalidOperationException exception.
            if (!this.showInProgress)
            {
                this.showInProgress = true;
                this.cameraCaptureTask.Show();
            }

        }

        private void Image_Hold_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            user_feedback.VibrateOnItemSelected();


            // Make sure that we don't call this.photoChooserTask.Show() a second time before the first Show returned. 
            // This may happend if we click really fast the btnTakePicture button. If that happens the second Show will 
            // throw an InvalidOperationException exception.
            if (!this.showInProgress)
            {
                this.showInProgress = true;
                this.photoChooserTask.Show();
            }
        }


    }
}
