using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FASTASelector.UserInterface
{
    public sealed class FontFamilyToStringConverter : IValueConverter
    {

        public FontFamilyToStringConverter( )
        {
            DefaultValue = "Consolas";
        }


        public string DefaultValue
        {
            get;
            set;
        }


        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if( value is FontFamily fontFamily )
            {
                return fontFamily.ToString( );
            }
            return DefaultValue;
        }


        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return Utility.CreateFontFamily( value?.ToString( ) );
        }

    }
}
