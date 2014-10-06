using System;
using System.Globalization;
using System.Windows.Data;
using Utility.Common;

namespace Utility.Converters
{
    public class SizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format(new FileSizeFormatProvider(), "{0:fs}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
