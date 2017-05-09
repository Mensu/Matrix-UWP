using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace Matrix_UWP.ViewModel {
  class NotificationViewModel : BindableBase {
    private ObservableCollection<Model.Notification> _list = new ObservableCollection<Model.Notification>() {
      new Model.Notification(),
      new Model.Notification()
    };
    public ObservableCollection<Model.Notification> list {
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

    public void updateWith(ObservableCollection<Model.Notification> newList) {
      this.list = newList;
    }

    public void toggleLoading() {
      this.isLoading = !this.isLoading;
    }
  }
}
