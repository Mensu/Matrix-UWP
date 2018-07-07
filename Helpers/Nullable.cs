using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Matrix_UWP.Helpers {
  class Nullable {
    static public string ToString(JToken token, string defaultValue = "暂无信息") {
      return token == null ? defaultValue : token.ToString();
    }

    static public bool ToBool(JToken token, bool defaultValue = false) {
      return token == null ? defaultValue : token.ToObject<bool>();
    }

    static public int ToInt(JToken token, int defaultValue = 0) {
      return token == null ? defaultValue : token.ToObject<int>();
    }

    static public DateTimeOffset ToDateTimeOffset(JToken token, DateTimeOffset defaultValue) {
      return token == null ? defaultValue : token.ToObject<DateTimeOffset>();
    }
  }
}
