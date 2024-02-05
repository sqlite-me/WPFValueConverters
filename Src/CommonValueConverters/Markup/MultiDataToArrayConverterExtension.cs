using CommonValueConverters.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace CommonValueConverters.Converters.Markup
{
    /// <inheritdoc cref="MultiDataToArrayConverter"/>
    public class MultiDataToArrayConverterExtension : MarkupExtension
    {
        public MultiDataToArrayConverterExtension() { }
#if !SILVERLIGHT
        public MultiDataToArrayConverterExtension(bool endWithParameter)
        {
            this.endWithParameter = endWithParameter;
        }
#endif

        /// <summary>
        /// if true append the parameter to the array end
        /// </summary>
#if !SILVERLIGHT
        [ConstructorArgument("endWithParameter")]
#endif
        public bool EndWithParameter { get => endWithParameter; set => endWithParameter = value; }
        public bool endWithParameter;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
#if !SILVERLIGHT
            return new MultiDataToArrayConverter(this.endWithParameter);
#else
            return new MultiDataToArrayConverter
            {
                EndWithParameter = this.endWithParameter
            };
#endif
        }
    }
}
