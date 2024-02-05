using CommonValueConverters.Selector;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace CommonValueConverters.Converters
{
    [ContentProperty("Mappings")]
#if !SILVERLIGHT
    [ValueConversion(typeof(object), typeof(object))]
#endif
    public class MapItemContainerTemplateSelector : System.Windows.Controls.ItemContainerTemplateSelector
    {
        private readonly Collection<Mapping<DataTemplate>> mappings;
        private readonly SelectorHelper<DataTemplate> helper;

        /// <summary>
        /// Initializes a new instance of the MapItemContainerTemplateSelector class.
        /// </summary>
        public MapItemContainerTemplateSelector()
        {
            this.mappings = new Collection<Mapping<DataTemplate>>();
            helper = new SelectorHelper<DataTemplate>(mappings);
        }

        /// <summary>
        /// Gets the collection of <see cref="Mapping"/>s configured for this <c>MapConverter</c>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Each <see cref="Mapping"/> defines a relationship between a source object (see <see cref="Mapping.From"/>) and a destination (see
        /// <see cref="Mapping.To"/>). The <c>MapConverter</c> uses these mappings whilst attempting to convert values.
        /// </para>
        /// </remarks>
        public Collection<Mapping<DataTemplate>> Mappings
        {
            get { return this.mappings; }
        }

        public string PropertyPath { get; set; }
        public object Default { get; set; } 
        public override DataTemplate SelectTemplate(object item, ItemsControl parentItemsControl)
        {
           var value =  __TagetData__.GetPropertyValue(item, PropertyPath);
            var rlt = helper.Select(value, Default);
            if (rlt != null) { return rlt; }

            return base.SelectTemplate(item, parentItemsControl);
        }
    }
}
