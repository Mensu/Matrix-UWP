using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_UWP.ViewModel {
  class LibrariesViewModel : BindableBase {
    private List<Model.Library> libraries = new List<Model.Library>();
    public List<Model.Library> Libraries {
      get => libraries;
      set => SetProperty(ref libraries, value);
    }
  }
}
