using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Matrix_UWP.ViewModel {
  class AssignmentViewModel : BindableBase {
    private ObservableCollection<Model.Assignment> _progressingList = new ObservableCollection<Model.Assignment>();
    public ObservableCollection<Model.Assignment> progressingList {
      get {
        return this._progressingList;
      }
      set {
        this.SetProperty(ref this._progressingList, value);
      }
    }

    private ObservableCollection<Model.Assignment> _endedList = new ObservableCollection<Model.Assignment>();
    public ObservableCollection<Model.Assignment> endedList {
      get {
        return this._endedList;
      }
      set {
        this.SetProperty(ref this._endedList, value);
      }
    }

    private Model.Assignment _curAssignment = Model.Assignment.Null;
    public Model.Assignment curAssignment {
      get {
        return this._curAssignment;
      }
      set {
        RaisePropertyChanged("canShare");
        this.SetProperty(ref this._curAssignment, value);
      }
    }

    private bool _listIsLoading;
    public bool listIsLoading {
      get {
        return this._listIsLoading;
      }
      set {
        this.SetProperty(ref this._listIsLoading, value);
      }
    }

    private bool _detailIsLoading;
    public bool detailIsLoading {
      get {
        return this._detailIsLoading;
      }
      set {
        this.SetProperty(ref this._detailIsLoading, value);
      }
    }

    public void updateWith(ObservableCollection<Model.Assignment> newList) {
      var now = DateTimeOffset.Now;
      this.progressingList = new ObservableCollection<Model.Assignment>(newList.Where(one => one.startDate <= now && now <= one.endDate));
      this.endedList = new ObservableCollection<Model.Assignment>(newList.Where(one => one.endDate < now));
      this.curAssignment = Model.Assignment.Null;
    }

    public bool? canShare {
      get { return _curAssignment != Model.Assignment.Null; }
    }
  }
}
