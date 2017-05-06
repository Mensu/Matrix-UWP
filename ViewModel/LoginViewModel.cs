using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_UWP.ViewModel {
  class LoginViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;
    private Model.Captcha _captcha = new Model.Captcha();
    public Model.Captcha captcha {
      get {
        return this._captcha;
      }
      set {
        this._captcha = value;
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("captcha"));
      }
    }

    
  }
}
