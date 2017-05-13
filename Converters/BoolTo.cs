using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
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

  class BoolToOpacity : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string language) {
      bool? isChecked = value as bool?;
      switch (isChecked) {
        case true:
          return 1.0;
        default:
          return 0.0;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      throw new NotImplementedException();
    }
  }

  class BoolToHamburgerMenuWidth : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string language) {
      return (value ?? false).Equals(true) ? System.Convert.ToDouble(parameter ?? 0) : 48;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      throw new NotImplementedException();
    }
  }

  class BoolToCourseStatusString : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string language) {
      return (value ?? false).Equals(true) ? "进行中" : "已结束";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      throw new NotImplementedException();
    }
  }
  class BoolToCourseStatusColor : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string language) {
      return new SolidColorBrush((value ?? false).Equals(true) ? Colors.Green : Colors.Red);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      throw new NotImplementedException();
    }
  }
}
