using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CommonValueConverters.Converters
{
    /// <summary>
    /// Convert Dictionary Key to Value
    /// </summary>
    public class DictionaryToValue : IValueConverter
    {
        /// <summary>
        /// Convert Dictionary Key to Value
        /// </summary>
        /// <param name="value">Dictionary key</param>
        /// <param name="targetType"></param>
        /// <param name="parameter">Dictionary object</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not IDictionary || value == null) return value;
            dynamic dics = parameter as dynamic;
            return dics[value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
