using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Matrix_UWP.Helpers {
  class Nullable {
    static public string toString(JToken token, string defaultValue = "暂无信息") {
      return token == null ? defaultValue : token.ToString();
    }

    static public bool toBool(JToken token, bool defaultValue = false) {
      return token == null ? defaultValue : token.ToObject<bool>();
    }

    static public int toInt(JToken token, int defaultValue = 0) {
      return token == null ? defaultValue : token.ToObject<int>();
    }

    static public DateTimeOffset toDateTimeOffset(JToken token, DateTimeOffset defaultValue) {
      return token == null ? defaultValue : token.ToObject<DateTimeOffset>();
    }
  }
}
