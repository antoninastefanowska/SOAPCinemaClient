using System;
using System.Globalization;
using System.Windows.Data;

namespace CinemaClient
{
    public class EpochToDateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long epoch = (long)value;
            DateTime date = new DateTime(1970, 1, 1).AddMilliseconds(epoch);
            string dateString = date.ToString("dddd dd-MM-yyyy");
            return dateString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
