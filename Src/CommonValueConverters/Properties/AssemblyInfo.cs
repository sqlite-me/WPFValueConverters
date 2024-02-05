using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Markup;

[assembly: XmlnsDefinition("http://schemas.wpf.litemagic.com/converters", "CommonValueConverters.Converters")]

#if !SILVERLIGHT40
[assembly: XmlnsDefinition("http://schemas.wpf.litemagic.com/converters", "CommonValueConverters.Converters.Markup")]
#endif