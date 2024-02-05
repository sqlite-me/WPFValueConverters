using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonValueConverters.Helper
{
    public static class ArgumentHelper
    {
        [DebuggerHidden]
        public static void AssertNotNull<T>(T arg, string argName)
            where T : class
        {
            if (arg == null)
            {
                throw new ArgumentNullException(argName);
            }
        }

        [DebuggerHidden]
        public static void AssertNotNull<T>(T? arg, string argName)
            where T : struct
        {
            if (!arg.HasValue)
            {
                throw new ArgumentNullException(argName);
            }
        }

        [DebuggerHidden]
        public static void AssertGenericArgumentNotNull<T>(T arg, string argName)
        {
            var type = typeof(T).GetTypeInfo();

            if (!type.IsValueType || (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>))))
            {
                AssertNotNull((object)arg, argName);
            }
        }

        [DebuggerHidden]
        public static void AssertNotNull<T>(IEnumerable<T> arg, string argName, bool assertContentsNotNull)
        {
            // make sure the enumerable item itself isn't null
            AssertNotNull(arg, argName);

            if (assertContentsNotNull && !typeof(T).GetTypeInfo().IsValueType)
            {
                // make sure each item in the enumeration isn't null
                foreach (var item in arg)
                {
                    if (item == null)
                    {
                        throw new ArgumentException("An item inside the enumeration was null.", argName);
                    }
                }
            }
        }

        [DebuggerHidden]
        public static void AssertNotNullOrEmpty(string arg, string argName)
        {
            if (string.IsNullOrEmpty(arg))
            {
                throw new ArgumentException("Cannot be null or empty.", argName);
            }
        }

        [DebuggerHidden]
        public static void AssertNotNullOrEmpty(IEnumerable arg, string argName)
        {
            if (arg == null || !arg.GetEnumerator().MoveNext())
            {
                throw new ArgumentException("Cannot be null or empty.", argName);
            }
        }

        [DebuggerHidden]
        public static void AssertNotNullOrEmpty(ICollection arg, string argName)
        {
            if (arg == null || arg.Count == 0)
            {
                throw new ArgumentException("Cannot be null or empty.", argName);
            }
        }

        [DebuggerHidden]
        public static void AssertNotNullOrWhiteSpace(string arg, string argName)
        {
            if (arg == null)
            {
                throw new ArgumentException("Cannot be null or white-space.", argName);
            }

            for (var i = 0; i < arg.Length; ++i)
            {
                if (!char.IsWhiteSpace(arg, i))
                {
                    return;
                }
            }

            throw new ArgumentException("Cannot be null or white-space.", argName);
        }

        [DebuggerHidden]
        [CLSCompliant(false)]
        public static void AssertEnumMember<TEnum>(TEnum enumValue, string argName)
                where TEnum : struct
        {
            var validValues = Enum
                .GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .ToArray();

            AssertEnumMember(
                enumValue,
                argName,
                validValues);
        }

        [DebuggerHidden]
        [CLSCompliant(false)]
        public static void AssertEnumMember<TEnum>(TEnum enumValue, string argName, params TEnum[] validValues)
            where TEnum : struct
        {
            AssertNotNull(validValues, nameof(validValues));

            if (typeof(TEnum).GetTypeInfo().GetCustomAttribute<FlagsAttribute>(false) != null)
            {
                // flag enumeration
                bool throwEx;
                var longValue = Convert.ToInt64(enumValue, CultureInfo.InvariantCulture);

                if (longValue == 0)
                {
                    // only throw if zero isn't permitted by the valid values
                    throwEx = true;

                    foreach (TEnum value in validValues)
                    {
                        if (Convert.ToInt64(value, CultureInfo.InvariantCulture) == 0)
                        {
                            throwEx = false;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var value in validValues)
                    {
                        longValue &= ~Convert.ToInt64(value, CultureInfo.InvariantCulture);
                    }

                    // throw if there is a value left over after removing all valid values
                    throwEx = longValue != 0;
                }

                if (throwEx)
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Enum value '{0}' is not valid for flags enumeration '{1}'.",
                            enumValue,
                            typeof(TEnum).FullName),
                        argName);
                }
            }
            else
            {
                // not a flag enumeration
                foreach (var value in validValues)
                {
                    if (enumValue.Equals(value))
                    {
                        return;
                    }
                }

                // at this point we know an exception is required - however, we want to tailor the message based on whether the
                // specified value is undefined or simply not allowed
                if (!Enum.IsDefined(typeof(TEnum), enumValue))
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Enum value '{0}' is not defined for enumeration '{1}'.",
                            enumValue,
                            typeof(TEnum).FullName),
                        argName);
                }
                else
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Enum value '{0}' is defined for enumeration '{1}' but it is not permitted in this context.",
                            enumValue,
                            typeof(TEnum).FullName),
                        argName);
                }
            }
        }

        private static bool IsOnlyWhitespace(string arg)
        {
            Debug.Assert(arg != null, "Expecting arg to be non-null.");

            foreach (var c in arg)
            {
                if (!char.IsWhiteSpace(c))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
