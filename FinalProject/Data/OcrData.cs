// -
// <copyright file="OcrData.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Hawaii.Services.Client.Ocr;
using GPS.Utils;

namespace GPS.Data
{
    /// <summary>
    /// OcrData implements a singleton that provides access to the photo and 
    /// corresponding text obtained from OCR conversion. It inherits from ModelBase which means that 
    /// interested parties can subscribe to its PropertyChanged. It also means that references to 
    /// the singleton instance of type OcrData can be used in data binding syntax in XAML files.
    /// </summary>
    public class OcrData : ModelBase
    {
        private static OcrData instance = new OcrData();
        private Stream photoStream;
        private int exifOrientationFlag;

        private OcrData()
        {
            this.OcrResultList = new ObservableCollection<OcrText>();
        }

        /// <summary>
        /// Gets the singleton instance of type OcrData.
        /// </summary>
        public static OcrData Instance
        {
            get
            {
                return OcrData.instance;
            }
        }

        /// <summary>
        /// Gets the OCR conversion result.
        /// </summary>
        public ObservableCollection<OcrText> OcrResultList { get; private set; }

        /// <summary>
        /// Gets or sets the stream that represents the photo as returned by CameraCaptureTask or PhotoChooserTask 
        /// in their Completed events in the ChosenPhoto field of the PhotoResult event arg.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when a 0 length stream is used as the value to set PhotoStream.
        /// </exception>
        public Stream PhotoStream
        {
            get
            {
                return this.photoStream;
            }

            set
            {
                if (!ReferenceEquals(this.photoStream, value))
                {
                    if (value.Length == 0)
                    {
                        throw new ArgumentException("PhotoStream cannot be set to a 0 length stream.", "PhotoStream");
                    }

                    this.photoStream = value;
                    OnPropertyChanged("PhotoStream");
                    OnPropertyChanged("PhotoStreamSize");
                }
            }
        }

        /// <summary>
        /// Gets the length of the photo stream in bytes.
        /// </summary>
        public long PhotoStreamSize
        {
            get
            {
                return this.photoStream.Length;
            }
        }

        /// <summary>
        /// Gets or sets the photo orientation.
        /// For comments on the photo orientation see ExifUtils.GetOrientationFlag.
        /// </summary>
        public int ExifOrientationFlag
        {
            get
            {
                return this.exifOrientationFlag;
            }

            set
            {
                if (!ReferenceEquals(this.exifOrientationFlag, value))
                {
                    this.exifOrientationFlag = value;
                    OnPropertyChanged("ExifOrientationFlag");
                }
            }
        }

        /// <summary>
        /// Sets the OCR conversion result.
        /// </summary>
        /// <param name="ocrResultList">
        /// The OCR conversion result.
        /// </param>
        public void SetOcrResult(List<OcrText> ocrResultList)
        {
            Debug.Assert(ocrResultList != null, "SetOcrResult must not be called wit a null ocrResultList");
            Debug.Assert(this.OcrResultList != null, "this.OcrResultList must be already initialized at this point");

            this.OcrResultList.Clear();
            foreach (OcrText ocrText in ocrResultList)
            {
                this.OcrResultList.Add(ocrText);
            }
        }

        /// <summary>
        /// Determines the total word count from all text blocks stored in the OCR conversion result.
        /// </summary>
        /// <returns>
        /// The total word count.
        /// </returns>
        public int GetWordCount()
        {
            Debug.Assert(this.OcrResultList != null, "this.OcrResultList must be already initialized at this point");

            int wordCount = 0;
            foreach (OcrText ocrText in this.OcrResultList)
            {
                wordCount += ocrText.Words.Count;
            }

            return wordCount;
        }
    }
}
