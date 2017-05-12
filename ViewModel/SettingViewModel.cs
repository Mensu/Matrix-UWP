using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace Matrix_UWP.ViewModel {
  class SettingViewModel : BindableBase {
    private Model.User _curUser = new Model.User();
    public Model.User curUser {
      get { return this._curUser; }
      set { this.SetProperty(ref this._curUser, value); }
    }
  }
}
