using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Xunit;

namespace Kent.Boogaart.Converters.UnitTest
{
    public sealed class CaseConverterTest : UnitTest
    {
        private CaseConverter caseConverter;

        protected override void SetUpCore()
        {
            base.SetUpCore();
            this.caseConverter = new CaseConverter();
        }

        [Fact]
        public void Constructor_ShouldSetDefaults()
        {
            Assert.Equal(CharacterCasing.Normal, this.caseConverter.SourceCasing);
            Assert.Equal(CharacterCasing.Normal, this.caseConverter.TargetCasing);
        }

        [Fact]
        public void Constructor_Casing_ShouldSetSourceAndTargetCasings()
        {
            this.caseConverter = new CaseConverter(CharacterCasing.Upper);
            Assert.Equal(CharacterCasing.Upper, this.caseConverter.SourceCasing);
            Assert.Equal(CharacterCasing.Upper, this.caseConverter.TargetCasing);
            this.caseConverter = new CaseConverter(CharacterCasing.Lower);
            Assert.Equal(CharacterCasing.Lower, this.caseConverter.SourceCasing);
            Assert.Equal(CharacterCasing.Lower, this.caseConverter.TargetCasing);
        }

        [Fact]
        public void Constructor_Casings_ShouldSetCasings()
        {
            this.caseConverter = new CaseConverter(CharacterCasing.Upper, CharacterCasing.Lower);
            Assert.Equal(CharacterCasing.Upper, this.caseConverter.SourceCasing);
            Assert.Equal(CharacterCasing.Lower, this.caseConverter.TargetCasing);
        }

        [Fact]
        public void SourceCasing_ShouldThrowIfInvalid()
        {
            var ex = Assert.Throws<ArgumentException>(() => this.caseConverter.SourceCasing = (CharacterCasing)100);
            Assert.Equal("Enum value '100' is not defined for enumeration 'System.Windows.Controls.CharacterCasing'.\r\nParameter name: value", ex.Message);
        }

        [Fact]
        public void SourceCasing_ShouldGetAndSetSourceCasing()
        {
            Assert.Equal(CharacterCasing.Normal, this.caseConverter.SourceCasing);
            this.caseConverter.SourceCasing = CharacterCasing.Upper;
            Assert.Equal(CharacterCasing.Upper, this.caseConverter.SourceCasing);
            this.caseConverter.SourceCasing = CharacterCasing.Lower;
            Assert.Equal(CharacterCasing.Lower, this.caseConverter.SourceCasing);
        }

        [Fact]
        public void TargetCasing_ShouldThrowIfInvalid()
        {
            var ex = Assert.Throws<ArgumentException>(() => this.caseConverter.TargetCasing = (CharacterCasing)100);
            Assert.Equal("Enum value '100' is not defined for enumeration 'System.Windows.Controls.CharacterCasing'.\r\nParameter name: value", ex.Message);
        }

        [Fact]
        public void TargetCasing_ShouldGetAndSetTargetCasing()
        {
            Assert.Equal(CharacterCasing.Normal, this.caseConverter.TargetCasing);
            this.caseConverter.TargetCasing = CharacterCasing.Upper;
            Assert.Equal(CharacterCasing.Upper, this.caseConverter.TargetCasing);
            this.caseConverter.TargetCasing = CharacterCasing.Lower;
            Assert.Equal(CharacterCasing.Lower, this.caseConverter.TargetCasing);
        }

        [Fact]
        public void Casing_ShouldThrowIfInvalid()
        {
            var ex = Assert.Throws<ArgumentException>(() => this.caseConverter.Casing = (CharacterCasing)100);
            Assert.Equal("Enum value '100' is not defined for enumeration 'System.Windows.Controls.CharacterCasing'.\r\nParameter name: value", ex.Message);
        }

        [Fact]
        public void Casing_ShouldSetSourceAndTargetCasings()
        {
            Assert.Equal(CharacterCasing.Normal, this.caseConverter.SourceCasing);
            Assert.Equal(CharacterCasing.Normal, this.caseConverter.TargetCasing);
            this.caseConverter.Casing = CharacterCasing.Upper;
            Assert.Equal(CharacterCasing.Upper, this.caseConverter.SourceCasing);
            Assert.Equal(CharacterCasing.Upper, this.caseConverter.TargetCasing);
            this.caseConverter.Casing = CharacterCasing.Lower;
            Assert.Equal(CharacterCasing.Lower, this.caseConverter.SourceCasing);
            Assert.Equal(CharacterCasing.Lower, this.caseConverter.TargetCasing);
        }

        [Fact]
        public void Convert_ShouldDoNothingIfValueIsNotAString()
        {
            Assert.Same(DependencyProperty.UnsetValue, this.caseConverter.Convert(123, null, null, null));
            Assert.Same(DependencyProperty.UnsetValue, this.caseConverter.Convert(123d, null, null, null));
            Assert.Same(DependencyProperty.UnsetValue, this.caseConverter.Convert(DateTime.Now, null, null, null));
        }

