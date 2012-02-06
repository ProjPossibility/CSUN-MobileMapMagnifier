// -
// <copyright file="ExifUtils.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System;
using System.Diagnostics;
using System.IO;
using System.Resources;

namespace GPS.Utils
{
    // ==================================================
    //  Photo Orientation field
    // ==================================================
    //  Here is how a photo taken of the letter F would looke when displayed by 
    //  an application that ignores the orientation tag.
    //  Orientation:    1        2       3      4         5            6           7          8
    //                888888  888888      88  88      8888888888  88                  88  8888888888
    //                88          88      88  88      88  88      88  88          88  88      88  88
    //                8888      8888    8888  8888    88          8888888888  8888888888          88
    //                88          88      88  88
    //                88          88  888888  888888
    // Note: Only the values 1,3,6,and 8 can be obtained if the camera simply changes its orientation (rotates).
    //       For the other values the camera would need to do some "miror" or "flip" effect.

    /// <summary>
    /// ExifUtils is a utility class that allow clients to get information 
    /// like the "photo orientation" field from an image in Exif format.
    /// Exif (Exchangeable image file format) is the image file format 
    /// used by digital cameras including those on smartphones.
    /// </summary>
    public class ExifUtils
    {
        /// <summary>
        /// Indicates an unknown orientation of the photo.
        /// </summary>
        public const int UnknownOrientation = 9;

        private const int StartOfImage = 0xFFD8;
        private const int StartOfStreamMarkerId = 0xFFDA;
        private const int ExifApplicationMarkerId = 0xFFE1;
        private const int OrientationTagId = 0x0112;

        private byte[] photoBits;

        /// <summary>
        /// Initializes a new instance of the ExifUtils class.
        /// </summary>
        /// <param name="photoStreamBits">
        /// The content in byte array form of the photo stream. This is the photo stream as returned 
        /// by CameraCaptureTask or PhotoChooserTask in their Completed events in the 
        /// ChosenPhoto field of the PhotoResult event arg.
        /// </param>
        public ExifUtils(byte[] photoStreamBits)
        {
            this.photoBits = photoStreamBits;
        }

        /// <summary>
        /// Obtains the photo orientation field.
        /// For the valid values see the comment marked with "Photo Orientation field".
        /// If the photo orientation field cannot be obtained then it returns ExifUtils.UnknownOrientation.
        /// </summary>
        public int GetOrientationFlag()
        {
            try
            {
                return this.InternalGetOrientationFlag();
            }
            catch (FormatException)
            {
                return ExifUtils.UnknownOrientation;
            }
        }

        /// <summary>
        /// Obtains the photo orientation field.
        /// For the valid values see the comment marked with "Photo Orientation field".
        /// If the photo orientation field cannot be obtained then it throws a FormatException.
        /// </summary>
        /// <exception cref="FormatException"></exception>
        private int InternalGetOrientationFlag()
        {
            long cursor = 0;

            this.SkipStartOfImageMarker(ref cursor);
            this.AdvanceToExifApplicationMarker(ref cursor);

            cursor += 2;    // Skip the 2 bytes that indicate the APP1 data size
            cursor += 6;    // Skip the EXIF header.
            cursor += 8;    // Skip the TIFF header.

            // AT this point we are the start of the IFD0 (Image File Directory) inside of Exif Application Marker
            // We'll identify and retrieve the tag that describes the orientation
            ExifTagInfo exifTagInfo = this.GetTagInfo(ExifUtils.OrientationTagId, cursor);
            return exifTagInfo.ValueOrOffset >> 16;
        }

        private void SkipStartOfImageMarker(ref long cursor)
        {
            int soi = this.GetInt16BigEndian(ref cursor);
            if (soi != ExifUtils.StartOfImage)
            {
                throw new FormatException("Invalid Exif format: Start Of Image marker not found.");
            }
        }

        /// <summary>
        /// Advances the cursor to the Exif Application Marker immediately after the marker id.
        /// </summary>
        private void AdvanceToExifApplicationMarker(ref long cursor)
        {
            this.AdvanceToApplicationMarker(ExifUtils.ExifApplicationMarkerId, ref cursor);
        }

