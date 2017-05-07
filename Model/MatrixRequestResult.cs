using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Matrix_UWP.Model {
  class MatrixRequestResult {

    public bool success {
      get;
    }

    public string status {
      get;
    }

    public string msg {
      get;
    }

    public JToken data {
      get;
    }

    public MatrixRequestResult(JObject json) {
      this.status = json["status"].ToString();
      this.success = (this.status == "OK");
      this.msg = json["msg"].ToString();
      this.data = json["data"];
    }
  }
}
