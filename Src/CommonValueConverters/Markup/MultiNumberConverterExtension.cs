using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace CommonValueConverters.Converters.Markup
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MultiNumberConverterExtension : MarkupExtension
    {
        private MultiNumberConvertType convertType;

        /// <summary>
        /// 
        /// </summary>
        public MultiNumberConverterExtension() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="convertType"></param>
        public MultiNumberConverterExtension(MultiNumberConvertType convertType)
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


        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new MultiNumberConverter(this.convertType);
        }
    }
}
