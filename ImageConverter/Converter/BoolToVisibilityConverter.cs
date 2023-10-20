using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ImageConverter.Converter
{
    class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var boolValue = (bool)value;
            if (parameter == null)
                return boolValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            if (parameter.Equals("reverse"))
                return boolValue ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;

            return System.Windows.Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
