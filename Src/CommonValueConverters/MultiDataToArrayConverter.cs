using System;
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
    /// multi binding to object array
    /// </summary>
    public class MultiDataToArrayConverter : IMultiValueConverter, IValueConverter
    {
        public MultiDataToArrayConverter() { }
        public MultiDataToArrayConverter(bool endWithParameter)
        {
            this.EndWithParameter = endWithParameter;
        }

        /// <summary>
        /// if true append the parameter to the array end
        /// </summary>
        public bool EndWithParameter { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values?.Length > 0)
            {
                var len = EndWithParameter ? values.Length + 1 : values.Length;
                object[] objects = new object[len];
                if (len > values.Length)
                {
                    objects[values.Length] = parameter;
                }
                for (int i = 0; i < values.Length; i++)
                {
                    objects[i] = values[i];
                }

                return objects;
            }

            return null;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return EndWithParameter ? new[] { value, parameter } : new[] { value };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value is object[] objects)
            {
                var len = EndWithParameter ? objects.Length - 1 : objects.Length;
                if (len < 1) { return null; }
                var backResult = new object[len];
                for (int i = 0; i < len; i++)
                {
                    backResult[i] = objects[i];
                }
                return backResult;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is object[] objects)
            {
                return objects.FirstOrDefault();
            }
            return value;
        }
    }
}