        [Fact]
        public void Convert_ShouldDoNothingIfCasingIsNormal()
        {
            Assert.Equal(CharacterCasing.Normal, this.caseConverter.TargetCasing);
            Assert.Equal("abcd", this.caseConverter.Convert("abcd", null, null, null));
            Assert.Equal("ABCD", this.caseConverter.Convert("ABCD", null, null, null));
            Assert.Equal("AbCd", this.caseConverter.Convert("AbCd", null, null, null));
        }

        [Fact]
        public void Convert_ShouldConvertStringsToSpecifiedCasing()
        {
            this.caseConverter.TargetCasing = CharacterCasing.Lower;
            Assert.Equal("abcd", this.caseConverter.Convert("abcd", null, null, null));
            Assert.Equal("abcd", this.caseConverter.Convert("ABCD", null, null, null));
            Assert.Equal("abcd", this.caseConverter.Convert("AbCd", null, null, null));

            this.caseConverter.TargetCasing = CharacterCasing.Upper;
            Assert.Equal("ABCD", this.caseConverter.Convert("abcd", null, null, null));
            Assert.Equal("ABCD", this.caseConverter.Convert("ABCD", null, null, null));
            Assert.Equal("ABCD", this.caseConverter.Convert("AbCd", null, null, null));
        }

        [Fact]
        public void Convert_ShouldUseSpecifiedCulture()
        {
            CultureInfo cultureInfo = new CultureInfo("tr");

            this.caseConverter.TargetCasing = CharacterCasing.Lower;
            Assert.Equal("ijk", this.caseConverter.Convert("ijk", null, null, cultureInfo));
            Assert.Equal("ıjk", this.caseConverter.Convert("IJK", null, null, cultureInfo));
            Assert.Equal("ijk", this.caseConverter.Convert("iJk", null, null, cultureInfo));

            this.caseConverter.TargetCasing = CharacterCasing.Upper;
            Assert.Equal("İJK", this.caseConverter.Convert("ijk", null, null, cultureInfo));
            Assert.Equal("IJK", this.caseConverter.Convert("IJK", null, null, cultureInfo));
            Assert.Equal("İJK", this.caseConverter.Convert("iJk", null, null, cultureInfo));
        }

        [Fact]
        public void ConvertBack_ShouldDoNothingIfValueIsNotAString()
        {
            Assert.Same(DependencyProperty.UnsetValue, this.caseConverter.ConvertBack(123, null, null, null));
            Assert.Same(DependencyProperty.UnsetValue, this.caseConverter.ConvertBack(123d, null, null, null));
            Assert.Same(DependencyProperty.UnsetValue, this.caseConverter.ConvertBack(DateTime.Now, null, null, null));
        }

        [Fact]
        public void ConvertBack_ShouldDoNothingIfCasingIsNormal()
        {
            Assert.Equal(CharacterCasing.Normal, this.caseConverter.SourceCasing);
            Assert.Equal("abcd", this.caseConverter.ConvertBack("abcd", null, null, null));
            Assert.Equal("ABCD", this.caseConverter.ConvertBack("ABCD", null, null, null));
            Assert.Equal("AbCd", this.caseConverter.ConvertBack("AbCd", null, null, null));
        }

        [Fact]
        public void ConvertBack_ShouldConvertStringsToSpecifiedCasing()
        {
            this.caseConverter.SourceCasing = CharacterCasing.Lower;
            Assert.Equal("abcd", this.caseConverter.ConvertBack("abcd", null, null, null));
            Assert.Equal("abcd", this.caseConverter.ConvertBack("ABCD", null, null, null));
            Assert.Equal("abcd", this.caseConverter.ConvertBack("AbCd", null, null, null));

            this.caseConverter.SourceCasing = CharacterCasing.Upper;
            Assert.Equal("ABCD", this.caseConverter.ConvertBack("abcd", null, null, null));
            Assert.Equal("ABCD", this.caseConverter.ConvertBack("ABCD", null, null, null));
            Assert.Equal("ABCD", this.caseConverter.ConvertBack("AbCd", null, null, null));
        }

        [Fact]
        public void ConvertBack_ShouldUseSpecifiedCulture()
        {
            CultureInfo cultureInfo = new CultureInfo("tr");

            this.caseConverter.SourceCasing = CharacterCasing.Lower;
            Assert.Equal("ijk", this.caseConverter.ConvertBack("ijk", null, null, cultureInfo));
            Assert.Equal("ıjk", this.caseConverter.ConvertBack("IJK", null, null, cultureInfo));
            Assert.Equal("ijk", this.caseConverter.ConvertBack("iJk", null, null, cultureInfo));

            this.caseConverter.SourceCasing = CharacterCasing.Upper;
            Assert.Equal("İJK", this.caseConverter.ConvertBack("ijk", null, null, cultureInfo));
            Assert.Equal("IJK", this.caseConverter.ConvertBack("IJK", null, null, cultureInfo));
            Assert.Equal("İJK", this.caseConverter.ConvertBack("iJk", null, null, cultureInfo));
        }
    }
}
