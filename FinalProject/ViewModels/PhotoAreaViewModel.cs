// -
// <copyright file="PhotoAreaViewModel.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GPS.Data;
using GPS.Utils;

namespace GPS.ViewModels
{
    /// <summary>
    /// View Model that is used by the PhotoArea user control.
    /// </summary>
    public class PhotoAreaViewModel : OcrViewModelBase
    {
        private ImageSource photoSource;    // Wrapped by this.PhotoSource
        private double photoAngle;          // Wrapped by this.PhotoAngle

        /// <summary>
        /// Initializes a new instance of the PhotoAreaViewModel class.
        /// </summary>
        /// <param name="ocrData">
        /// A reference to the OcrData instance that stores the 
        /// photo stream and the text obtained after the OCR conversion.
        /// </param>
        /// <param name="ocrConversionStateManager">
        /// A reference to the OcrConversionStateManager instance that stores the 
        /// status of the last OCR conversion.
        /// </param>
        public PhotoAreaViewModel(OcrData ocrData, OcrConversionStateManager ocrConversionStateManager)
            : base(ocrData, ocrConversionStateManager)
        {
            this.OcrData.PropertyChanged += new PropertyChangedEventHandler(this.OcrData_PropertyChanged);

            // If the photo stream is available when this instance is created we'll call OnPhotoStreamChanged
            // to update all properties related to the photo.
            if (this.OcrData.PhotoStream != null)
            {
                this.OnPhotoStreamChanged();
            }
        }

        /// <summary>
        /// Gets the stream that represents the photo as returned by CameraCaptureTask or PhotoChooserTask 
        /// in their Completed events in the ChosenPhoto field of the PhotoResult event arg.
        /// Note: the internal set will raise the PropertyChanged event.
        /// </summary>
        public ImageSource PhotoSource
        {
            get
            {
                return this.photoSource;
            }

            private set
            {
                if (!ReferenceEquals(this.photoSource, value))
                {
                    this.photoSource = value;
                    OnPropertyChanged("PhotoSource");
                }
            }
        }

        /// <summary>
        /// Gets the angle we need to rotate the photo with in order to show it correctly on the screen. 
        /// This angle depends on the orientation that the camera had when the photo was taken.
        /// </summary>
        public double PhotoAngle
        {
            get
            {
                return this.photoAngle;
            }

            private set
            {
                if (this.photoAngle != value)
                {
                    this.photoAngle = value;
                    OnPropertyChanged("PhotoAngle");
                }
            }
        }

        private void OcrData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PhotoStream")
            {
                if (this.OcrData.PhotoStream != null)
                {
                    this.OnPhotoStreamChanged();
                }
            }
        }

        /// <summary>
        /// Must be called after the photo stream stored by this.OcrData changed. 
        /// This will in turn update all properties that are related to the photo.
        /// </summary>
        private void OnPhotoStreamChanged()
        {
            this.SetPhotoSource(this.OcrData.PhotoStream);
            this.SetPhotoAngle(this.OcrData.ExifOrientationFlag);
        }

        /// <summary>
        /// Sets the this.PhotoSource field based on photoStream.
        /// </summary>
        /// <param name="photoStream">
        /// The photo stream as returned by CameraCaptureTask or PhotoChooserTask
        /// in their Completed events in the ChosenPhoto field of the PhotoResult event arg.
        /// </param>
        private void SetPhotoSource(Stream photoStream)
        {
            BitmapImage bmp = new BitmapImage();
            bmp.SetSource(photoStream);
            this.PhotoSource = bmp;
        }

        /// <summary>
        /// Sets the this.PhotoAngle field based on the photo orientation.
        /// </summary>
        /// <param name="exifOrientationFlag">
        /// The photo orientation. For comments on the photo orientation see ExifUtils.GetOrientationFlag.
        /// </param>
        private void SetPhotoAngle(int exifOrientationFlag)
        {
            double photoRotationAngle = 0;
            switch (exifOrientationFlag)
            {
                case 1:
                    // The PhotoAngle will be set to 0
                    break;
                case 8:
                    photoRotationAngle = 270;
                    break;
                case 3:
                    photoRotationAngle = 180;
                    break;
                case 6:
                    photoRotationAngle = 90;
                    break;

                default:
                    // We do not know how to handle other orientations so we will fall back to doing nothing
                    // this.PhotoAngle will remain 0;
                    break;
            }

            this.PhotoAngle = photoRotationAngle;
        }
    }
}
