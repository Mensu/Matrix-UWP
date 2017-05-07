using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_UWP.Helpers {
  class ISOStringConverter {
    static public DateTimeOffset fromISOString(string ISOString) {
      return new DateTimeOffset(
        int.Parse(ISOString.Substring(0, 4)), int.Parse(ISOString.Substring(5, 2)), int.Parse(ISOString.Substring(8, 2)),
        int.Parse(ISOString.Substring(11, 2)), int.Parse(ISOString.Substring(14, 2)), int.Parse(ISOString.Substring(17, 2)),
        new TimeSpan()
      );
    }

    static public string toISOString(DateTimeOffset datetime) {
      return datetime.ToString("yyyy-MM-ddTHH:mm:ss.sssZ");
    }

    static public string toReadableString(DateTimeOffset datetime) {
      return datetime.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss");
    }
  }
}
