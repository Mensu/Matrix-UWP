using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Matrix_UWP.Model;

namespace Matrix_UWP.Converters {
  class AssignemtTypeToString : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string language) {
      string ret;
      Assignment.Type? astype = value as Model.Assignment.Type?;
      switch (astype) {
        case Assignment.Type.ProgrammingProblem:
        case Assignment.Type.RealtimeProgramming:
        case Assignment.Type.ScheduleProgramming:
          ret = "编程题";
          break;
        case Assignment.Type.Choice:
          ret = "选择题";
          break;
        case Assignment.Type.FileUpload:
          ret = "文件上传题";
          break;
        case Assignment.Type.ShortAnswer:
          ret = "简答题";
          break;
        case Assignment.Type.ProgrammingFillingBlank:
          ret = "程序填空题";
          break;
        case Assignment.Type.ProgrammingOutput:
          ret = "程序输出题";
          break;
        case Assignment.Type.Report:
          ret = "报告题";
          break;
        default:
          ret = "未知题型";
          break;
      }
      return ret;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      throw new NotImplementedException();
    }
  }
}
