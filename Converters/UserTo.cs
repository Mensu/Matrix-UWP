using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Matrix_UWP.Converters {
  class UserToAvatar : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string language) {
      var curUser = value as Model.User;
      if (curUser == null) {
        return new BitmapImage(new Uri("ms-appx:///Assets/Login/Avatar.png"));
      } else {
        return new BitmapImage(Model.MatrixRequest.GetAvatarUri(curUser.username));
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      throw new NotImplementedException();
    }
  }
}
