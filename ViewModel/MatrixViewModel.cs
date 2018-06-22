using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_UWP.ViewModel {
  class MatrixViewModel : BindableBase {
    private Model.User user = new Model.User();
    public Model.User User {
      get => user;
      set => SetProperty(ref user, value);
    }

    private bool hasLibrary = false;
    public bool? HasLibrary {
      get => hasLibrary;
      set => SetProperty(ref hasLibrary, value ?? false);
    }

    private string title = null;
    public string Title {
      get => title;
      set => SetProperty(ref title, value);
    }

    private bool loading = false;
    public bool Loading {
      get => loading;
      set => SetProperty(ref loading, value);
    }
  }
}
