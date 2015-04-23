using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace BreakthruGame
{
    /// <summary>
    /// Converts a point and a multiplier to a TranslateTransform
    /// </summary>
    public class PosToTranslationConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts a point and multiplier to a TranslateTransform by setting X in the translate transform to point.X * multiplier (and vice versa for Y).
        /// </summary>
        /// <param name="values">An array with a System.Windows.Point at index 0 and a double at index 0</param>
        /// <param name="targetType">Not used (but should be some type that fits TranslateTransform)</param>
        /// <param name="parameter">Not used</param>
        /// <param name="culture">Not used</param>
        /// <returns>A TranslateTransform as desacribed above</returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Initialize to dummy values to avoid compilation error.
            Point point = new Point();
            double multiplier = 0;

            try
            {
                point = (Point)values[0];
                multiplier = (double)values[1];
            }
            catch (InvalidCastException)
            {
                return null;
            }

            return new TranslateTransform
            {
                X = point.X * multiplier,
                Y = point.Y * multiplier
            };
        }

        /// <summary>
        /// Implemented as part of IMultiValueConverter, but throws NotImplementedException. Should NOT be used.
        /// </summary>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
