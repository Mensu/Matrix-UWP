using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mntone.SvgForXaml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Matrix_UWP.Converters {
  public class CaptchaToContent : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string language) {
      return SvgDocument.Parse((value as Model.Captcha).svgText);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      throw new NotImplementedException();
    }
  }
}
