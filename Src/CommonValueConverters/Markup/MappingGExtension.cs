using CommonValueConverters.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using Exp = System.Linq.Expressions.Expression;

namespace CommonValueConverters.Converters.Markup
{
    [ContentProperty(nameof(To))]
#if !SILVERLIGHT
    [ValueConversion(typeof(object), typeof(object))]
#endif
    public sealed class MappingGExtension : MarkupExtension
    {
        public Type Type { get; set; } = typeof(object);

        public MappingGExtension() { }

        /// <summary>
        /// Initializes a new instance of the Mapping class with the specified <paramref name="from"/> and <paramref name="to"/> values.
        /// </summary>
        /// <param name="from">
        /// The value for the source in the mapping (see <see cref="From"/>).
        /// </param>
        /// <param name="to">
        /// The value for the destination in the mapping (see <see cref="To"/>).
        /// </param>
        public MappingGExtension(object from, object to)
        {
            this.From = from;
            this.To = to;
        }

        /// <summary>
        /// Gets or sets the source object for the mapping.
        /// </summary>
#if !SILVERLIGHT
        [ConstructorArgument("from")]
#endif
        public object From
        {
            get { return this.from; }
            set { this.from = value; }
        }
        private object from;

        /// <summary>
        /// Gets or sets the destination object for the mapping.
        /// </summary>
#if !SILVERLIGHT
        [ConstructorArgument("to")]
#endif
        public object To
        {
            get { return this.to; }
            set { this.to = value; }
        }
        private object to;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (to != null)
            {
                var type = to.GetType();
                bool typeError = false;
                if (Type.IsInterface)
                {
                    typeError = type.GetInterfaces().All(t => t != Type);
                }
                else
                if (type != Type && !type.IsSubclassOf(Type))
                {
                    typeError = true;
                }
                if (typeError)
                {
                    throw new TypeAccessException($"{nameof(To)} is not type {Type}");
                }
            }

            var typeMap = typeof(Mapping<>).MakeGenericType(Type);
            var ctor= typeMap.GetConstructor(new Type[] { typeof(object), Type });
            var delaget= Exp.Lambda(Exp.New(ctor, Exp.Constant(from,typeof(object)), Exp.Constant(to))).Compile();
            return delaget.DynamicInvoke();
        }
    }
}
