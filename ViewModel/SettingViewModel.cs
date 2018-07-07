using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace Matrix_UWP.ViewModel {
  class SettingViewModel : BindableBase {
    private Model.User curUser = new Model.User();
    public Model.User CurUser {
      get => curUser;
      set => SetProperty(ref curUser, value);
    }
  }
}
