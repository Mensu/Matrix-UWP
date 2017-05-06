﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Matrix_UWP.Model {
  class Captcha : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;
    private const string defaultSvgText = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"150\" height=\"50\"><path fill=\"#222\" d=\"M56.02 34.52Q52.56 34.63 50.92 34.10Q48.94 33.45 48.64 30.06L50.08 22\"/></svg>";
    private bool _isValid;
    public bool isValid {
      get {
        return this._isValid;
      }
      set {
        this._isValid = value;
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("isValid"));
      }
    }

    private string _svgText;
    public string svgText {
      get {
        return this._svgText;
      }
      set {
        this._svgText = value;
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("svgText"));
      }
    }

    public Captcha() {
      this.svgText = defaultSvgText;
      this.isValid = false;
    }
    public Captcha(JToken token) {
      JObject data = token as JObject;
      this.svgText = data["captcha"].ToString();
      this.isValid = true;
    }
  }
}