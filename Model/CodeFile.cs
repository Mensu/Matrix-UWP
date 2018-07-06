using Newtonsoft.Json.Linq;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_UWP.Model {
  public class CodeFile : BindableBase {
    public CodeFile(JToken codeFile, string language, bool readOnly = true) {
      Name = Helpers.Nullable.ToString(codeFile["name"], "未知");
      Code = Helpers.Nullable.ToString(codeFile["code"], "");
      ReadOnly = readOnly;
      Language = language;
    }
    public CodeFile(string name, string language, string code = "", bool readOnly = false) {
      Name = name;
      Code = code;
      Language = language;
      ReadOnly = readOnly;
    }
    private bool readOnly = false;
    public bool ReadOnly {
      get => readOnly;
      set => SetProperty(ref readOnly, value);
    }

    private string code = "";
    public string Code {
      get => code;
      set => SetProperty(ref code, value);
    }

    private string name = "";
    public string Name {
      get => name;
      set => SetProperty(ref name, value);
    }

    private string language = "text";
    public string Language {
      get => language;
      set => SetProperty(ref language, value);
    }
  }
}
