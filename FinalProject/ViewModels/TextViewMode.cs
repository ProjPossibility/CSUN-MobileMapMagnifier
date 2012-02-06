// -
// <copyright file="TextViewMode.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace GPS.ViewModels
{
    /// <summary>
    /// Specifies the view mode for the screen that shows the result of the OCR conversion.
    /// </summary>
    public enum TextViewMode
    {
        /// <summary>
        /// The text is displayed in a list where each item contains one text area.
        /// </summary>
        Simple = 0,

        /// <summary>
        /// The text is displayed in a list where each item 
        /// contains one individual word showing some of its OCR parameters.
        /// </summary>
        Detailed = 1,
    }
}
