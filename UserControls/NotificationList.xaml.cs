using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

namespace Matrix_UWP.UserControls {
  public sealed partial class NotificationList : UserControl,Helpers.IHamburgerContent {
    internal ViewModel.NotificationViewModel vm = new ViewModel.NotificationViewModel();
    public NotificationList() {
      this.InitializeComponent();
    }
    
    public async Task refreshList() {
      var newList = this.vm.list;
      this.vm.isLoading = true;
      try {
        newList = await Model.MatrixRequest.getNotificationList();
      } catch (MatrixException.SoftError err) {
        Debug.WriteLine(err);
        return;
      } catch (MatrixException.FatalError err) {
        Debug.WriteLine(err);
        return;
      } finally {
        this.vm.isLoading = false;
      }
      this.vm.updateWith(newList);
    }

    private async void checkbox_Checked(object sender, RoutedEventArgs e) {
      var elem = sender as FrameworkElement;
      var notification = elem.DataContext as Model.Notification;
      if (notification == null) return;
      try {
        await notification.toggleReadState();
      } catch (MatrixException.MatrixException err) {
        Debug.WriteLine($"更改消息已读未读时出错:");
        Debug.WriteLine(err);
      }
    }

    public async Task ResetContentAsync() {
      await refreshList();
    }
  }
}
