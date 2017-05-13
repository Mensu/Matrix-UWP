using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Matrix_UWP.Converters {
  class UserRoleToRoleString : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string language) {
      Model.Course.UserRole? role = value as Model.Course.UserRole?;
      string ret = "角色是";
      switch (role) {
        case Model.Course.UserRole.Teacher:
          ret += "老师";
          break;
        case Model.Course.UserRole.TA:
          ret += "助教";
          break;
        case Model.Course.UserRole.Student:
          ret += "学生";
          break;
        default:
          ret = "未知身份";
          break;
      }
      return ret;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      throw new NotImplementedException();
    }
  }
  class UserRoleToColor : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string language) {
      Model.Course.UserRole? role = value as Model.Course.UserRole?;
      Windows.UI.Color color;
      switch (role) {
        case Model.Course.UserRole.Teacher:
          color = Windows.UI.Color.FromArgb(255, 92, 184, 92);
          break;
        case Model.Course.UserRole.TA:
          color = Windows.UI.Color.FromArgb(255, 240, 173, 78);
          break;
        case Model.Course.UserRole.Student:
          color = Windows.UI.Color.FromArgb(255, 42, 126, 220);
          break;
        default:
          color = Windows.UI.Colors.Black;
          break;
      }
      return new Windows.UI.Xaml.Media.SolidColorBrush(color);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      throw new NotImplementedException();
    }
  }
}
