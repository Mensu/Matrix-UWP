using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace Matrix_UWP.ViewModel {
  class NotificationViewModel : BindableBase {
    private List<Model.Notification> notifications = new List<Model.Notification>();
    public List<Model.Notification> Notifications {
      get => notifications;
      set {
        SetProperty(ref notifications, value);
        RaisePropertyChanged("FilteredNotification");
      }
    }

    public enum Status {
      All,
      Read,
      Unread,
    };

    private Status statusFilter = Status.All;
    public Status StatusFilter {
      get => statusFilter;
      set {
        SetProperty(ref statusFilter, value);
        RaisePropertyChanged("FilteredNotification");
      }
    }

    public List<Model.Notification> FilteredNotification {
      get => notifications.Where(notification => (statusFilter == Status.All || notification.is_read == (statusFilter == Status.Read))).ToList();
    }
  }
}
