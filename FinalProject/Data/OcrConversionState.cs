// -
// <copyright file="OcrConversionState.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace GPS.Data
{
    /// <summary>
    /// Specifies the state of the OCR conversion.
    /// </summary>
    public enum OcrConversionState
    {
        /// <summary>
        /// Indicates that the conversion was not started.
        /// </summary>
        ConversionNotStarted = 0,

        /// <summary>
        /// Indicates that the conversion is in progress.
        /// </summary>
        Converting = 1,

        /// <summary>
        /// Indicates that the conversion completed successfully but 
        /// that the result returned from the OCR service was empty.
        /// </summary>
        ConversionEmpty = 2,

        /// <summary>
        /// Indicates that the conversion completed successfully.
        /// </summary>
        ConversionOK = 3,

        /// <summary>
        /// Indicates that the conversion completed with an error.
        /// </summary>
        ConversionError = 4,
    }
}
