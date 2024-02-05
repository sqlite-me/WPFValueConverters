using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace CommonValueConverters.Converters
{
    /// <summary>
    /// Mult Number to one, Convert to <see cref="MultiNumberConvertType.Avg"/>/<see cref="MultiNumberConvertType.Sum"/>/<see cref="MultiNumberConvertType.Max"/>/<see cref="MultiNumberConvertType.Min"/>
    /// </summary>
    public class MultiNumberConverter : IMultiValueConverter
    {
        private MultiNumberConvertType convertType;

        /// <summary>
        /// 
        /// </summary>
        public MultiNumberConverter() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="convertType"></param>
        public MultiNumberConverter(MultiNumberConvertType convertType)
        {
            this.convertType = convertType;
        }


        /// <summary>
        /// Gets or sets the expression for this <c>MathConverter</c>.
        /// </summary>
#if !SILVERLIGHT
        [ConstructorArgument("convertType")]
#endif
        public MultiNumberConvertType ConvertType
        {
            get
            {
                return this.convertType;
            }

            set
            {
                this.convertType = value;
            }
        }


        public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null) return null;
            if (values.Length == 0) return 0;
            
            var arr = values.Select(t=>System.Convert.ToDouble(t)).ToArray();
          
            return convertType switch
            {
                MultiNumberConvertType.Min => min(arr),
                MultiNumberConvertType.Max => max(arr),
                MultiNumberConvertType.Avg => arr.Average(),
                MultiNumberConvertType.Sum => arr.Sum(),
                _ => null,
            };
        }

        private double max(double[] arr)
        {
            var m = arr[0];
            for(var i=1;i<arr.Length;i++)
            {
                if (arr[i] > m) m =arr[i];
            }
            return m;
        }

        private double min(double[] arr)
        {
            var m = arr[0];
            for (var i = 1; i < arr.Length; i++)
            {
                if (arr[i] < m) m = arr[i];
            }
            return m;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum MultiNumberConvertType
    {
        Sum,
        Avg,
        Max,
        Min,
    }
}
