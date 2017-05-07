using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Prism.Mvvm;
using Windows.UI.Xaml;

namespace Matrix_UWP.Model {
  class Captcha : BindableBase {
    private const string defaultSvgText = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"150\" height=\"50\"><path fill=\"#222\" d=\"M56.02 34.52Q52.56 34.63 50.92 34.10Q48.94 33.45 48.64 30.06L50.08 22\"/></svg>";
    public Captcha(bool visible = false) {
      this.svgText = defaultSvgText;
      this.visible = visible ? Visibility.Visible : Visibility.Collapsed;
    }
    public Captcha(JToken token) {
      JObject data = token as JObject;
      this.svgText = data["captcha"].ToString();
      this.visible = Visibility.Visible;
    }

    private Visibility _visible;
    public Visibility visible {
      get {
        return this._visible;
      }
      set {
        this.SetProperty(ref this._visible, value);
      }
    }

    private string _svgText;
    public string svgText {
      get {
        return this._svgText;
      }
      set {
        this.SetProperty(ref this._svgText, value);
      }
    }
  }
}
