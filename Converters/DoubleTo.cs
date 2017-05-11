using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Matrix_UWP.Converters {
  class DoubleOffset : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string language) {
      return System.Convert.ToDouble(value ?? 0) + System.Convert.ToDouble(parameter ?? 0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      throw new NotImplementedException();
    }
  }
}