        /// <summary>
        /// Advances the cursor to the given Application Marker immediately after the marker id.
        /// </summary>
        private void AdvanceToApplicationMarker(int applicationMarkerId, ref long cursor)
        {
            while (cursor < this.photoBits.Length)
            {
                int marker = this.GetInt16BigEndian(ref cursor);
                if (marker == applicationMarkerId)
                {
                    return;
                }

                if (marker == ExifUtils.StartOfStreamMarkerId)
                {
                    // We reached the StartOfStreamMarkerId. We will not find the applicationMarkerId further down.
                    break;
                }

                // Skip the data for this marker area.
                int dataSize = this.GetInt16BigEndian(ref cursor);
                cursor += dataSize - 2;   // -2 because we already skipped the 2 bytes with dataSize.
            }

            throw new FormatException("Invalid Exif format: Exif Application Marker (APP1:0xFFE1) not found.");
        }

        /// <summary>
        /// Identifies and returns data about a given tag in the Image File Directory area.
        /// The cursor must be at the start of the Image File Directory area.
        /// </summary>
        private ExifTagInfo GetTagInfo(int targetTagNumber, long cursor)
        {
            // The Image File Directory starts with 2 bytes that indicate the number of tags.
            // Each tags has 4 fields
            int tagsCount = this.GetInt16BigEndian(ref cursor);

            for (int tagIndex = 0; tagIndex < tagsCount; tagIndex++)
            {
                int tagNumber = this.GetInt16BigEndian(ref cursor);
                if (targetTagNumber == tagNumber)
                {
                    int format = this.GetInt16BigEndian(ref cursor);
                    int componentsNumber = this.GetInt32BigEndian(ref cursor);
                    int valueOrOffset = this.GetInt32BigEndian(ref cursor);

                    return new ExifTagInfo(tagNumber, format, componentsNumber, valueOrOffset);
                }

                cursor += 2;    // skip the format field.
                cursor += 4;    // skip the components number field.
                cursor += 4;    // skip the components value/offset field.
            }

            throw new FormatException(string.Format("IFD tag {0} not found", targetTagNumber));
        }

        private int GetInt16BigEndian(ref long cursor)
        {
            if (cursor + 1 >= this.photoBits.Length)
            {
                throw new FormatException("Unexpected end of data");
            }

            cursor += 2;
            return (this.photoBits[cursor - 2] << 8) + this.photoBits[cursor - 1];
        }

        private int GetInt32BigEndian(ref long cursor)
        {
            if (cursor + 3 >= this.photoBits.Length)
            {
                throw new FormatException("Unexpected end of data");
            }

            cursor += 4;
            return
                (this.photoBits[cursor - 4] << 24) +
                (this.photoBits[cursor - 3] << 16) +
                (this.photoBits[cursor - 2] << 8) +
                this.photoBits[cursor - 1];
        }

        /// <summary>
        /// ExifTagInfo contains information about a Exif Tag.
        /// </summary>
        private class ExifTagInfo
        {
            /// <summary>
            /// Initializes a new instance of the ExifTagInfo class.
            /// </summary>
            /// <param name="tagNumber">The tag number. This is a number that identifies the tag type.</param>
            /// <param name="format">The format of the tag data.</param>
            /// <param name="componentsNumber">
            /// The number of components. 
            /// Used when there is a maximum limit for the number of components.
            /// </param>
            /// <param name="valueOrOffset">
            /// The value of the tag or the offset where the tag value can be find.
            /// </param>
            public ExifTagInfo(int tagNumber, int format, int componentsNumber, int valueOrOffset)
            {
                this.TagNumber = tagNumber;
                this.Format = format;
                this.ComponentsNumber = componentsNumber;
                this.ValueOrOffset = valueOrOffset;
            }

            /// <summary>
            /// Gets the tag number. This is a number that identifies the tag type.
            /// </summary>
            public int TagNumber { get; private set; }

            /// <summary>
            /// Gets the format of the tag data.
            /// </summary>
            public int Format { get; private set; }

            /// <summary>
            /// Gets the number of components. 
            /// This is used when there is a maximum limit for the number of components.
            /// </summary>
            public int ComponentsNumber { get; private set; }

            /// <summary>
            /// Gets the value of the tag or the offset where the tag value can be find.
            /// </summary>
            public int ValueOrOffset { get; private set; }
        }
    }
}
