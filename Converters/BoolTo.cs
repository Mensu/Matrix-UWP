using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Matrix_UWP.Converters {
  class BoolToGray : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string language) {
      bool? isChecked = value as bool?;
      var color = new Windows.UI.Color();
      switch (isChecked) {
        case true:
          color.R = 150;
          color.G = 150;
          color.B = 150;
          color.A = 255;
          break;
        default:
          color.R = 0;
          color.G = 0;
          color.B = 0;
          color.A = 255;
          break;
      }
      return new SolidColorBrush(color);
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      throw new NotImplementedException();
    }
  }
  class BoolToVisible : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string language) {
      bool? isChecked = value as bool?;
      switch (isChecked) {
        case true:
          return Visibility.Visible;
        default:
          return Visibility.Collapsed;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      throw new NotImplementedException();
    }
  }
}
