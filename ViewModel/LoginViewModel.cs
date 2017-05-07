using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace Matrix_UWP.ViewModel {
  class LoginViewModel : BindableBase {
    private Model.Captcha _captcha = new Model.Captcha();
    public Model.Captcha captcha {
      get {
        return this._captcha;
      }
      set {
        this.SetProperty(ref this._captcha, value);
      }
    }

    
  }
}
