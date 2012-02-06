// -
// <copyright file="Utils.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;

namespace GPS.Utils
{
    /// <summary>
    /// Utils is a general utility class that contains helper methods used throughout this application.
    /// </summary>
    public class OcrClientUtils
    {
        /// <summary>
        /// Returns a byte array with the content of the stream.
        /// </summary>
        public static byte[] GetPhotoBits(Stream stream)
        {
            Debug.Assert(stream != null, "GetPhotoBits should not be called with a null stream.");
            byte[] photoBits = new byte[stream.Length];

            long seekPosition = stream.Seek(0, SeekOrigin.Begin);
            int bytesRead = stream.Read(photoBits, 0, photoBits.Length);
            Debug.Assert(bytesRead == photoBits.Length, "Validating the number of bytes read from the stream.");
            seekPosition = stream.Seek(0, SeekOrigin.Begin);

            return photoBits;
        }

        /// <summary>
        /// It will ensure that the photo does not exceed a maximum size. 
        /// If needed the photo is scaled down by preserving its aspect ratio.
        /// </summary>
        /// <param name="photoStream">
        /// The stream that represents the photo as returned by CameraCaptureTask or PhotoChooserTask.
        /// </param>
        /// <param name="photoMaxSizeDiagonal">
        /// The diagonal of the scaled down photo.
        /// </param>
        /// <returns>
        /// Returns a stream that has the scaled-down photo in JPEG format.
        /// If the original stream represents a photo that is smaller or the same size as the scale-down size
        /// then it returns the original stream.
        /// </returns>
        public static Stream LimitPhotoSize(Stream photoStream, double photoMaxSizeDiagonal)
        {
            WriteableBitmap wb = new WriteableBitmap(1, 1);
            wb.SetSource(photoStream);

            // Only scale down if needed.
            double photoSizeDiagonal = Math.Sqrt(wb.PixelWidth * wb.PixelWidth + wb.PixelHeight * wb.PixelHeight);
            if (photoSizeDiagonal > photoMaxSizeDiagonal)
            {
                int newWidth = (int)(wb.PixelWidth * photoMaxSizeDiagonal / photoSizeDiagonal);
                int newHeight = (int)(wb.PixelHeight * photoMaxSizeDiagonal / photoSizeDiagonal);

                Stream resizedStream = new MemoryStream();
                Extensions.SaveJpeg(wb, resizedStream, newWidth, newHeight, 0, 100);

                return resizedStream;
            }

            // No need to scale down. The photo is smaller or the same size as the scale-down size
            return photoStream;
        }
    }
}

