using CommonValueConverters.Selector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Exp = System.Linq.Expressions.Expression;

namespace CommonValueConverters.Converters
{
    [ContentProperty(nameof(Mappings))]
#if !SILVERLIGHT
    [ValueConversion(typeof(object), typeof(object))]
#endif
    public class MapDataTemplateSelector: System.Windows.Controls.DataTemplateSelector
    {
        private readonly Collection<Mapping<DataTemplate>> mappings;
        private readonly SelectorHelper<DataTemplate> helper;
        private readonly __TagetData__ _tagetData;

        /// <summary>
        /// Initializes a new instance of the MapDataTemplateSelector class.
        /// </summary>
        public MapDataTemplateSelector()
        {
            this.mappings = new Collection<Mapping<DataTemplate>>();
            helper=new SelectorHelper<DataTemplate>(mappings);
            _tagetData = new __TagetData__();
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
            DataTemplate? rlt = null;
            if (_binding != null)
            {
                if (_tagetData.SourceFromItem)
                {
                    _tagetData.Source = item;
                }
                var fromValue = _tagetData.Data;
                rlt = helper.Select(fromValue, Default);
            }
            else
            {
                var fromValue = __TagetData__.GetPropertyValue(item, PropertyPath);
                rlt = helper.Select(fromValue, Default);
            }

            if (rlt != null) { return rlt; }

            return base.SelectTemplate(item, container);
        }
    }
    internal class __TagetData__ : DependencyObject
    {
        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(__TagetData__), new PropertyMetadata(null));


        public object Source
        {
            get { return (object)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(__TagetData__), new PropertyMetadata(null));

        public bool SourceFromItem { get; set; }


        private static readonly Dictionary<string, Delegate> _assagPropertys = new();
        private static Delegate getFunc(Type type, string propertyPath)
        {
            var key = $"{type}_._{propertyPath}";
            if (_assagPropertys.ContainsKey(key)) return _assagPropertys[key];

            propertyPath = propertyPath.Trim(' ', '.');
            var param = Exp.Parameter(type);
            Exp expCurr = param;
            if (propertyPath.Length > 0)
            {
                foreach (var proName in propertyPath.Split('.'))
                {
                    var pro = expCurr.Type.GetProperty(proName);
                    if (pro == null)
                    {
                        throw new InvalidOperationException($"invalid path:({type.FullName}){propertyPath}");
                    }
                    expCurr = Exp.Condition(Exp.ReferenceNotEqual(expCurr, Exp.Constant(null)), Exp.Property(expCurr, pro), Exp.Default(pro.PropertyType));
                }
            }
            return _assagPropertys[key] = Exp.Lambda(expCurr, param).Compile();
        }

        public static object? GetPropertyValue(object value,string propertyPath)
        {
            if (!string.IsNullOrEmpty(propertyPath) && value != null)
            {
                value = getFunc(value.GetType(), propertyPath).DynamicInvoke(value)!;
            }

            return value;
        }
    }
}
