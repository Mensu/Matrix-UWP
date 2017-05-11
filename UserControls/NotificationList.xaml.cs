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
        ShowError(err.Message);
        return;
      } catch (MatrixException.FatalError err) {
        ShowError(err.Message);
        return;
      } finally {
        this.vm.isLoading = false;
      }
      this.vm.updateWith(newList);
    }

    private void ShowError(string msg, UserControls.InfoMessage.MessageLevel level = UserControls.InfoMessage.MessageLevel.Warning) {
      this.Msg.Level = level;
      Msg.Text = msg;
      Msg.Show();
    }

    private async void checkbox_Checked(object sender, RoutedEventArgs e) {
      var notificationElem = sender as FrameworkElement;
      var notification = notificationElem.DataContext as Model.Notification;
      if (notification == null) return;
      await notification.toggleReadState();
    }

    public async Task ResetContentAsync() {
      await refreshList();
    }
  }
}
