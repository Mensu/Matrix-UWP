using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace Matrix_UWP.ViewModel {
  class LibraryViewModel : BindableBase {
    private ObservableCollection<Model.Library> _list = new ObservableCollection<Model.Library>();
    public ObservableCollection<Model.Library> list {
      get {
        return this._list;
      }
      set {
        this.SetProperty(ref this._list, value);
      }
    }

    private bool _isLoading;
    public bool isLoading {
      get {
        return this._isLoading;
      }
      set {
        this.SetProperty(ref this._isLoading, value);
      }
    }

    public void updateWith(ObservableCollection<Model.Library> newList) {
      this.list = new ObservableCollection<Model.Library>(newList.OrderBy(one => one.lib_id));
    }

    public void toggleLoading() {
      this.isLoading = !this.isLoading;
    }
  }
}
