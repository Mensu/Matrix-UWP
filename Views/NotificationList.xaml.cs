using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Matrix_UWP.Views {
  public sealed partial class NotificationList : UserControl {
    internal ViewModel.NotificationViewModel vm = new ViewModel.NotificationViewModel();
    public NotificationList() {
      this.InitializeComponent();
    }

    private async void listView_ItemClick(object sender, ItemClickEventArgs e) {
      //var notification = e.ClickedItem as Model.Notification;
      //await notification.toggleReadState();
    }

    public async Task refreshList() {
      var newList = await Model.MatrixRequest.getNotificationList();
      this.vm.updateWith(newList);
    }

    private async void checkbox_Checked(object sender, RoutedEventArgs e) {
      var notificationElem = sender as FrameworkElement;
      var notification = notificationElem.DataContext as Model.Notification;
      if (notification == null) return;
      await notification.toggleReadState();
    }
  }
}
