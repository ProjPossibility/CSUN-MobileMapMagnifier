// -
// <copyright file="AngleToRenderTransform.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace GPS.Converters
{
    /// <summary>
    /// Converter that transforms an angle into a RotateTransform.
    /// Note: One may be tempted to directly bind the Angle property of a RotateTransform. 
    ///       That will only work in WPF and not in Silverlight for WPF7. In Silverlight/WPF7 binding 
    ///       can be applied only to a FrameworkElement and RotateTransform is not a FrameworkElement.
    /// </summary>
    public class AngleToRenderTransform : IValueConverter
    {
        /// <summary>
        /// Converts the source data (value) that represents an angle measured in degrees to a RotateTransform 
        /// before passing it to the target of a data binding for display in the UI.
        /// </summary>
        /// <param name="value">
        /// The source data being passed to the target.
        /// This represents an angle measured in degrees.
        /// </param>
        /// <param name="targetType">
        /// The type of data expected by the target dependency property.
        /// </param>
        /// <param name="parameter">
        /// It will be ignored by this converter.
        /// </param>
        /// <param name="culture">
        /// The culture of the conversion. Ignored by this converter.
        /// </param>
        /// <returns>
        /// Returns a RotateTransform with an angle corresponding to value.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double? angle = (double?)value;

            RotateTransform rotateTransform = new RotateTransform();
            rotateTransform.Angle = angle.Value;

            return rotateTransform;
        }

        /// <summary>
        /// The ConvertBack is not implemented for this converter. 
        /// ConvertBack will throw NotImplementedException if invoked.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
