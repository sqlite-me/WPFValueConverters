using CommonValueConverters.Converters;
using CommonValueConverters.Selector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Markup;
using System.Security.Policy;

namespace CommonValueConverters.Converters
{
    [ContentProperty(nameof(TemplateCollection))]
#if !SILVERLIGHT
    [ValueConversion(typeof(object), typeof(object))]
#endif
    public class DataTypeTemplateSelector : System.Windows.Controls.DataTemplateSelector
    {
        private readonly Collection<DataTemplate> mappings;
        private readonly __TagetData__ _tagetData;

        /// <summary>
        /// Initializes a new instance of the MapDataTemplateSelector class.
        /// </summary>
        public DataTypeTemplateSelector()
        {
            this.mappings = new Collection<DataTemplate>();
            _tagetData = new __TagetData__();
        }

        /// <summary>
        /// Gets the collection of <see cref="Mapping"/>s configured for this <c>MapConverter</c>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// </remarks>
        public Collection<DataTemplate> TemplateCollection
        {
            get { return this.mappings; }
        }

        public string PropertyPath { get; set; }
        public BindingBase Binding
        {
            get => _binding; set
            {
                _binding = value;
                BindingOperations.ClearBinding(_tagetData, __TagetData__.DataProperty);
                _tagetData.SourceFromItem = false;
                if (_binding is Binding binding && binding.RelativeSource == null && binding.Source == null)
                {
                    _tagetData.SourceFromItem = true;
                    var path = binding.Path.Path?.Trim(' ', ' ') ?? "";
                    binding.Path.Path = path == "" ? nameof(__TagetData__.Source) : nameof(__TagetData__.Source) + "." + path;
                    binding.Source = _tagetData;
                }
                if (_binding != null)
                    BindingOperations.SetBinding(_tagetData, __TagetData__.DataProperty, _binding);
            }
        }
        private BindingBase _binding;
        public object Default { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            object? fromValue=null;
            if (_binding != null)
            {
                if (_tagetData.SourceFromItem)
                {
                    _tagetData.Source = item;
                }
                fromValue = _tagetData.Data;
            }
            else
            {
                fromValue = __TagetData__.GetPropertyValue(item, PropertyPath);
            }

            DataTemplate? rlt = null;
            foreach (var one in mappings)
            {
                if (one.DataType is Type type && type.IsInstanceOfType(fromValue))
                {
                    return one;
                }
            }
            if (Default is Type typeDef)
            {
                foreach (var one in mappings)
                {
                    if (typeDef.Equals(one.DataType))
                    {
                        return one;
                    }
                }
            }

                if (rlt != null) { return rlt; }

            return base.SelectTemplate(item, container);
        }
    }
}
