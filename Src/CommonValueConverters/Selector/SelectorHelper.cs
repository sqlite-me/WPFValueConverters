using CommonValueConverters.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CommonValueConverters.Selector
{
    internal class SelectorHelper<T>
    {
        private readonly Collection<Mapping<T>> mappings;

        public SelectorHelper(Collection<Mapping<T>> mappings)
        {
            this.mappings = mappings;
        }

        public T? Select(object? fromValue, object @default)
        {
            if (fromValue == null && @default != null) fromValue = @default;

            T defValue = default;
            foreach (var mapping in this.mappings)
            {
                if (object.Equals(fromValue, mapping.From) && mapping.To is T template)
                {
                    return template;
                }
                else if (@default != null && object.Equals(@default, mapping.From) && mapping.To is T templateDef)
                {
                    defValue = mapping.To;
                }
            }

            if (@default is T template1)
            {
                return template1;
            }

            return defValue;
        }
    }
}
