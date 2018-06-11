using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Matrix_UWP.ViewModel {
  class HomeViewModel: BindableBase {
    public HomeViewModel() {
      not_corrected_list = new ObservableCollection<Model.Assignment>();
      unfinished_list = new ObservableCollection<Model.Assignment>();
    }

    private Model.User _currentUser = new Model.User();
    public Model.User currentUser {
      get {
        return _currentUser;
      }
      set {
        SetProperty(ref _currentUser, value);
      }
    }

    private BitmapImage _avatar;
    public BitmapImage avatar {
      get {
        return _avatar;
      }
      set {
        SetProperty(ref _avatar, value);
      }
    }

    private ObservableCollection<Model.Assignment> _not_corrected_list;
    public ObservableCollection<Model.Assignment> not_corrected_list {
      get { return _not_corrected_list; }
      set {
        RaisePropertyChanged("has_not_corrected");
        SetProperty(ref _not_corrected_list, value);
      }
    }

    private ObservableCollection<Model.Assignment> _unfinished_list;
    public ObservableCollection<Model.Assignment> unfinished_list {
      get { return _unfinished_list; }
      set {
        RaisePropertyChanged("has_unfinished");
        SetProperty(ref _unfinished_list, value);
      }
    }

    public bool has_not_corrected {
      get { return not_corrected_list.Count != 0; }
    }

    public bool has_unfinished {
      get { return unfinished_list.Count != 0; }
    }

    private string _matrix_notification;
    public string matrix_notification {
      get { return _matrix_notification; }
      set { SetProperty(ref _matrix_notification, value); }
    }

    public async Task update() {
      this.unfinished_list = await Model.MatrixRequest.GetUnfinishAssignment();
      this.not_corrected_list = await Model.MatrixRequest.GetUnjudgeAssignment();
      this.matrix_notification = await Model.MatrixRequest.GetMatrixNotification();
      Model.User curUser = await Model.MatrixRequest.GetProfile();
      if (curUser.user_id != _currentUser.user_id) {
        currentUser = curUser;
        avatar = new BitmapImage(Model.MatrixRequest.GetAvatarUri(curUser.username));
      }
    }
  }
}
