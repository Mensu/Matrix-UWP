using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Matrix_UWP.Converters {
  class DateTimeOffsetToString : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string language) {
      return Helpers.ISOStringConverter.toReadableString((DateTimeOffset)value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      return Helpers.ISOStringConverter.fromISOString(value as string);
    }
  }
  class DateTimeOffsetToOffsetString : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string language) {
      DateTimeOffset end = (value as DateTimeOffset?) ?? DateTimeOffset.Now;
      var offset = end - DateTimeOffset.Now;
      if (offset.Ticks < 0) {
        return "0";
      }
      return offset.Days.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      return Helpers.ISOStringConverter.fromISOString(value as string);
    }
  }
}
